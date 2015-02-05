using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagBot_FFXIV_v02
{
    internal class FarmingHandler2
    {
        private readonly Player _player;
        public readonly ManualResetEvent FarmingMre;
        private readonly ManualResetEvent _runningMre;

        //CONSTANTS
        public const int MaxPullingDistance = 25; //Max distance one can pull from (magic or archery reach)
        private const int MaxDistanceFromPath = 50;
        public const int MaxEnemyLevelDiff = 4;
        private const int FightingDistance = 15;
        private const int PointTurnDistance = 4;
        private const int GatherDistance = 2;

        private int _totalXpEarned;
        private int _preBattleXp;
        public static string[] EnemyList; //Bad paractice, but needed access to this list in EnemyCheck(), which in turn is called from both methods in this class and in Player

        public FarmingHandler2(Player player)
        {
            _player = player;
            FarmingMre = new ManualResetEvent(false);
            _runningMre = new ManualResetEvent(false);

            //Ensure NUMLOCK is on (has to use SendInput)
            if (!MemoryHandler.IsKeyLocked(MemoryApi.KeyCode.NUMLOCK))
            {
                Globals.Instance.GameLogger.Log("Turning NUMLOCK on...");
                MemoryHandler.HumanKeyPress(MemoryApi.KeyCode.NUMLOCK);
            }

            _totalXpEarned = 0;
            _preBattleXp = _player.GetXp();
        }

        public void StartStandStillExpFarming(Route escapeRoute)
        {
            Globals.Instance.GameLogger.Log("=====!!!!===== Initiating Stand-Still Exp Farming =====!!!!=====");
            FarmingMre.Reset();
            while (!FarmingMre.WaitOne(0))
            {
                FarmingMre.WaitOne(Utils.getRandom(300, 500)); //Pause between AggroCheck and EnemyCheck
                var target = _player.AggroCheck(FarmingMre);
                if (target != null)
                {
                    Globals.Instance.GameLogger.Log("Aggressive enemy found. Attacking it...");
                    EngageHandler(target, escapeRoute, escapeRoute, false, FarmingMre, false, false);
                    continue;
                }
                FarmingMre.WaitOne(Utils.getRandom(300, 500)); //Pause between AggroCheck and EnemyCheck
                target = _player.EnemyCheck(FarmingMre);
                if (target != null)
                {
                    if (_player.WaypointLocation.Distance(target.WaypointLocation) > MaxPullingDistance) continue;
                    Globals.Instance.GameLogger.Log("Non-aggressive enemy found. Attacking it...");
                    EngageHandler(target, escapeRoute, escapeRoute, true, FarmingMre, false, false);
                    continue;
                }
                Globals.Instance.GameLogger.Log("StandStillExpFarming() found no enemies, proceeding to check again...");
            }
            Globals.Instance.GameLogger.Log("StandStillExpFarming() complete.");
        }

        public void StartExpFarming(Route route, Route escapeRoute, string[] enemyList)
        {
            FarmingMre.Reset();
            EnemyList = enemyList;
            Globals.Instance.GameLogger.Log("=====!!!!===== Initiating Exp Farming =====!!!!=====");
            while ((_player.HP < _player.MaxHP * 0.9 || _player.MP < _player.MaxMP * 0.9)) FarmingMre.WaitOne(1000); //Heal up before start

            Globals.Instance.GameLogger.Log("Running to first waypoint...");
            //var closestWaypointIndex = route.Points.IndexOf(route.ClosestWaypoint(_player.WaypointLocation));
            //RunningHandler(route, escapeRoute, false, true, PointTurnDistance, FarmingMre, closestWaypointIndex, true); //Don't start farming until at path, so run to closest point before starting
            RunningHandler(route, escapeRoute, true, true, PointTurnDistance, FarmingMre);

            //RunningHandler(route, escapeRoute, false, false, PointTurnDistance, FarmingMre); //If we just want to test running and not attacking, comment the above and uncomment this
        }

        public void StartGathering(Route route, Route escapeRoute)
        {
            Globals.Instance.GameLogger.Log("=====!!!!===== Initiating Gathering =====!!!!=====");
            FarmingMre.Reset();
            _runningMre.Reset();
            var locker = new object();
            var gatheringAre = new AutoResetEvent(false);
            var searchThread = new Thread(s => SearchAndGather(FarmingMre, gatheringAre, _runningMre, locker, route, escapeRoute, true));
            EnemyList = new[] { "Tree", "Vegetation" };

            while ((_player.HP < _player.MaxHP * 0.9 || _player.MP < _player.MaxMP * 0.9)) FarmingMre.WaitOne(1000); //Heal up before start

            searchThread.Start();

            while (!FarmingMre.WaitOne(0))
            {
                lock (locker)
                {
                    gatheringAre.Set();
                    RunningHandler(route, escapeRoute, false, false, PointTurnDistance, _runningMre);
                    _runningMre.Reset();
                }
                gatheringAre.WaitOne(); //Ensures S&G gets lock first
            }
            Globals.Instance.GameLogger.Log("StartGathering() complete.");
        }

        private void SearchAndGather(ManualResetEvent mre, AutoResetEvent are, ManualResetEvent runningMre, object locker, Route route, Route escapeRoute, bool aggroCheck)
        {
            while (!mre.WaitOne(0))
            {
                Character target = null;
                are.WaitOne(); //Ensures running starts first
                while (!mre.WaitOne(0))
                {
                    mre.WaitOne(Utils.getRandom(300, 500));
                    target = _player.NpcCheck(mre);
                    if (target != null) break;
                }

                runningMre.Set(); //Ensures Running is halted
                lock (locker)
                {
                    are.Set(); //Allows running thread to move to wait position at its lock
                    if (mre.WaitOne(0)) return;

                    //Run to point
                    Character aggressor;
                    var outcome = _player.RunToTarget(target, aggroCheck, GatherDistance, mre, out aggressor);
                    if (outcome == "dead")
                    {
                        StopApp();
                        break;
                    }
                    if (outcome == "canceled")
                    {
                        break;
                    }
                    if (outcome == "stuck") //Don't move away if we are running to aggro and get stuck (aggroCheck is false).
                    {
                        mre.WaitOne(500);
                        Globals.Instance.KeySenderInstance.SendDown(Keys.W);
                        Player.Turn180(mre);
                        Globals.Instance.KeySenderInstance.SendUp(Keys.W);
                    }
                    if (aggroCheck && outcome == "aggro" && aggressor != null)
                    {
                        InitiateEscape(escapeRoute, true, mre);
                        continue;
                    }

                    EngageHandler(target, route, escapeRoute, true, mre, false, true, true);
                }
            }
            Globals.Instance.GameLogger.Log("SearchAndGather() complete.");
        }

        private void RunningHandler(Route route, Route escapeRoute, bool enemyCheck, bool aggroCheck, int distanceTreshold, ManualResetEvent mre, int goalWp = -1, bool oppositeWay = false)
        {
            Globals.Instance.GameLogger.Log("RunningHandler() initiated...");
            var onlyAggro = goalWp > -1;
            while (!mre.WaitOne(0))
            {
                Character target;
                mre.WaitOne(500); //Wait before we start running route again
                var outcome = _player.RunRouteToPoint(route, enemyCheck, aggroCheck, distanceTreshold, mre, out target, goalWp, oppositeWay);
                if (outcome.Contains("dead"))
                {
                    StopApp();
                    break;
                }
                if (outcome.Contains("canceled"))
                {
                    break;
                }
                if (aggroCheck && outcome.Contains("aggro") && target != null)
                {
                    EngageHandler(target, route, escapeRoute, false, mre, onlyAggro);
                    continue;
                }
                if (enemyCheck && outcome.Contains("enemy") && target != null)
                {
                    EngageHandler(target, route, escapeRoute, true, mre, onlyAggro);
                    continue;
                }
                if (goalWp > -1) break;
            }
            Globals.Instance.GameLogger.Log("RunningHandler() complete.");
        }

        private void EngageHandler(Character target, Route route, Route escapeRoute, bool aggroCheck, ManualResetEvent mre, bool onlyAggro = false, bool runToTarget = true, bool gathering = false)
        {
            Globals.Instance.GameLogger.Log("EngageHandler() initiated. RunToPoint() aggroCheck: " + aggroCheck);
            if (runToTarget) Globals.Instance.KeySenderInstance.SendUp(Keys.W);

            while (!mre.WaitOne(0))
            {
                if (!aggroCheck && (target.MaxHP > _player.MaxHP + 2000)) //Level property does not work anymore
                {
                    Globals.Instance.GameLogger.Log("Aggro from too high-lvl enemy. Escaping...");
                    InitiateEscape(escapeRoute, true, mre);
                    break;
                }

                if (runToTarget)
                {
                    mre.WaitOne(300); //Pause before running to next enemy
                    Character aggressor;
                    var distanceTreshold = gathering ? GatherDistance : FightingDistance;
                    var outcome = _player.RunToTarget(target, aggroCheck, distanceTreshold, mre, out aggressor);
                    //If dead: StopApp(). If aggro: change target. If canceled/stuck: break. Else (successful), proceed with attacking enemy and subsequently searching for next
                    if (outcome == "dead")
                    {
                        StopApp();
                        break;
                    }
                    if (outcome == "canceled")
                    {
                        break;
                    }
                    if (aggroCheck && outcome == "stuck") //Don't move away if we are running to aggro and get stuck (aggroCheck is false).
                    {
                        Player.Turn180(mre);
                        goto TargetCheck; //Look for another enemy
                    }
                    if (aggroCheck && outcome == "aggro" && aggressor != null)
                    {
                        if (gathering) InitiateEscape(escapeRoute, true, mre);
                        else
                        {
                            target = aggressor;
                            aggroCheck = false;
                            continue;
                        }
                    }
                }

                //If we do not have aggro (aggroCheck true) and enemy is too far, skip it
                if (aggroCheck && _player.WaypointLocation.Distance(target.WaypointLocation) > MaxPullingDistance)
                {
                    Globals.Instance.GameLogger.Log("Target too far away, skipping");
                    goto TargetCheck;
                }

                mre.WaitOne(300); //Pause before attacking enemy
                var engageOutcome = gathering ? _player.Gather(mre) : _player.AttackTarget(target, mre, !aggroCheck); //If canceled: break. If escape: Initiate escape (run escape route, then continue running). Else (successful): search for next enemy. 
                if (engageOutcome == "dead")
                {
                    StopApp();
                    break;
                }
                if (engageOutcome == "canceled")
                {
                    break;
                }
                if (engageOutcome == "escape")
                {
                    InitiateEscape(escapeRoute, true, mre);
                    //Continue to immediately check for aggro and enemy
                }
                if (engageOutcome == "too far")
                {
                    //do nothing
                }
                if (engageOutcome == "success")
                {
                    ProcessXp();
                }

            TargetCheck:
                //When enemy is dead, always proceed to check for aggressor and then enemy (or target if gathering)
                target = _player.AggroCheck(mre);
                if (target != null)
                {
                    if (gathering) InitiateEscape(escapeRoute, true, mre);
                    else
                    {
                        Globals.Instance.GameLogger.Log("Aggressive enemy found. Pursuing it...");
                        aggroCheck = false;
                        continue;
                    }
                }

                if (!onlyAggro)
                {
                    //If no aggro, and too far from path, return to first point, then look for aggro and enemy (or target if gathering)
                    var closestWp = route.ClosestWaypoint(_player.WaypointLocation);
                    if (runToTarget && (_player.WaypointLocation.Distance(closestWp) > MaxDistanceFromPath) &&
                        !mre.WaitOne(0))
                    {
                        Globals.Instance.GameLogger.Log("Now too far from path, returning to closest waypoint.");
                        RunningHandler(route, escapeRoute, false, false, PointTurnDistance, mre,
                            route.Points.IndexOf(closestWp));
                        goto TargetCheck;
                    }

                    //Search for target
                    FarmingMre.WaitOne(Utils.getRandom(300, 500)); //Pause between AggroCheck and EnemyCheck
                    target = gathering ? _player.NpcCheck(mre) : _player.EnemyCheck(mre);
                    if (target != null)
                    {
                        Globals.Instance.GameLogger.Log("Non-aggressive target found. Pursuing it...");
                        aggroCheck = true;
                        continue;
                    }
                }

                //Target equals null here at bottom, so we did not find any enemies to pursue
                Globals.Instance.GameLogger.Log("No further targets found in EngageHandler().");
                break;
            }
            Globals.Instance.GameLogger.Log("EngageHandler() complete.");
        }

        private void InitiateEscape(Route escapeRoute, bool restart, ManualResetEvent mre)
        {
            //Traverses from closest waypoint, up to highest, then return from closest down to point 0
            Globals.Instance.GameLogger.Log("Escape initiated, restart: " + restart);
            RunningHandler(escapeRoute, escapeRoute, false, false, PointTurnDistance, mre, escapeRoute.Points.Count - 1);
            if (restart)
            {
                while ((_player.HP < _player.MaxHP * 0.9 || _player.MP < _player.MaxMP * 0.9))
                    mre.WaitOne(3000); //Wait before we continue running
                RunningHandler(escapeRoute, escapeRoute, false, false, PointTurnDistance, mre, 0, true);
            }
        }

        private void ProcessXp()
        {
            Task.Run(async () =>
            {
                await Task.Delay(3000);
                var postBattleXp = _player.GetXp();
                var xpReceived = postBattleXp - _preBattleXp;
                var uiSynch = MainForm.Get.UISynchContext;
                if (xpReceived > 0)
                {
                    Console.WriteLine(@"XP Received: " + xpReceived);
                    var maxExp = int.Parse(MainForm.Get.GetText("lbMaxExp"));
                    var minExp = int.Parse(MainForm.Get.GetText("lbMinExp"));
                    if (xpReceived > maxExp) uiSynch.Send(o => MainForm.Get.UpdateText("lbMaxExp", xpReceived.ToString(CultureInfo.InvariantCulture)), null);
                    if (xpReceived < minExp) uiSynch.Send(o => MainForm.Get.UpdateText("lbMinExp", xpReceived.ToString(CultureInfo.InvariantCulture)), null);

                    _totalXpEarned = _totalXpEarned + xpReceived;
                    var xpPerSecond = _totalXpEarned / MainForm.Get.FarmingSecondsPassed;
                    uiSynch.Send(o => MainForm.Get.UpdateText("lbXpPerSec", xpPerSecond.ToString(CultureInfo.InvariantCulture)), null);
                    _preBattleXp = postBattleXp; //Just to update the pre-battle number
                }
            });
        }

        public void StopApp()
        {
            Globals.Instance.GameLogger.Log("StopApp() initiated...");
            FarmingMre.Set();
            _runningMre.Set();
            MainForm.Get.Invoke(MainForm.Get.SEFDelegate); //Can't I call the delegate directly?
        }

        public void FinalActions(Route escapeRoute, ManualResetEvent mre)
        {
            InitiateEscape(escapeRoute, false, mre);
            mre.WaitOne(20000);
            Globals.Instance.KeySenderInstance.SendKey(Keys.Escape); //These two escapes are to ensure the cursor appears in the menue
            mre.WaitOne(1000);
            Globals.Instance.KeySenderInstance.SendKey(Keys.Escape);
            mre.WaitOne(1000);
            Globals.Instance.KeySenderInstance.SendKey(Keys.D9, KeySender.ModifierControl);
            mre.WaitOne(1000);
            Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad4);
            mre.WaitOne(1000);
            Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad0);
            mre.WaitOne(30000);
            Globals.Instance.KeySenderInstance.SendKey(Keys.Escape); //These two escapes are to ensure the cursor appears in the menue
            mre.WaitOne(1000);
            Globals.Instance.KeySenderInstance.SendKey(Keys.Escape);
            mre.WaitOne(1000);
            Globals.Instance.KeySenderInstance.SendKey(Keys.D0, KeySender.ModifierControl);
            mre.WaitOne(1000);
            Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad4);
            mre.WaitOne(1000);
            Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad0);
            mre.Set();
        }

        //private static void ReleaseAllButtons()
        //{
        //    Globals.Instance.GameLogger.Log("Releasing all buttons...");
        //    Globals.Instance.KeySenderInstance.SendUp(Keys.A);
        //    Globals.Instance.KeySenderInstance.SendUp(Keys.D);
        //    Globals.Instance.KeySenderInstance.SendUp(Keys.W);
        //    Globals.Instance.KeySenderInstance.SendUp(Keys.S);
        //}
    }
}