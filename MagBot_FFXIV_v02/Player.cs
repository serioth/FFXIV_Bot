using System;
using System.Threading;
using System.Windows.Forms;

namespace MagBot_FFXIV_v02
{
    class Player : Character
    {
        public Player(int baseOffset) : base(baseOffset)
        {
            PetInfo = new PetInfo();
        }

        private PetInfo PetInfo { get; set; }

        //OUTCOMES: canceled (break), dead (StopApp()), aggro (as appropriate), success (only if we define an end goal)
        public string RunRouteToPoint(Route route, bool enemycheck, bool aggroCheck, ManualResetEvent mre, out Character target, int goalWp = -1, bool oppositeWay = false)
        {
            //Start and stop of running must be done outside of this method
            string outcome = null;
            target = null;
            var wp = route.ClosestWaypoint(WaypointLocation);
            Globals.Instance.KeySenderInstance.SendDown(Keys.W);
            while (!mre.WaitOne(0))
            {
                Globals.Instance.ExpFarmingLogger.Log("RunToPoint(): Route " + RouteManager.Instance.Routes.IndexOf(route)  +". Point " + route.Points.IndexOf(wp));
                outcome = RunToPoint(() => wp, enemycheck, aggroCheck, false, out target, ExpFarming.PointTurnDistance, mre); //This lambda represents a method that just returns wp
                if (outcome == "canceled" || outcome == "dead" || outcome == "enemy" || outcome == "aggro") break;
                if (outcome == "success") if (goalWp > -1 && wp == route.Points[goalWp]) break;
                if (outcome == "stuck") //This will go forever, if it cannot get unstuck
                {
                    Turn180(mre);
                    continue;
                }

                wp = oppositeWay ? route.PreviousWaypoint(wp) : route.NextWaypoint(wp);
            }
            if (mre.WaitOne(0)) outcome = "canceled"; //Cancel can happen after RunToPoint(), in which case this fixes outcome
            Globals.Instance.KeySenderInstance.SendUp(Keys.W);
            return outcome;
        }

        //USAGE: Running to enemy, aggressor or point
        //OUTCOMES: canceled, dead, aggro, stuck, success. Always release all keys at highest level
        //canceled: Can either be by user or by program. In any case, just return
        //dead: We want to end the program. Return outcome up call tree and at highest level call StopApp()
        //aggro: Propagate and handle
        //stuck: We cannot do anything else here. Return outcome up one level and decide how to deal with it
            //If pursuing aggressor: wait and attack
            //If pursuing enemy: return outcome up call tree and proceed to look for next enemy
            //If pursuing point: return outcome up one level, Turn180() and select next point
        //success: return outcome up one level and proceed as appropriate
        private string RunToPoint(Func<Waypoint> wp, bool enemyCheck, bool aggroCheck, bool stopAtEnd, out Character target, int distanceTreshold, ManualResetEvent mre)
        {
            //Start and stop of running must be done outside of this method
            var outcome = "success";
            target = null;
            if (stopAtEnd) Globals.Instance.KeySenderInstance.SendDown(Keys.W);
            while ((wp().Distance(WaypointLocation) > distanceTreshold))
            {
                //Slowing down of loop is done in JumpIfStuck()
                Face(wp(), distanceTreshold, mre);

                if (mre.WaitOne(0))
                {
                    outcome = "canceled";
                    break;
                }
                if (HP == 0)
                {
                    outcome = "dead";
                    break;
                }
                if (aggroCheck)
                {
                    target = AggroCheck(mre);
                    if (target != null)
                    {
                        outcome = "aggro";
                        break;
                    }
                }
                if (enemyCheck)
                {
                    target = EnemyCheck(mre);
                    if (target != null)
                    {
                        outcome = "enemy";
                        break;
                    }
                }
                if (JumpIfStuck(wp(), mre) == "stuck")
                {
                    outcome = "stuck";
                    break;
                }
            }
            if (stopAtEnd) Globals.Instance.KeySenderInstance.SendUp(Keys.W);
            Globals.Instance.ExpFarmingLogger.Log("RunToPoint() completed. Outcome: " + outcome);
            return outcome;
        }

        public string RunToEnemy(Character target, bool aggroCheck, ManualResetEvent mre, out Character aggressor)
        {
            //The lambda represents a method that runs and returns target.WaypointLocation
            Globals.Instance.ExpFarmingLogger.Log("RunToPoint(): Enemy.");
            return RunToPoint(() => target.WaypointLocation, false, aggroCheck, true, out aggressor, ExpFarming.FightingDistance, mre);
        }

        private void Face(Waypoint pt, int distanceTreshold, ManualResetEvent mre)
        {
            var angleDiff = FacingAngle - pt.Angle(WaypointLocation);
            var absAngleDiff = Math.Abs(angleDiff);

            //If the absolute value of diff is greater than treshold, we should turn
            //If the diff is greater than 180, then a negative diff means we turn right
            //If on the other hand the diff is smaller than 180, a negative diff means we turn left
            if (absAngleDiff > 0.15)
            {
                if (absAngleDiff >= Math.PI)
                {
                    Globals.Instance.KeySenderInstance.SendDown(angleDiff < 0 ? Keys.D : Keys.A);
                }
                else
                {
                    Globals.Instance.KeySenderInstance.SendDown(angleDiff < 0 ? Keys.A : Keys.D);
                }
            }

            //0.15 radians. 0.2 to ensure it works on slow pc?
            while (!IsFacing(pt) && (pt.Distance(WaypointLocation) > distanceTreshold))
            {
                if (mre.WaitOne(10)) break;
            }

            Globals.Instance.KeySenderInstance.SendUp(Keys.A);
            Globals.Instance.KeySenderInstance.SendUp(Keys.D);
        }

        private bool IsFacing(Waypoint pt)
        {
            var absAngleDiff = Math.Abs(FacingAngle - pt.Angle(WaypointLocation));
            return !(absAngleDiff > 0.15);
        }

        //This can potentially delay the stopping of the program by the total of the two sleeps
        private string JumpIfStuck(Waypoint target, ManualResetEvent mre)
        {
            var oldDistance = WaypointLocation.Distance(target);
            mre.WaitOne(500);
            if (WaypointLocation.Distance(target) < oldDistance) return null;

            for (var jumpCount = 0; jumpCount < 3; jumpCount++)
            {
                Globals.Instance.ExpFarmingLogger.Log("Stuck, jumping...");
                Globals.Instance.KeySenderInstance.SendKey(Keys.Space);
                if (mre.WaitOne(500)) return "canceled";
                if (WaypointLocation.Distance(target) < oldDistance) return "jump ok";
            }
            return "stuck";
        }

        //OUTCOMES: success, canceled, escape
        public string AttackTarget(Character target, ManualResetEvent mre, bool aggressor)
        {
            Globals.Instance.ExpFarmingLogger.Log("AttackTarget() initiated. Target: " + target.Name + ", Level: " + target.Level);
            int targetLevelOneAddress = Globals.Instance.MemoryBaseOffsetDictionary["Target1"];
            Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad0); //Engage enemy
            if (!aggressor) PreBattleAndPull(mre);

            while (target.HP > 0)
            {
                if (mre.WaitOne(0))
                {
                    Globals.Instance.ExpFarmingLogger.Log("AttackTarget() canceled.");
                    return "canceled";
                }

                if (SummonerAttackPattern(target, mre) == "escape")
                {
                    Globals.Instance.ExpFarmingLogger.Log("Escape initiated in AttackTarget().");
                    return "escape";
                }

                if ((WaypointLocation.Distance(target.WaypointLocation) > ExpFarming.MaxPullingDistance) || !HasTarget(targetLevelOneAddress))
                {
                    Globals.Instance.ExpFarmingLogger.Log("Enemy escaped/lost, returning success");
                    break;
                }
                if (HP == 0)
                {
                    Globals.Instance.ExpFarmingLogger.Log("Died in AttackTarget().");
                    return "dead";
                }
            }
            Globals.Instance.ExpFarmingLogger.Log("AttackTarget() completed, enemy dead or escaped.");
            return "success";
        }

        private void WaitForRegen(ManualResetEvent mre)
        {
            if (MP < MaxMP * 0.6) Globals.Instance.ExpFarmingLogger.Log("MP < 60%. Waiting for MP to restore...");
            while ((MP < MaxMP * 0.6))
            {
                if (mre.WaitOne(0)) return;
                mre.WaitOne(300);
            }
        }

        private string RestoreHP(ManualResetEvent mre)
        {
            while (HP < (MaxHP * 0.8))
            {
                if (mre.WaitOne(0)) return "canceled";
                if ((MP < (MaxMP * 0.3)) || HP < MaxHP * 0.5)
                {
                    return "escape";
                }
                UseSkill(Globals.Instance.SkillDictionary["Physick"], mre);
            }
            return "OK";
        }

        public Character AggroCheck(ManualResetEvent mre)
        {
            //The out escape parameter is so we can escape if aggro from high-lvl enemy
            //If the aggressor has too high level, attack it nevertheless, then escape
            Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad8, KeySender.ModifierControl);
            mre.WaitOne(200);
            var target = GetTarget(Globals.Instance.MemoryBaseOffsetDictionary["Target2"]);
            if (target != null) Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad0);

            return target;
        }

        public Character EnemyCheck(ManualResetEvent mre)
        {
            Globals.Instance.KeySenderInstance.SendKey(Keys.Tab);
            mre.WaitOne(200);
            var target = GetTarget(Globals.Instance.MemoryBaseOffsetDictionary["Target1"]);
            if (target == null) return null;

            if (target.Level > Level + ExpFarming.MaxEnemyLevelDiff)
            {
                Globals.Instance.ExpFarmingLogger.Log(target.Name + "'s level (" + target.Level + ") too high, skipping...");
                mre.WaitOne(300);
                Globals.Instance.KeySenderInstance.SendKey(Keys.Escape);
                return null;
            }
            return target;
        }

        private static Character GetTarget(int targetBaseOffset)
        {
            //This checks whether or not we are actually targeting something
            if(!HasTarget(targetBaseOffset)) return null;

            //Every time we create a Character instance the pointers are recreated (see constructor and LoadPointers())
            var target = new Character(targetBaseOffset);
            return target;
        }

        private static bool HasTarget(int targetBaseOffset)
        {
            //This checks whether or not we are actually targeting something
            int lvlOneValue;
            IntPtr targetPointer = MemoryHandler.Instance.GetLvlOneAddressFromBaseOffset(targetBaseOffset);
            MemoryHandler.Instance.ReadInt(targetPointer, out lvlOneValue);
            return lvlOneValue > 0;
        }

        private void UseSkill(Skill skill, ManualResetEvent mre)
        {
            if (skill.Ready != true || !(MP > skill.MPCost)) return;
            if (mre.WaitOne(0)) return;

            Globals.Instance.ExpFarmingLogger.Log("Using skill: " + skill.Name);

            Globals.Instance.KeySenderInstance.SendKey(Globals.Instance.KeySenderInstance.ToKey(skill.Button));

            skill.Timer.Start();
            skill.Ready = false;

            //After initiating the skill we need to sleep until it is finished casting
            //Sleep amount = random number from the time it takes to cast up to a second longer
            mre.WaitOne(Utils.getRandom(skill.Cast + 500, skill.Cast + 800));
        }

        private void PreBattleAndPull(ManualResetEvent mre)
        {
            if (PetInfo.PetHP < 250) UseSkill(Globals.Instance.SkillDictionary["SummonII"], mre);
            Globals.Instance.KeySenderInstance.SendKey(Keys.D2, KeySender.ModifierControl); //Pet Obey+Curl Macro
            mre.WaitOne(Utils.getRandom(3000,4000));
        }

        private string SummonerAttackPattern(Character target, ManualResetEvent mre)
        {
            if (RestoreHP(mre) == "escape") return "escape";
            if (PetInfo.PetHP < 250)
            {
                UseSkill(Globals.Instance.SkillDictionary["SummonII"], mre);
                Globals.Instance.KeySenderInstance.SendKey(Keys.D2, KeySender.ModifierControl); //Pet Sic Macro
            }

            if (RestoreHP(mre) == "escape") return "escape";
            UseSkill(Globals.Instance.SkillDictionary["Sustain"], mre);
            if (target.HP <= 0) return "success";

            if (RestoreHP(mre) == "escape") return "escape";
            UseSkill(Globals.Instance.SkillDictionary["Aetherflow"], mre);
            UseSkill(Globals.Instance.SkillDictionary["EnergyDrain"], mre);
            UseSkill(Globals.Instance.SkillDictionary["Virus"], mre);
            if (target.HP <= 0) return "success";

            if (RestoreHP(mre) == "escape") return "escape";
            UseSkill(Globals.Instance.SkillDictionary["Miasma"], mre);
            if (target.HP <= 0) return "success";

            if (RestoreHP(mre) == "escape") return "escape";
            UseSkill(Globals.Instance.SkillDictionary["Bio"], mre);
            if (target.HP <= 0) return "success";

            if (RestoreHP(mre) == "escape") return "escape";
            UseSkill(Globals.Instance.SkillDictionary["BioII"], mre);
            if (target.HP <= 0) return "success";

            if (RestoreHP(mre) == "escape") return "escape";
            UseSkill(Globals.Instance.SkillDictionary["Ruin"], mre);
            
            return "pattern complete"; //No need to check if enemy is dead here, that's done in loop
        }

        //If we cancel (mre is set), then no blocking occurs and we just fall through
        private static void Turn180(ManualResetEvent mre)
        {
            Globals.Instance.ExpFarmingLogger.Log("Turn180() initiated...");
            if (!mre.WaitOne(0))
            {
                Globals.Instance.KeySenderInstance.SendDown(Keys.A);
                mre.WaitOne(800); //Turn 90 degrees
                Globals.Instance.KeySenderInstance.SendUp(Keys.A);
                mre.WaitOne(1500); //Run sideways for a bit
            }
        }
    }

    class PetInfo
    {
        private readonly IntPtr _petHP;
        private readonly IntPtr _maxPetHP;
        private int _output;

        public PetInfo()
        {
            _petHP =
                MemoryHandler.Instance.GetPointerFromOffsets(new[]
                {Globals.Instance.MemoryBaseOffsetDictionary["Pet"]});
            _maxPetHP =  IntPtr.Add(_petHP, 4);
        }

        public int PetHP
        {
            get
            {
                return MemoryHandler.Instance.ReadInt(_petHP, out _output) == MemoryHandler.MemoryResult.Success ? _output : 0;
            }
        }

        public int MaxPetHP
        {
            get
            {
                return MemoryHandler.Instance.ReadInt(_maxPetHP, out _output) == MemoryHandler.MemoryResult.Success ? _output : 0;
            }
        }
    }
}
