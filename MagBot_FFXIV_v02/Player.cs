using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        public string RunRouteToPoint(Route route, bool enemycheck, bool aggroCheck, int distanceTreshold, FarmingHandler.FarmingType farmingType, ManualResetEvent mre, out Character target, int goalWp = -1, bool oppositeWay = false)
        {
            //Start and stop of running must be done outside of this method
            string outcome = null;
            target = null;
            var wp = route.ClosestWaypoint(WaypointLocation);
            Globals.Instance.KeySenderInstance.SendDown(Keys.W);
            while (!mre.WaitOne(0))
            {
                Globals.Instance.GameLogger.Log("RunToPoint(): " + route.Name  +". Point " + route.Points.IndexOf(wp));
                outcome = RunToPoint(() => wp, enemycheck, aggroCheck, false, farmingType, out target, distanceTreshold, mre); //This lambda represents a method that just returns wp
                if (outcome == "canceled" || outcome == "dead" || outcome == "target" || outcome == "aggro") break;
                if (outcome == "success") if (goalWp > -1 && wp == route.Points[goalWp]) break;
                if (outcome == "stuck") //This will go forever, if it cannot get unstuck. It runs to same wp. Should we perhaps pick a new point?
                {
                    Turn180(mre);
                    continue;
                }

                wp = oppositeWay ? route.PreviousWaypoint(wp) : route.NextWaypoint(wp);
            }
            if (mre.WaitOne(0)) outcome = "canceled"; //FarmingMre can happen after RunToPoint(), in which case this fixes outcome
            Globals.Instance.KeySenderInstance.SendUp(Keys.W);
            return outcome;
        }

        //USAGE: Running to enemy, aggressor or point
        //OUTCOMES: canceled, dead, aggro, stuck, success. Always release all keys at highest level
        //canceled: Can either be by user or by program. In any case, just return
        //dead: We want to end the program. Return outcome up call tree and at highest level call StopApp()
        //aggro: Propagate and handle
        //stuck: We cannot do anything else here. Return outcome up one level and decide how to deal with it ---> NEW: Let's just do Turn180() on all of them!
            //If pursuing aggressor: wait and attack
            //If pursuing enemy: return outcome up call tree and proceed to look for next enemy
            //If pursuing point: return outcome up one level, Turn180() and select next point
        //success: return outcome up one level and proceed as appropriate
        private string RunToPoint(Func<Waypoint> wp, bool targetCheck, bool aggroCheck, bool stopAtEnd, bool gathering, out Character target, int distanceTreshold, ManualResetEvent mre)
        {
            //Start and stop of running must be done outside of this method
            var outcome = "success";
            target = null;
            if (stopAtEnd) Globals.Instance.KeySenderInstance.SendDown(Keys.W);

            //Stopwatch is for jumping logic
            var sw = new Stopwatch();
            sw.Start();
            var previousDistance = wp().Distance(WaypointLocation);
            var previousTime=new TimeSpan(0,0,0); //hr, min, sec

            while ((WaypointLocation.Distance(wp()) > distanceTreshold))
            {
                //Slowing down of loop is done in JumpIfStuck()
                Face(wp(), distanceTreshold, mre);

                if (!aggroCheck && !targetCheck) mre.WaitOne(200); // Just to slow it down (aggroCheck and targetCheck normally slows it down)

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
                if (targetCheck)
                {
                    target = gathering ? NpcCheck(mre) : EnemyCheck(mre);
                    if (target != null)
                    {
                        outcome = "target";
                        break;
                    }
                }
                if (sw.Elapsed >= previousTime.Add(new TimeSpan(0, 0, 1)))
                {
                    if (JumpIfStuck(ref previousTime, ref previousDistance, wp(), mre) != "stuck") continue;
                    outcome = "stuck";
                    break;
                }
            }
            sw.Stop();
            if (stopAtEnd &&  outcome != "stuck") Globals.Instance.KeySenderInstance.SendUp(Keys.W);
            Globals.Instance.GameLogger.Log("RunToPoint() completed. Outcome: " + outcome);
            return outcome;
        }

        public string RunToTarget(Character target, bool aggroCheck, int distanceTreshold, bool gathering, ManualResetEvent mre, out Character aggressor)
        {
            //The lambda represents a method that runs and returns target.WaypointLocation
            Globals.Instance.GameLogger.Log("RunToTarget(): " + target.Name);
            return RunToPoint(() => target.WaypointLocation, false, aggroCheck, true, gathering, out aggressor, distanceTreshold, mre);
        }

        private void Face(Waypoint pt, double distanceTreshold, ManualResetEvent mre)
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

        //This can delays enemy search
        private string JumpIfStuck(ref TimeSpan previousTime, ref double previousDistance, Waypoint target, WaitHandle mre)
        {
            //var oldDistance = WaypointLocation.Distance(target);
            //mre.WaitOne(500);
            //if (WaypointLocation.Distance(target) < oldDistance) return null;

            //for (var jumpCount = 0; jumpCount < 3; jumpCount++)
            //{
            //    Globals.Instance.GameLogger.Log("Stuck, jumping...");
            //    Globals.Instance.KeySenderInstance.SendKey(Keys.Space);
            //    if (mre.WaitOne(500)) return "canceled";
            //    if (WaypointLocation.Distance(target) < oldDistance) return "jump ok";
            //}
            //return "stuck";
            
            //Jumping logic
            //If stopwatch is one second more than it was before, check against previous distance, set new distance and set stopWatch
            previousTime = previousTime.Add(new TimeSpan(0, 0, 1));
            if (WaypointLocation.Distance(target) < previousDistance)
            {
                previousDistance = WaypointLocation.Distance(target);
                return "ok";
            }
            for (var jumpCount = 0; jumpCount < 3; jumpCount++)
            {
                Globals.Instance.GameLogger.Log("Stuck, jumping...");
                Globals.Instance.KeySenderInstance.SendKey(Keys.Space);
                if (mre.WaitOne(1000)) return "canceled";
                if (WaypointLocation.Distance(target) < previousDistance*0.90) return "jump ok";
            }
            return "stuck";
        }

        public string AttackTarget(Character target, ManualResetEvent mre)
        {
            Globals.Instance.GameLogger.Log("AttackTarget() initiated. Target: " + target.Name + ", Level: " + target.Level);

            var targetLevelOneAddress = Globals.Instance.MemoryBaseOffsetDictionary["Target1"];
            Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad0); //Engage enemy
            if (!HasAggressor()) PreBattleAndPull(mre);

            while (target.HP > 0)
            {
                if (mre.WaitOne(0))
                {
                    Globals.Instance.GameLogger.Log("AttackTarget() canceled.");
                    return "canceled";
                }

                if (SummonerAttackPattern(target, mre) == "escape")
                {
                    Globals.Instance.GameLogger.Log("Escape initiated in AttackTarget().");
                    return "escape";
                }

                if ((WaypointLocation.Distance(target.WaypointLocation) > FarmingHandler.MaxPullingDistance) || !HasTarget(targetLevelOneAddress))
                {
                    Globals.Instance.GameLogger.Log("Enemy escaped/lost, returning success");
                    break;
                }
                if (HP == 0)
                {
                    Globals.Instance.GameLogger.Log("Died in AttackTarget().");
                    return "dead";
                }
            }
            Globals.Instance.GameLogger.Log("AttackTarget() completed, enemy dead or escaped.");
            return "success";
        }

        public string Gather(ManualResetEvent mre)
        {
            Globals.Instance.GameLogger.Log("Gather() initiated...");
            PreGather(mre);

            var count = 0;
            while (HasTarget(Globals.Instance.MemoryBaseOffsetDictionary["Target1"]))
            {
                if (mre.WaitOne(0))
                {
                    Globals.Instance.GameLogger.Log("Gather() canceled.");
                    //TODO: Add a check to see if widget is up
                    Thread.Sleep(4000); //If we stop while in here, we actually HAVE to wait until the current gathering attempt is done
                    Globals.Instance.KeySenderInstance.SendKey(Keys.Escape); //Need to escape out of the gathering menu before we can start running (in case of end-of-farming)
                    return "canceled";
                }

                if (HpLow(0.8))
                {
                    Globals.Instance.GameLogger.Log("Escape in Gather().");
                    Thread.Sleep(4000); //Todo: Same as above
                    Globals.Instance.KeySenderInstance.SendKey(Keys.Escape); //Need to escape out of the gathering menu before we can move
                    return "escape";
                }

                if (HP == 0)
                {
                    Globals.Instance.GameLogger.Log("Died in Gather().");
                    return "dead";
                }

                //Engage
                Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad0); //If we do not have a npc selected, this selects closest enemy or npc
                mre.WaitOne(Utils.getRandom(2600, 3000));
                count++;

                if (count > 7)
                {
                    //For some reason we have not been able to reach target, so lock, move forward and circle around target and try again
                    Globals.Instance.GameLogger.Log("Could not reach target, attempting to reposition.");
                    Reposition(mre);
                    PreGather(mre);
                    count = 0;
                }
            }

            //Todo: This should check for stealth status and only take it off if we have it on. Until then, don't use it
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                //UseSkill(Globals.Instance.SkillDictionary["Stealth"], mre, false, true);
                UseSkill(Globals.Instance.SkillDictionary["Run"], mre);
            });


            return "success";
        }

        private bool HpLow(double tresholdPercent)
        {
            return HP < MaxHP*tresholdPercent;
        }

        private bool MpLow(double tresholdPercent)
        {
            return MP < MaxMP * tresholdPercent;
        }

        private void PreGather(ManualResetEvent mre)
        {
            UseSkill(Globals.Instance.SkillDictionary["XpFood"], mre, true); //TODO: Instead check if it is on, and cast if not
            //UseSkill(Globals.Instance.SkillDictionary["Stealth"], mre, false, true);
            Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad0);
            mre.WaitOne(1500);

            //SPELLS
            //It will always use only the lower GP cost spell, because we do not regenerate GP fast enough
            //So if we are leveling, choose FieldMaster. If we are farming for items, choose other ones (maybe randomize)
            //LeafTurn increases HQ, which increases XP earned though. So keep that in mind when leveling
            //UseSkill(Globals.Instance.SkillDictionary["FieldMastery"], mre, false, true);
            UseSkill(Globals.Instance.SkillDictionary["LeafTurn"], mre, false, true);
            Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad0);
            mre.WaitOne(Utils.getRandom(2600, 3000));
        }

        private void WaitForRegen(ManualResetEvent mre)
        {
            if (MP < MaxMP * 0.6) Globals.Instance.GameLogger.Log("MP < 60%. Waiting for MP to restore...");
            while ((MP < MaxMP * 0.6))
            {
                if (mre.WaitOne(0)) return;
                mre.WaitOne(300);
            }
        }

        public int GetXp()
        {
            var xpPointer = MemoryHandler.Instance.GetPointerFromOffsets(new[] { Globals.Instance.MemoryBaseOffsetDictionary["XP"] });
            int xp;
            MemoryHandler.Instance.ReadInt(xpPointer, out xp);
            return xp;
        }

        private string RestoreHP(ManualResetEvent mre)
        {
            while (HpLow(0.8))
            {
                if (mre.WaitOne(0)) return "canceled";
                if (MpLow(0.3) || HpLow(0.5))
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

            if (target.HP > HP + 2000) //Level property does not work anymore
            {
                Globals.Instance.GameLogger.Log(target.Name + "'s level (" + target.Level + ") too high, skipping...");
                mre.WaitOne(300);
                Globals.Instance.KeySenderInstance.SendKey(Keys.Escape);
                return null;
            }

            if (!EnemyValid(target))
            {
                Globals.Instance.GameLogger.Log("Invalid enemy. Skipping...");
                mre.WaitOne(300);
                Globals.Instance.KeySenderInstance.SendKey(Keys.Escape);
                return null;
            }
            return target;
        }

        public Character NpcCheck(ManualResetEvent mre)
        {
            Globals.Instance.KeySenderInstance.SendKey(Keys.F12);
            mre.WaitOne(200);
            var target = GetTarget(Globals.Instance.MemoryBaseOffsetDictionary["Target1"]);
            if (target == null) return null;

            if (!EnemyValid(target))
            {
                Globals.Instance.GameLogger.Log("Invalid target. Skipping...");
                return null;
            }
            return target;
        }

        private bool EnemyValid(Character target)
        {
            var enemyOk = FarmingHandler.TargetList.Length == 0 || FarmingHandler.TargetList.Any(enemy => target.Name.Contains(enemy));
            return enemyOk;
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
            var targetPointer = MemoryHandler.Instance.GetLvlOneAddressFromBaseOffset(targetBaseOffset);
            MemoryHandler.Instance.ReadInt(targetPointer, out lvlOneValue);
            return lvlOneValue > 0;
        }

        public static bool HasAggressor()
        {
            //This checks whether or not we are actually targeting something
            return HasTarget(Globals.Instance.MemoryBaseOffsetDictionary["Target2"]);
        }

        public void UseSkill(Skill skill, ManualResetEvent mre, bool ctrl = false, bool gp = false)
        {
            var myAmount = gp ? GP : MP;
            if (skill.Cost != 0 && myAmount < skill.Cost) return;
            if (skill.Ready != true) return;
            if (mre.WaitOne(0)) return;

            Globals.Instance.GameLogger.Log("Using skill: " + skill.Name + ". Button: " + skill.Button);

            if (ctrl)
            {
                Globals.Instance.KeySenderInstance.SendKey(Globals.Instance.KeySenderInstance.ToKey(skill.Button), KeySender.ModifierControl);
            }
            else
            {
                Globals.Instance.KeySenderInstance.SendKey(Globals.Instance.KeySenderInstance.ToKey(skill.Button));
            }

            skill.Timer.Start();
            skill.Ready = false;

            //After initiating the skill we need to sleep until it is finished casting
            //Sleep amount = random number from the time it takes to cast up to a second longer
            //200 and 500 worked, trying lower
            mre.WaitOne(Utils.getRandom(skill.Cast + 0, skill.Cast + 300));
        }

        private void PreBattleAndPull(ManualResetEvent mre)
        {
            UseSkill(Globals.Instance.SkillDictionary["XpFood"], mre, true); //TODO: Instead check if it is on, and cast if not
            if (PetInfo.PetHP < 250) UseSkill(Globals.Instance.SkillDictionary["SummonII"], mre);
            UseSkill(Globals.Instance.SkillDictionary["Curl"], mre, true); //Curl for 20sec
            mre.WaitOne(Utils.getRandom(100, 200));
            Globals.Instance.KeySenderInstance.SendKey(Keys.D1, KeySender.ModifierControl); //Pet Obey on target
            mre.WaitOne(Utils.getRandom(3000,4000));
        }

        private string SummonerAttackPattern(Character target, ManualResetEvent mre)
        {
            if (RestoreHP(mre) == "escape") return "escape";
            if (PetInfo.PetHP < 250)
            {
                UseSkill(Globals.Instance.SkillDictionary["SummonII"], mre);
                Globals.Instance.KeySenderInstance.SendKey(Keys.D1, KeySender.ModifierControl); //Pet Obey on target
            }

            if (RestoreHP(mre) == "escape") return "escape";
            UseSkill(Globals.Instance.SkillDictionary["Sustain"], mre);
            if (target.HP <= 0) return "success";

            if (RestoreHP(mre) == "escape") return "escape";
            UseSkill(Globals.Instance.SkillDictionary["Sustain"], mre);
            if (target.HP <= 0) return "success";

            if (RestoreHP(mre) == "escape") return "escape";
            UseSkill(Globals.Instance.SkillDictionary["Aetherflow"], mre);
            UseSkill(Globals.Instance.SkillDictionary["EnergyDrain"], mre);
            UseSkill(Globals.Instance.SkillDictionary["Virus"], mre);
            UseSkill(Globals.Instance.SkillDictionary["Curl"], mre, true);
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
        public static void Turn180(ManualResetEvent mre)
        {
            Globals.Instance.GameLogger.Log("Turn180() initiated...");
            if (!mre.WaitOne(0))
            {
                var random = Utils.getRandom(0, 10);
                var random2 = Utils.getRandom(0, 1);
                if (random < 5)
                {
                    Globals.Instance.KeySenderInstance.SendDown(random2 == 0 ? Keys.A : Keys.D);
                    mre.WaitOne(800); //Turn 90 degrees
                }
                else
                {
                    Globals.Instance.KeySenderInstance.SendDown(random2 == 0 ? Keys.A : Keys.D);
                    mre.WaitOne(1500); //Turn 180 degrees
                }
                Globals.Instance.KeySenderInstance.SendUp(random2 == 0 ? Keys.A : Keys.D);
                mre.WaitOne(1500); //Run straight for a bit
            }
        }

        private void Reposition(WaitHandle mre)
        {
            Globals.Instance.GameLogger.Log("Reposition() initiated...");
            if (mre.WaitOne(0)) return;
            Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad5);
            mre.WaitOne(300);
            Globals.Instance.KeySenderInstance.SendDown(Keys.W);
            mre.WaitOne(5000);
            Globals.Instance.KeySenderInstance.SendUp(Keys.W);
            mre.WaitOne(300);
            Globals.Instance.KeySenderInstance.SendDown(Keys.A);
            mre.WaitOne(800); //Turn 90 degrees
            Globals.Instance.KeySenderInstance.SendUp(Keys.A);
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
