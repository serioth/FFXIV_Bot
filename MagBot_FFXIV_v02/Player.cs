using System;
using System.Threading;
using System.Windows.Forms;

namespace MagBot_FFXIV_v02
{
    class Player : Character
    {
        private readonly int _targetTwoBaseOffset;

        public Player(int baseOffset)
            : base(baseOffset)
        {
            _targetTwoBaseOffset = Globals.Instance.MemoryBaseOffsetDictionary["Target2"];
            PetInfo = new PetInfo();
        }

        private PetInfo PetInfo { get; set; }

        private Character Target { get; set; }

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
            while ((absAngleDiff > 0.15) && (pt.Distance(WaypointLocation) > ExpFarming.WaypointTurnDistance) && ExpFarming.Running)
            {
                angleDiff = FacingAngle - pt.Angle(WaypointLocation);
                absAngleDiff = Math.Abs(angleDiff);
            }
            Globals.Instance.KeySenderInstance.SendUp(Keys.A);
            Globals.Instance.KeySenderInstance.SendUp(Keys.D);
        }

        public bool IsFacing(Waypoint pt)
        {
            var absAngleDiff = Math.Abs(FacingAngle - pt.Angle(WaypointLocation));
            return !(absAngleDiff > 0.15);
        }


        public string RunToPoint(Waypoint wp, Func<bool> run)
        {
            bool runToEnemy = run.Method.Name.Contains("AttackTarget");
            var treshold = runToEnemy ? ExpFarming.AttackingDistance : ExpFarming.WaypointTurnDistance;
            //Reset Camera
            //MemoryHandler.HumanKeyPress(MemoryApi.KeyCode.NUMLOCK);
            //Thread.Sleep(Utils.getRandom(100,300));
            //MemoryHandler.HumanKeyPress(MemoryApi.KeyCode.END);
            //Thread.Sleep(Utils.getRandom(100, 300));
            //MemoryHandler.HumanKeyPress(MemoryApi.KeyCode.NUMLOCK);

            while ((wp.Distance(WaypointLocation) > treshold) && run())
            {
                if (runToEnemy) wp = Target.WaypointLocation;

                if (HP == 0)
                {
                    return "died";
                }

                if (runToEnemy && !ExpFarming.Aggro)
                {
                    Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad8, KeySender.ModifierControl);
                    Thread.Sleep(Utils.getRandom(100, 200));
                    Character aggressor = GetTarget(_targetTwoBaseOffset);
                    if (aggressor != null && Target.ID != aggressor.ID)
                    {
                        Globals.Instance.ExpFamingLogger.Log("Retargeting aggressor...");
                        ExpFarming.Aggro = true;
                        wp = aggressor.WaypointLocation;
                        Target = aggressor;
                    }
                }

                Face(wp);

                //Code to jump over obstacles if stuck
                double tempDistance = WaypointLocation.Distance(wp);
                Thread.Sleep(500);
                var jumpCount = 0;
                while (!(WaypointLocation.Distance(wp) < tempDistance * 0.99) && run())
                {
                    Globals.Instance.ExpFamingLogger.Log("Jumping...");
                    Thread.Sleep(Utils.getRandom(800, 1200)); //TO avoid hitting jump to rapidly

                    Globals.Instance.KeySenderInstance.SendKey(Keys.Space);

                    jumpCount++;

                    if (jumpCount == 5)
                    {
                        return "stuck";
                    }
                }
            }
            return ExpFarming.Running ? "point reached" : "running cancelled";
        }

        //Runs to and attacks an enemy
        public string AttackTarget(Character target)
        {
            Target = target;
            Globals.Instance.ExpFamingLogger.Log("Targeting: " + Target.Name);

            //Wait between tab and lock, and provide time to stop RunningHandler
            //PROBLEM: This also makes player wait after one enemy is dead and you search for next
            Thread.Sleep(Utils.getRandom(200, 400));

            //Lock onto enemy
            //Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad5);

            //Thread.Sleep(Utils.getRandom(200, 400)); //After locking onto enemy, wait with running towards it

            if (!ExpFarming.Aggro) WaitForRegen();

            if (!ExpFarming.StandStill)
            {
                Globals.Instance.KeySenderInstance.SendDown(Keys.W);
                ExpFarming.Running = true;
                string runResult = RunToPoint(Target.WaypointLocation, () => ExpFarming.Attacking);
                ExpFarming.Running = false;
                Globals.Instance.KeySenderInstance.SendUp(Keys.W);
                switch (runResult)
                {
                    case("stuck"): return "Stuck. Jumping not successful.";
                    case("running cancelled"): return "attack cancelled";
                }
            }

            if (WaypointLocation.Distance(Target.WaypointLocation) > ExpFarming.MaxPullingDistance) return "target too far";

            //Summoner battle preparation
            //Here we look up in dictionary each time, that's ok as it is not a heavy operation
            //Thread.Sleep(Utils.getRandom(200, 400));

            Globals.Instance.KeySenderInstance.SendKey(Keys.NumPad0);

            //Keep attacking until target is dead (not targetting anymore)
            while (Target.HP > 0 && ExpFarming.Attacking)
            {
                if(RestoreHP()=="escape") return "escape";

                if (PetInfo.PetHP < 100)
                {
                    UseSkill(Globals.Instance.SkillDictionary["SummonII"]);
                }

                if (Target.HP <= 0) break;

                if (RestoreHP() == "escape") return "escape";
                UseSkill(Globals.Instance.SkillDictionary["Aetherflow"]);
                UseSkill(Globals.Instance.SkillDictionary["EnergyDrain"]);
                UseSkill(Globals.Instance.SkillDictionary["Virus"]);
                
                if (RestoreHP() == "escape") return "escape";
                UseSkill(Globals.Instance.SkillDictionary["Miasma"]);
                if (Target.HP <= 0) break;
                
                if (RestoreHP() == "escape") return "escape";
                UseSkill(Globals.Instance.SkillDictionary["Bio"]);
                if (Target.HP <= 0) break;
                
                if (RestoreHP() == "escape") return "escape";
                UseSkill(Globals.Instance.SkillDictionary["Ruin"]);
            }
            //No longer targetting the enemy, which means it must be dead

            return ExpFarming.Attacking ? "target defeated" : "attack cancelled";
        }

        private void WaitForRegen()
        {
            if (MP < MaxMP * 0.6) Globals.Instance.ExpFamingLogger.Log("MP < 60%. Waiting for MP to restore...");
            while ((MP < MaxMP * 0.6) && ExpFarming.Attacking)
            {
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

        public void Turn180()
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

        public Character GetTarget(int targetBaseOffset)
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
            if (!ExpFarming.Attacking) return;

            if (skill.Ready != true || !(MP > skill.MPCost)) return;

            Globals.Instance.ExpFamingLogger.Log("Using skill: " + skill.Name);

            Globals.Instance.KeySenderInstance.SendKey(Globals.Instance.KeySenderInstance.ToKey(skill.Button));

            skill.Timer.Start();
            skill.Ready = false;

            //After initiating the skill we need to sleep until it is finished casting
            //Sleep amount = random number from the time it takes to cast up to a second longer
            Thread.Sleep(Utils.getRandom(skill.Cast + 600, skill.Cast + 1200));
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
