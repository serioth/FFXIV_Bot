using System.Threading;
using System.Windows.Forms;

namespace MagBot_FFXIV_v02
{
    internal class ExpFarming
    {
        private readonly Player _player;
        public readonly ManualResetEvent Cancel;

        //CONSTANTS
        public const int MaxPullingDistance = 25; //Max distance one can pull from (magic or archery reach)
        private const int MaxDistanceFromPath = 50;
        public const int MaxEnemyLevelDiff = 4;
        public const int FightingDistance = 15;
        public const int PointTurnDistance = 4;

        public ExpFarming(Player player)
        {
            Globals.Instance.ExpFarmingLogger = new Logger("ExpFarmingLog");
            _player = player;
            Cancel = new ManualResetEvent(false);

            //Ensure NUMLOCK is on (has to use SendInput)
            if (!MemoryHandler.IsKeyLocked(MemoryApi.KeyCode.NUMLOCK))
            {
                Globals.Instance.ExpFarmingLogger.Log("Turning NUMLOCK on...");
                MemoryHandler.HumanKeyPress(MemoryApi.KeyCode.NUMLOCK);
            }
        }

        public void StartStandStillExpFarming(Route escapeRoute)
        {
            Globals.Instance.ExpFarmingLogger.Log("=====!!!!===== Initiating Stand-Still Exp Farming =====!!!!=====");
            Cancel.Reset();
            while (!Cancel.WaitOne(0))
            {
                Cancel.WaitOne(Utils.getRandom(300, 500)); //Pause between AggroCheck and EnemyCheck
                var target = _player.AggroCheck(Cancel);
                if (target != null)
                {
                    Globals.Instance.ExpFarmingLogger.Log("Aggressive enemy found. Attacking it...");
                    AttackHandler(target, escapeRoute, escapeRoute, false, Cancel, false, false);
                    continue;
                }
                Cancel.WaitOne(Utils.getRandom(300, 500)); //Pause between AggroCheck and EnemyCheck
                target = _player.EnemyCheck(Cancel);
                if (target != null)
                {
                    if (_player.WaypointLocation.Distance(target.WaypointLocation) > MaxPullingDistance) continue;
                    Globals.Instance.ExpFarmingLogger.Log("Non-aggressive enemy found. Attacking it...");
                    AttackHandler(target, escapeRoute, escapeRoute, true, Cancel, false, false);
                    continue;
                }
                Globals.Instance.ExpFarmingLogger.Log("StandStillExpFarming() found no enemies, proceeding to check again...");
            }
            Globals.Instance.ExpFarmingLogger.Log("StandStillExpFarming() complete.");
        }

        public void StartExpFarming(Route route, Route escapeRoute)
        {
            Cancel.Reset();
            Globals.Instance.ExpFarmingLogger.Log("=====!!!!===== Initiating Exp Farming =====!!!!=====");
            while ((_player.HP < _player.MaxHP * 0.9 || _player.MP < _player.MaxMP * 0.9)) Cancel.WaitOne(1000); //Heal up before start

            Globals.Instance.ExpFarmingLogger.Log("Running to first waypoint...");
            var closestWaypointIndex = route.Points.IndexOf(route.ClosestWaypoint(_player.WaypointLocation));
            //RunningHandler(route, escapeRoute, false, true, Cancel, closestWaypointIndex, true); //Don't start farming until at path, so run to closest point before starting
            //RunningHandler(route, escapeRoute, true, true, Cancel);

            RunningHandler(route, escapeRoute, false, false, Cancel, -1, true);
        }

        private void RunningHandler(Route route, Route escapeRoute, bool enemyCheck, bool aggroCheck, ManualResetEvent mre, int goalWp = -1, bool oppositeWay = false)
        {
            Globals.Instance.ExpFarmingLogger.Log("RunningHandler() initiated...");
            var onlyAggro = goalWp > -1;
            while (!mre.WaitOne(0))
            {
                Character target;
                mre.WaitOne(500); //Wait before we start running route again
                var outcome = _player.RunRouteToPoint(route, enemyCheck, aggroCheck, mre, out target, goalWp, oppositeWay);
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
                    AttackHandler(target, route, escapeRoute, false, mre, onlyAggro);
                    continue;
                }
                if (enemyCheck && outcome.Contains("enemy") && target != null)
                {
                    AttackHandler(target, route, escapeRoute, true, mre, onlyAggro);
                    continue;
                }
                if (goalWp > -1) break;
            }
            Globals.Instance.ExpFarmingLogger.Log("RunningHandler() complete.");
        }

        private void AttackHandler(Character target, Route route, Route escapeRoute, bool aggroCheck, ManualResetEvent mre, bool onlyAggro = false, bool runToEnemy = true)
        {
            Globals.Instance.ExpFarmingLogger.Log("AttackHandler() initiated. RunToPoint() aggroCheck: " + aggroCheck);
            if (runToEnemy) Globals.Instance.KeySenderInstance.SendUp(Keys.W);

            while (!mre.WaitOne(0))
            {
                if (!aggroCheck && (target.Level > _player.Level + MaxEnemyLevelDiff))
                {
                    Globals.Instance.ExpFarmingLogger.Log("Aggro from too high-lvl enemy. Escaping...");
                    InitiateEscape(escapeRoute, true, mre);
                    break;
                }

                if (runToEnemy)
                {
                    Cancel.WaitOne(300); //Pause before running to next enemy
                    Character aggressor;
                    var outcome = _player.RunToEnemy(target, aggroCheck, mre, out aggressor);
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
                        RunningHandler(route, escapeRoute, false, false, mre,
                            route.Points.IndexOf(route.ClosestWaypoint(_player.WaypointLocation)) + 1);
                        goto EnemyCheck;
                    }
                    if (aggroCheck && outcome == "aggro" && aggressor != null)
                    {
                        target = aggressor;
                        aggroCheck = false;
                        continue;
                    }
                }

                //If we do not have aggro (aggroCheck true) and enemy is too far, skip it
                if (aggroCheck && _player.WaypointLocation.Distance(target.WaypointLocation) > MaxPullingDistance)
                {
                    Globals.Instance.ExpFarmingLogger.Log("Target too far away, skipping");
                    goto EnemyCheck;
                }

                mre.WaitOne(300); //Pause before attacking enemy
                var attackOutcome = _player.AttackTarget(target, mre, !aggroCheck); //If canceled: break. If escape: Initiate escape (run escape route, then continue running). Else (successful): search for next enemy. 
                if (attackOutcome == "dead")
                {
                    StopApp();
                    break;
                }
                if (attackOutcome == "canceled")
                {
                    break;
                }
                if (attackOutcome == "escape")
                {
                    InitiateEscape(escapeRoute, true, mre);
                    //Continue to immediately check for aggro and enemy
                }
                if (attackOutcome == "too far")
                {
                    //do nothing
                }

                EnemyCheck:
                //When enemy is dead, always proceed to check for aggressor and then enemy
                target = _player.AggroCheck(mre);
                if (target != null)
                {
                    Globals.Instance.ExpFarmingLogger.Log("Aggressive enemy found. Pursuing it...");
                    aggroCheck = false;
                    continue;
                }

                if (!onlyAggro)
                {
                    //If no aggro, and too far from path, return to first point, then look for aggro and enemy
                    var closestWp = route.ClosestWaypoint(_player.WaypointLocation);
                    if (runToEnemy && (_player.WaypointLocation.Distance(closestWp) > MaxDistanceFromPath) && !mre.WaitOne(0))
                    {
                        Globals.Instance.ExpFarmingLogger.Log("Now too far from path, returning to closest waypoint.");
                        RunningHandler(route, escapeRoute, false, false, mre, route.Points.IndexOf(closestWp));
                        goto EnemyCheck;
                    }

                    //Search for enemy
                    Cancel.WaitOne(Utils.getRandom(300, 500)); //Pause between AggroCheck and EnemyCheck
                    target = _player.EnemyCheck(mre);
                    if (target != null)
                    {
                        Globals.Instance.ExpFarmingLogger.Log("Non-aggressive enemy found. Pursuing it...");
                        aggroCheck = true;
                        continue;
                    }
                }

                //Target equals null here at bottom, so we did not find any enemies to pursue
                Globals.Instance.ExpFarmingLogger.Log("No further enemies found in AttackHandler().");
                break;
            }
            Globals.Instance.ExpFarmingLogger.Log("AttackHandler() complete.");
        }

        public void InitiateEscape(Route escapeRoute, bool restart, ManualResetEvent mre)
        {
            //Traverses from closest waypoint, up to highest, then return from closest down to point 0
            Globals.Instance.ExpFarmingLogger.Log("Escape initiated, restart: " + restart);
            RunningHandler(escapeRoute, escapeRoute, false, false, mre, escapeRoute.Points.Count - 1);
            if (restart)
            {
                while ((_player.HP < _player.MaxHP*0.9 || _player.MP < _player.MaxMP*0.9))
                    mre.WaitOne(3000); //Wait before we continue running
                RunningHandler(escapeRoute, escapeRoute, false, false, mre, 0, true);
            }
        }

        public void StopApp()
        {
            Globals.Instance.ExpFarmingLogger.Log("StopApp() initiated...");
            Cancel.Set();
            MainForm.Get.Invoke(MainForm.Get.SEFDelegate); //Can't I call the delegate directly?
        }

        private static void ReleaseAllButtons()
        {
            Globals.Instance.ExpFarmingLogger.Log("Releasing all buttons...");
            Globals.Instance.KeySenderInstance.SendUp(Keys.A);
            Globals.Instance.KeySenderInstance.SendUp(Keys.D);
            Globals.Instance.KeySenderInstance.SendUp(Keys.W);
            Globals.Instance.KeySenderInstance.SendUp(Keys.S);
        }
    }
}