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

        public string RunToPoint(Func<Waypoint> wp, CancellationToken ct, bool stopAtEnd, bool aggro)
        {
            Globals.Instance.KeySenderInstance.SendDown(Keys.W); //Redundant if we already running path, but no harm in that

            while ((wp().Distance(WaypointLocation) > ExpFarming.DistanceTreshold))
            {
                if (ct.IsCancellationRequested)
                {
                    Globals.Instance.ExpFarmingLogger.Log("RunToPoint() cancelled...");
                    Globals.Instance.KeySenderInstance.SendUp(Keys.W);
                    ct.ThrowIfCancellationRequested(); //THIS SHOULD PUT WHATEVER TASK CALLED THIS METHOD, INTO CANCELLED STATE
                    return "RunToPoint() cancelled";
                }

                //In the case we escape and die on exit route
                if (HP == 0) return "died";

                //Unless player is currently escaping, he/she should always attack aggressors
                if (!aggro) if (AggroCheck() != null) return "aggro";

                Face(wp());
                if (JumpIfStuck(wp()) == "stuck") return "stuck";
            }

            if (stopAtEnd) Globals.Instance.KeySenderInstance.SendUp(Keys.W);

            return "reached";
        }

        private void Face(Waypoint pt)
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
            while (!IsFacing(pt) && (pt.Distance(WaypointLocation) > ExpFarming.DistanceTreshold))
            {
                Thread.Sleep(10);
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
        private string JumpIfStuck(Waypoint target)
        {
            var oldDistance = WaypointLocation.Distance(target);
            Thread.Sleep(500);
            if (WaypointLocation.Distance(target) < oldDistance) return null;

            for (var jumpCount = 0; jumpCount < 3; jumpCount++)
            {
                Globals.Instance.ExpFarmingLogger.Log("Stuck, jumping...");
                Globals.Instance.KeySenderInstance.SendKey(Keys.Space);
                Thread.Sleep(300); //To avoid jumping too quickly
                if (WaypointLocation.Distance(target) < oldDistance) return "jump ok";
            }
            return "stuck";
        }

        //Runs to and attacks an enemy
        public string AttackTarget(Character target, CancellationToken ct)
        {
            //TODO: This does not work as we are running continuosly. Place elsewhere (ensure we stop before running to enemy?
            //if (!aggro) WaitForRegen(ct);

            Globals.Instance.ExpFarmingLogger.Log("Attacking: " + target.Name);
            Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad0); //Engage enemy

            //Keep attacking until target is dead
            while (target.HP > 0)
            {
                if (ct.IsCancellationRequested)
                {
                    Globals.Instance.ExpFarmingLogger.Log("AttackTarget() cancelled...");
                    ct.ThrowIfCancellationRequested(); //THIS SHOULD PUT WHATEVER TASK CALLED THIS METHOD, INTO CANCELLED STATE
                    return "cancelled";
                }

                if (SummonerAttackPattern(target) != "escape") continue;
                return "escape";
            }
            return "success";
        }

        private void WaitForRegen(CancellationToken ct)
        {
            if (MP < MaxMP * 0.6) Globals.Instance.ExpFarmingLogger.Log("MP < 60%. Waiting for MP to restore...");
            while ((MP < MaxMP * 0.6))
            {
                if (ct.IsCancellationRequested)
                {
                    Globals.Instance.ExpFarmingLogger.Log("WaitForRegen() cancelled...");
                    ct.ThrowIfCancellationRequested(); //THIS SHOULD PUT WHATEVER TASK CALLED THIS METHOD, INTO CANCELLED STATE
                    return;
                }

                Thread.Sleep(300);
            }
        }

        private string RestoreHP()
        {
            while (HP < (MaxHP * 0.8))
            {
                if ((MP < (MaxMP * 0.4)) || HP < MaxHP * 0.2)
                {
                    return "escape";
                }
                UseSkill(Globals.Instance.SkillDictionary["Physick"]);
            }
            return "OK";
        }

        public static Character AggroCheck()
        {
            Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad8, KeySender.ModifierControl);
            Thread.Sleep(200);
            var target = GetTarget(Globals.Instance.MemoryBaseOffsetDictionary["Target2"]);
            if (target == null) return null;

            Globals.Instance.ExpFarmingLogger.Log("Aggressor found");
            return target;
        }

        public static Character EnemyCheck()
        {
            Globals.Instance.KeySenderInstance.SendKey(Keys.Tab);
            Thread.Sleep(200);
            var target = GetTarget(Globals.Instance.MemoryBaseOffsetDictionary["Target1"]);
            if (target == null) return null;

            Globals.Instance.ExpFarmingLogger.Log("Enemy found");
            return target;
        }

        public static void Turn180()
        {
            Thread.Sleep(Utils.getRandom(300,600));

            Globals.Instance.KeySenderInstance.SendDown(Keys.A);
            Thread.Sleep(1500);
            Globals.Instance.KeySenderInstance.SendUp(Keys.A);
            Thread.Sleep(500);
            Globals.Instance.KeySenderInstance.SendDown(Keys.W);
            Thread.Sleep(1500);
            Globals.Instance.KeySenderInstance.SendUp(Keys.W);
        }

        public static Character GetTarget(int targetBaseOffset)
        {
            //This checks whether or not we are actually targeting something
            int lvlOneValue;
            IntPtr targetPointer = MemoryHandler.Instance.GetLvlOneAddressFromBaseOffset(targetBaseOffset);
            MemoryHandler.Instance.ReadInt(targetPointer, out lvlOneValue);
            if (lvlOneValue <= 0) return null;

            //Every time we create a Character instance the pointers are recreated (see constructor and LoadPointers())
            var target = new Character(targetBaseOffset);
            return target;
        }

        private void UseSkill(Skill skill)
        {
            if (skill.Ready != true || !(MP > skill.MPCost)) return;

            Globals.Instance.ExpFarmingLogger.Log("Using skill: " + skill.Name);

            Globals.Instance.KeySenderInstance.SendKey(Globals.Instance.KeySenderInstance.ToKey(skill.Button));

            skill.Timer.Start();
            skill.Ready = false;

            //After initiating the skill we need to sleep until it is finished casting
            //Sleep amount = random number from the time it takes to cast up to a second longer
            Thread.Sleep(Utils.getRandom(skill.Cast + 600, skill.Cast + 1200));
        }

        private string SummonerAttackPattern(Character target)
        {
            if (RestoreHP() == "escape") return "escape";

            if (PetInfo.PetHP < 100)
            {
                UseSkill(Globals.Instance.SkillDictionary["SummonII"]);
            }

            if (RestoreHP() == "escape") return "escape";
            UseSkill(Globals.Instance.SkillDictionary["Aetherflow"]);
            UseSkill(Globals.Instance.SkillDictionary["EnergyDrain"]);
            UseSkill(Globals.Instance.SkillDictionary["Virus"]);
            if (target.HP <= 0) return "success";

            if (RestoreHP() == "escape") return "escape";
            UseSkill(Globals.Instance.SkillDictionary["Miasma"]);
            if (target.HP <= 0) return "success";

            if (RestoreHP() == "escape") return "escape";
            UseSkill(Globals.Instance.SkillDictionary["Bio"]);
            if (target.HP <= 0) return "success";

            if (RestoreHP() == "escape") return "escape";
            UseSkill(Globals.Instance.SkillDictionary["Ruin"]);
            
            return "pattern complete"; //No need to check if enemy is dead here, that's done in loop
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
