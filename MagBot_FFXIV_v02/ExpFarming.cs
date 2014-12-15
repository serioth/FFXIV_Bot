using System;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace MagBot_FFXIV_v02
{
    internal class ExpFarming
    {
        //task branch
        private readonly Player _player;
        private readonly int _targetBaseOffset;
        private readonly int _targetTwoBaseOffset;

        private Thread _runningThread;
        private Thread _attackingThread;

        //These are thread safe bools for automation loops
        //bool is by default atomic (a read/write operation cannot be interrupted half-way), so locking is not needed
        //Fields that are declared volatile are not subject to compiler optimizations that assume access by a single thread.
        //This ensures that the most up-to-date value is present in the field at all times.
        private static volatile bool _escaping;

        //CONSTANTS
        private const int MaxPullingDistance = 25; //Max distance one can pull from (magic or archery reach)
        private const int MaxDistanceFromPath = 50;
        public const int DistanceTreshold = 6;

        private bool RestartExpFarming { get; set; }

        private Waypoint CurrentWaypoint { get; set; }

        private static CancellationTokenSource RunningCts { get; set; }

        private CancellationTokenSource AttackingCts { get; set; }

        private Timer EnemySearchTimer { get; set; }

        private static bool Escaping
        {
            get { return _escaping; }
            set { _escaping = value; }
        }

        public ExpFarming(Player player)
        {
            _player = player;
            _targetBaseOffset = Globals.Instance.MemoryBaseOffsetDictionary["Target"];
            _targetTwoBaseOffset = Globals.Instance.MemoryBaseOffsetDictionary["Target2"];
            Globals.Instance.ExpFarmingLogger = new Logger("ExpFarmingLog");
        }

        private void OnElapsedEventFindEnemy(Player player, CancellationToken ct, Route escapeRoute)
        {
            //First check if any enemies are attacking us
            var aggro = false;
            var target = Player.AggroCheck();
            if (target != null) aggro = true;
            else target = Player.EnemyCheck();

            EnemySearchTimer.Stop();
            RunningCts.Cancel();
            PursueEnemy(player, target, ct, aggro, escapeRoute);
            //TODO: If enemy found, stop running, stop checking for enemies, and initiate EnemyPursuit
            //TODO: The results from EnemyPursuit can propagate back up here as this is the highest level
        }

        public void StartExpFarming(Player player, Route startingRoute, Waypoint startingWaypoint, Route escapeRoute, bool standStill)
        {
            EnemySearchTimer = new Timer(1000) {AutoReset = false};
            EnemySearchTimer.Elapsed += (sender, args) => OnElapsedEventFindEnemy(player, AttackingCts.Token, escapeRoute);

            //Ensure NUMLOCK is on (has to use SendInput)
            if (!MemoryHandler.IsKeyLocked(MemoryApi.KeyCode.NUMLOCK))
            {
                Globals.Instance.ExpFarmingLogger.Log("Turning NUMLOCK on...");
                MemoryHandler.HumanKeyPress(MemoryApi.KeyCode.NUMLOCK);
            }

            Globals.Instance.ExpFarmingLogger.Log("=====!!!!===== Initiating Exp Farming =====!!!!=====");

            //Heal up before we start
            if (_player.HP < _player.MaxHP || _player.MP < _player.MaxMP) Globals.Instance.ExpFarmingLogger.Log("Healing up prior to starting exp farming...");
            while ((_player.HP < _player.MaxHP*0.9 || _player.MP < _player.MaxMP*0.9))
            {
                int tempHP = _player.HP;
                Thread.Sleep(1000);
                if (_player.HP < tempHP)
                {
                    //Set MainForm Buttons to right visability and Stop Exp Farming
                    //If we use Invoke: _runningThread asks UI thread to call StopExpFarming(), which freezes the UI thread until running thread is complete (Join())
                    //The _runningThread can never complete though, because Invoke is synchronous: Caller (_runningThread) is blocked until the marshal is complete (method returns)
                    //We have to use BeginInvoke to avoid this Deadlock
                    //BeginInvoke works asynchronously: Caller returns immediately and the marshaled request is queued up. In other words, it does not block _runningThread from completing
                    Globals.Instance.ExpFarmingLogger.Log("Being attacked while resting prior to start of exp farming. Stopping farming...");
                    MainForm.Get.BeginInvoke(MainForm.Get.SEFDelegate);
                    return;
                }
            }


            if (!StandStill)
            {
                StartRunning(startingRoute, startingWaypoint, true);
            }

            Attacking = true;
            _attackingThread = new Thread(() => AttackHandler(startingRoute, escapeRoute))
            {
                IsBackground = true,
                Name = "Attacking thread"
            };
            _attackingThread.Start();
        }

        private void RunRouteToPoint(Player player, Route route, Waypoint goalWp, CancellationToken ct, bool escaping)
        {
            var cts = new CancellationTokenSource();
            var wp = route.ClosestWaypoint(player.WaypointLocation);
            Globals.Instance.KeySenderInstance.SendDown(Keys.W);
            while (wp != goalWp)
            {
                player.RunToPoint(() => wp, ct, false, escaping);
                wp = route.NextWaypoint(wp);
            }
            Globals.Instance.KeySenderInstance.SendUp(Keys.W);
        }

        private void PursueEnemy(Player player, Character target, CancellationToken ct, bool aggro, Route escapeRoute)
        {
            //If we spotted an enemy, but it's level is too high, ignore it
            //If we get aggro from an enemy and its level is too high, escape
            if (target.Level > player.Level + 5)
            {
                if (!aggro) return;
                InitiateEscape(player, escapeRoute, ct);
                return;
            };

            switch (player.RunToPoint(() => target.WaypointLocation, ct, true, aggro))
            {
                case ("RunToPoint() cancelled"): return;
                case ("died"): StopExpFarming(); return;
                case ("stuck"): break; //If stuck we still try to attack aggressor as it will come to us
                case ("reached"): break; //Attack aggressor
                case ("aggro"):
                    var aggressor = Player.AggroCheck();
                    if (aggressor != null) PursueEnemy(player, aggressor, ct, true, escapeRoute);
                    return; //If we defeat aggressor, we do not want to continue on the enemy we persued before. Start from scratch
            }
            switch (player.AttackTarget(target, ct))
            {
                case ("cancelled"):
                    return;
                case ("escape"):
                    InitiateEscape(player,escapeRoute, ct);
            }
        }


        private void InitiateEscape(Player player, Route escapeRoute, CancellationToken ct)
        {
            Globals.Instance.ExpFarmingLogger.Log("Escape initiated...");
            RunRouteToPoint(player, escapeRoute, escapeRoute.Points[0], ct, true);
        }

        public void StopExpFarming() 
        {
            Globals.Instance.ExpFarmingLogger.Log("=====!!!!===== Exp farming stopped, releasing threads... =====!!!!===== ");
            StopAttacking();
            StopRunning();
            RestartExpFarming = false;
            Escaping = false;
            ReleaseAllButtons();
        }

        private void RunningHandler(Route route, Waypoint waypoint)
        {
            //Check if too close to starting-waypoint
            if (_player.WaypointLocation.Distance(waypoint) < 5)
            {
                Globals.Instance.ExpFarmingLogger.Log("Too close to starting waypoint. Skipping to next waypoint...");
                waypoint = route.NextWaypoint(waypoint);
            }
            
            Globals.Instance.KeySenderInstance.SendDown(Keys.W);

            var cts = new CancellationTokenSource();

            while (Running)
            {
                Globals.Instance.ExpFarmingLogger.Log("Running to point: " + route.Points.IndexOf(waypoint));
                var runningOutcome = _player.RunToPoint(() => CurrentWaypoint, cts.Token);

                switch(runningOutcome.Result) 
                {
                    case ("died"):
                        Globals.Instance.ExpFarmingLogger.Log("Died while running...");
                        MainForm.Get.BeginInvoke(MainForm.Get.SEFDelegate);
                        return;
                    case("stuck"):
                        Globals.Instance.ExpFarmingLogger.Log("Stuck while running. Trying to turn and select new point");
                        Player.Turn180();
                        break;
                    case ("point reached"):
                        Globals.Instance.ExpFarmingLogger.Log("Reached point: " + route.Points.IndexOf(waypoint));
                        break;
                    case("running cancelled"):
                        Globals.Instance.ExpFarmingLogger.Log("RunToPoint() cancelled");
                        return;
                }

                //We have reached end of escape route
                if (Escaping && (waypoint == route.Points.Last()))
                {
                    Globals.Instance.ExpFarmingLogger.Log("End of escape route reached..");

                    if (RestartExpFarming)
                    {
                        Globals.Instance.ExpFarmingLogger.Log("Escape successful. Exp farming restarting shortly...");
                        //It does not matter that the minutes here is small
                        //because when the farming restarts the player waits until healed
                        const int minutesToRestart = 1;
                        var timer = new Timer { Interval = minutesToRestart * 60000, AutoReset = false };
                        timer.Elapsed += OnTimedEvent_ExpFarmingRestart;
                        timer.Start();
                    }

                    MainForm.Get.BeginInvoke(MainForm.Get.SEFDelegate);
                    return;
                }

                waypoint = route.NextWaypoint(waypoint);
            }

            //Important because running is often stopped while w/a/d is still pressed
            ReleaseAllButtons();

            Globals.Instance.ExpFarmingLogger.Log("RunningHandler thread terminated...");
        }

        //Tabs for enemies and reacts to outcome of attacking
        //It only restarts running in the cases that it has tried to attack something (so _running is then always false)
        private void AttackHandler(Route route, Route escapeRoute)
        {
            while (Attacking)
            {
                Thread.Sleep(Utils.getRandom(200, 400)); //Sleep before checking for targets again

                //First check if any enemies are attacking us
                Aggro = false;
                Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad8, KeySender.ModifierControl);
                Character target = Player.GetTarget(_targetTwoBaseOffset);
                if (target != null)
                {
                    Globals.Instance.ExpFarmingLogger.Log("Targeting aggressor...");
                    Aggro = true;
                }

                //If no aggro, make sure we aren't too far from first waypoint, then try regular targetting
                if (target == null)
                {
                    //If no aggro and attacking-spree has taken player too far from first point on route, run back there
                    //AttackThread is slept until player is back at that waypoint
                    if (!StandStill && _player.WaypointLocation.Distance(route.ClosestWaypoint(_player.WaypointLocation)) > MaxDistanceFromPath)
                    {
                        Globals.Instance.ExpFarmingLogger.Log("Now too far from closest waypoint. Returning to waypoint 1...");
                        StartRunning(route, route.Points[0], true);
                        continue;
                    }

                    Globals.Instance.KeySenderInstance.SendKey(Keys.Tab);
                    target = Player.GetTarget(_targetBaseOffset);
                }

                //If any enemy targeted, attack it
                if (target != null)
                {
                    if (!Aggro && StandStill)
                    {
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        while (Attacking && target.WaypointLocation.Distance(_player.WaypointLocation) > MaxPullingDistance)
                        {
                            Globals.Instance.KeySenderInstance.SendKey(Keys.Tab);
                            target = Player.GetTarget(_targetBaseOffset);
                            Thread.Sleep(Utils.getRandom(500, 1000));
                        }
                    }

                    //If this is the first enemy we target while running loop, then Running bool == true.
                    //If it is a new enemy, then it is false. Check is done in method
                    StopRunning();

                    string attackOutcome = _player.AttackTarget(target);

                    switch (attackOutcome)
                    {
                        case "escape":
                            if (StandStill) return;
                            Globals.Instance.ExpFarmingLogger.Log("Danger in battle, initiating escape...");
                            RestartExpFarming = true;
                            Escape(escapeRoute);
                            return;
                        case "Stuck. Jumping not successful.":
                            Globals.Instance.ExpFarmingLogger.Log("Stuck. Jumping not successful. Attempting to turn around and pick another target...");
                            Globals.Instance.KeySenderInstance.SendKey(Keys.Escape);
                            Player.Turn180();
                            break;
                        case "target too far":
                            Globals.Instance.ExpFarmingLogger.Log("Target now too far away. Proceeding to search for next target...");
                            Globals.Instance.KeySenderInstance.SendKey(Keys.Escape);
                            break;
                        case ("target defeated"):
                            Globals.Instance.ExpFarmingLogger.Log("Attack successful. Proceeding to search for next target...");
                            break;
                        case ("attack cancelled"):
                            Globals.Instance.ExpFarmingLogger.Log("Attack cancelled...");
                            return;
                    }
                    continue;
                }

                //No target found, going back to closest current waypoint and start running path again while searching for targets
                //Globals.Instance.ExpFarmingLogger.Log("Could not find any targets. Continuing on path...");
                if (StandStill) continue;

                StartRunning(route, route.ClosestWaypoint(_player.WaypointLocation), false);
            }

            //Important because attacking can be stopped while w is still pressed
            ReleaseAllButtons();

            Globals.Instance.ExpFarmingLogger.Log("AttackHandler thread terminated...");
        }

        //Restarting running from specified waypoint
        //This method is only called from the AttackHandler thread and main thread (through escape)
        private void StartRunning(Route restartFromRoute, Waypoint restartFromWp, bool returnAllTheWay)
        {
            //The Join ensures the calling thread waits until _runningThread is terminated
            //This is often called while _runningThread is alive (tab that is not directly after an attack)

            if (!Running)
            {
                Globals.Instance.ExpFarmingLogger.Log("StartRunning() initiated from thread: " + Thread.CurrentThread.Name +". Starting _runningThread from: " +
                                                                       restartFromRoute.Name + ", Waypoint: " +
                                                                       restartFromRoute.Points.IndexOf(restartFromWp) /+ 1);
                //StopRunning();
                Running = true;

                _runningThread = new Thread(() => RunningHandler(restartFromRoute, restartFromWp))
                {
                    IsBackground = true,
                    Name = "Running thread"
                };
                _runningThread.Start();
            }
            //else
            //{
            //    Globals.Instance.ExpFarmingLogger.Log("StartRunning() entered without restarting _runningThread...");
            //}

            //If returnAllTheWay flag is set, give it time to return to the waypoint before we continue next action
            //This method is only called from the AttackHandler thread, so it sleeps Attacking thread
            if (!returnAllTheWay) return;
            Globals.Instance.ExpFarmingLogger.Log("Returning all the way to waypoint " + restartFromRoute.Points.IndexOf(restartFromWp) /+ 1 + " before proceeding with next action...");
            while (restartFromWp.Distance(_player.WaypointLocation) > DistanceTreshold) Thread.Sleep(1000);
        }

        //Only called from AttackHandler thread / UI thread
        //Join ensures attackhandler thread waits until the running thread is terminated
        private void StopRunning()
        {
            if (!Running) return;
            if (Thread.CurrentThread == _runningThread)
            {
                Globals.Instance.ExpFarmingLogger.Log(
                    "StopRunning() called from _runningThread, setting Running bool to false...");
                Running = false;
            }
            else
            {
                Globals.Instance.ExpFarmingLogger.Log("StopRunning() called from thread: " + Thread.CurrentThread.Name + ". Setting Running bool to false and waiting for _runningThread to terminate....");
                Running = false;
                _runningThread.Join();
            }
        }

        private void StopAttacking()
        {
            if (!Attacking) return;
            if (Thread.CurrentThread == _attackingThread)
            {
                Globals.Instance.ExpFarmingLogger.Log(
                    "StopAttacking() called from _attackingThread, setting Attacking bool to false...");
                Attacking = false;
            }
            else
            {
                Globals.Instance.ExpFarmingLogger.Log("StopAttacking() called from thread: " + Thread.CurrentThread.Name + ". Setting Attacking bool to false and waiting for _attackingThread to terminate...");
                Attacking = false;
                _attackingThread.Join();
            }
        }

        public void Escape(Route escapeRoute)
        {
            Globals.Instance.ExpFarmingLogger.Log("Initiating escape from closest waypoint through " + escapeRoute.Name);
            Escaping = true;
            StopAttacking();
            StopRunning();

            StartRunning(escapeRoute, escapeRoute.ClosestWaypoint(_player.WaypointLocation), true);

            var aTimer = new Timer(200);
            aTimer.Elapsed += delegate(object sender, ElapsedEventArgs e) { CheckDistance(1); };
            aTimer.Enabled = true;
        }

        private void CheckDistance(int i)
        {
        }

        private void OnTimedEvent_ExpFarmingRestart(object sender, EventArgs e)
        {
            MainForm.Get.Invoke(new Action(() => MainForm.Get.btStartExpFarming_Click(sender, e)));
        }

        private void ReleaseAllButtons()
        {
            Globals.Instance.ExpFarmingLogger.Log("Releasing all buttons...");
            Globals.Instance.KeySenderInstance.SendUp(Keys.A);
            Globals.Instance.KeySenderInstance.SendUp(Keys.D);
            Globals.Instance.KeySenderInstance.SendUp(Keys.W);
            Globals.Instance.KeySenderInstance.SendUp(Keys.S);
        }
    }
}