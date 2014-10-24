using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace MagBot_FFXIV_v02
{
    internal class ExpFarming
    {
        private readonly Player _player;
        private readonly int _targetBaseOffset;
        private readonly int _targetTwoBaseOffset;

        private Thread _runningThread;
        private Thread _attackingThread;

        //These are thread safe bools for automation loops
        //bool is by default atomic (a read/write operation cannot be interrupted half-way), so locking is not needed
        //Fields that are declared volatile are not subject to compiler optimizations that assume access by a single thread.
        //This ensures that the most up-to-date value is present in the field at all times.
        private static volatile bool _running;
        private static volatile bool _attacking;
        private static volatile bool _escaping;

        //CONSTANTS
        public const int MaxPullingDistance = 25; //Max distance one can pull from (magic or archery reach)
        private const int MaxDistanceFromPath = 50;
        public const int WaypointTurnDistance = 6;
        public const int AttackingDistance = 8;

        private bool RestartExpFarming { get; set; }

        public static bool StandStill { get; private set; }

        public static bool Running
        {
            get { return _running; }
            set { _running = value; }
        }

        public static bool Attacking
        {
            get { return _attacking; }
            private set { _attacking = value; }
        }

        private static bool Escaping
        {
            get { return _escaping; }
            set { _escaping = value; }
        }

        public static bool Aggro { get; set; }

        public ExpFarming(Player player)
        {
            _player = player;
            _targetBaseOffset = Globals.Instance.MemoryBaseOffsetDictionary["Target"];
            _targetTwoBaseOffset = Globals.Instance.MemoryBaseOffsetDictionary["Target2"];
            Globals.Instance.ExpFamingLogger = new Logger("ExpFarmingLog");
        }

        public void StartExpFarming(Route startingRoute, Waypoint startingWaypoint, Route escapeRoute, bool standStill)
        {
            StandStill = standStill;

            //Ensure NUMLOCK is on (has to use SendInput)
            if (!MemoryHandler.IsKeyLocked(MemoryApi.KeyCode.NUMLOCK))
            {
                Globals.Instance.ExpFamingLogger.Log("Turning NUMLOCK on...");
                MemoryHandler.HumanKeyPress(MemoryApi.KeyCode.NUMLOCK);
            }

            Globals.Instance.ExpFamingLogger.Log("=====!!!!===== Initiating Exp Farming =====!!!!=====");

            //Heal up before we start
            if (_player.HP < _player.MaxHP || _player.MP < _player.MaxMP) Globals.Instance.ExpFamingLogger.Log("Healing up prior to starting exp farming...");
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
                    Globals.Instance.ExpFamingLogger.Log("Being attacked while resting prior to start of exp farming. Stopping farming...");
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

        public void StopExpFarming() 
        {
            Globals.Instance.ExpFamingLogger.Log("=====!!!!===== Exp farming stopped, releasing threads... =====!!!!===== ");
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
                Globals.Instance.ExpFamingLogger.Log("Too close to starting waypoint. Skipping to next waypoint...");
                waypoint = route.NextWaypoint(waypoint);
            }
            
            Globals.Instance.KeySenderInstance.SendDown(Keys.W);

            while (Running)
            {
                Globals.Instance.ExpFamingLogger.Log("Running to point: " + route.Points.IndexOf(waypoint));
                string runningOutcome = _player.RunToPoint(waypoint, () => Running);

                switch(runningOutcome) 
                {
                    case ("died"):
                        Globals.Instance.ExpFamingLogger.Log("Died while running...");
                        MainForm.Get.BeginInvoke(MainForm.Get.SEFDelegate);
                        return;
                    case("stuck"):
                        Globals.Instance.ExpFamingLogger.Log("Stuck while running. Trying to turn and select new point");
                        _player.Turn180();
                        break;
                    case ("point reached"):
                        Globals.Instance.ExpFamingLogger.Log("Reached point: " + route.Points.IndexOf(waypoint));
                        break;
                    case("running cancelled"):
                        Globals.Instance.ExpFamingLogger.Log("RunToPoint() cancelled");
                        return;
                }

                //We have reached end of escape route
                if (Escaping && (waypoint == route.Points.Last()))
                {
                    Globals.Instance.ExpFamingLogger.Log("End of escape route reached..");

                    if (RestartExpFarming)
                    {
                        Globals.Instance.ExpFamingLogger.Log("Escape successful. Exp farming restarting shortly...");
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

            Globals.Instance.ExpFamingLogger.Log("RunningHandler thread terminated...");
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
                Character target = _player.GetTarget(_targetTwoBaseOffset);
                if (target != null)
                {
                    Globals.Instance.ExpFamingLogger.Log("Targeting aggressor...");
                    Aggro = true;
                }

                //If no aggro, make sure we aren't too far from first waypoint, then try regular targetting
                if (target == null)
                {
                    //If no aggro and attacking-spree has taken player too far from first point on route, run back there
                    //AttackThread is slept until player is back at that waypoint
                    if (!StandStill && _player.WaypointLocation.Distance(route.ClosestWaypoint(_player.WaypointLocation)) > MaxDistanceFromPath)
                    {
                        Globals.Instance.ExpFamingLogger.Log("Now too far from closest waypoint. Returning to waypoint 1...");
                        StartRunning(route, route.Points[0], true);
                        continue;
                    }

                    Globals.Instance.KeySenderInstance.SendKey(Keys.Tab);
                    target = _player.GetTarget(_targetBaseOffset);
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
                            target = _player.GetTarget(_targetBaseOffset);
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
                            Globals.Instance.ExpFamingLogger.Log("Danger in battle, initiating escape...");
                            RestartExpFarming = true;
                            Escape(escapeRoute);
                            return;
                        case "Stuck. Jumping not successful.":
                            Globals.Instance.ExpFamingLogger.Log("Stuck. Jumping not successful. Attempting to turn around and pick another target...");
                            Globals.Instance.KeySenderInstance.SendKey(Keys.Escape);
                            _player.Turn180();
                            break;
                        case "target too far":
                            Globals.Instance.ExpFamingLogger.Log("Target now too far away. Proceeding to search for next target...");
                            Globals.Instance.KeySenderInstance.SendKey(Keys.Escape);
                            break;
                        case ("target defeated"):
                            Globals.Instance.ExpFamingLogger.Log("Attack successful. Proceeding to search for next target...");
                            break;
                        case ("attack cancelled"):
                            Globals.Instance.ExpFamingLogger.Log("Attack cancelled...");
                            return;
                    }
                    continue;
                }

                //No target found, going back to closest current waypoint and start running path again while searching for targets
                //Globals.Instance.ExpFamingLogger.Log("Could not find any targets. Continuing on path...");
                if (StandStill) continue;

                StartRunning(route, route.ClosestWaypoint(_player.WaypointLocation), false);
            }

            //Important because attacking can be stopped while w is still pressed
            ReleaseAllButtons();

            Globals.Instance.ExpFamingLogger.Log("AttackHandler thread terminated...");
        }

        //Restarting running from specified waypoint
        //This method is only called from the AttackHandler thread and main thread (through escape)
        private void StartRunning(Route restartFromRoute, Waypoint restartFromWp, bool returnAllTheWay)
        {
            //The Join ensures the calling thread waits until _runningThread is terminated
            //This is often called while _runningThread is alive (tab that is not directly after an attack)

            if (!Running)
            {
                Globals.Instance.ExpFamingLogger.Log("StartRunning() initiated from thread: " + Thread.CurrentThread.Name +". Starting _runningThread from: " +
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
            //    Globals.Instance.ExpFamingLogger.Log("StartRunning() entered without restarting _runningThread...");
            //}

            //If returnAllTheWay flag is set, give it time to return to the waypoint before we continue next action
            //This method is only called from the AttackHandler thread, so it sleeps Attacking thread
            if (!returnAllTheWay) return;
            Globals.Instance.ExpFamingLogger.Log("Returning all the way to waypoint " + restartFromRoute.Points.IndexOf(restartFromWp) /+ 1 + " before proceeding with next action...");
            while (restartFromWp.Distance(_player.WaypointLocation) > WaypointTurnDistance) Thread.Sleep(1000);
        }

        //Only called from AttackHandler thread / UI thread
        //Join ensures attackhandler thread waits until the running thread is terminated
        private void StopRunning()
        {
            if (!Running) return;
            if (Thread.CurrentThread == _runningThread)
            {
                Globals.Instance.ExpFamingLogger.Log(
                    "StopRunning() called from _runningThread, setting Running bool to false...");
                Running = false;
            }
            else
            {
                Globals.Instance.ExpFamingLogger.Log("StopRunning() called from thread: " + Thread.CurrentThread.Name + ". Setting Running bool to false and waiting for _runningThread to terminate....");
                Running = false;
                _runningThread.Join();
            }
        }

        private void StopAttacking()
        {
            if (!Attacking) return;
            if (Thread.CurrentThread == _attackingThread)
            {
                Globals.Instance.ExpFamingLogger.Log(
                    "StopAttacking() called from _attackingThread, setting Attacking bool to false...");
                Attacking = false;
            }
            else
            {
                Globals.Instance.ExpFamingLogger.Log("StopAttacking() called from thread: " + Thread.CurrentThread.Name + ". Setting Attacking bool to false and waiting for _attackingThread to terminate...");
                Attacking = false;
                _attackingThread.Join();
            }
        }

        public void Escape(Route escapeRoute)
        {
            Globals.Instance.ExpFamingLogger.Log("Initiating escape from closest waypoint through " + escapeRoute.Name);
            Escaping = true;
            StopAttacking();
            StopRunning();

            StartRunning(escapeRoute, escapeRoute.ClosestWaypoint(_player.WaypointLocation), true);
        }

        private void OnTimedEvent_ExpFarmingRestart(object sender, EventArgs e)
        {
            MainForm.Get.Invoke(new Action(() => MainForm.Get.btStartExpFarming_Click(sender, e)));
        }

        private void ReleaseAllButtons()
        {
            Globals.Instance.ExpFamingLogger.Log("Releasing all buttons...");
            Globals.Instance.KeySenderInstance.SendUp(Keys.A);
            Globals.Instance.KeySenderInstance.SendUp(Keys.D);
            Globals.Instance.KeySenderInstance.SendUp(Keys.W);
            Globals.Instance.KeySenderInstance.SendUp(Keys.S);
        }
    }
}