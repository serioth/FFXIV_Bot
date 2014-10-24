using System.Globalization;
using System.IO;
using System.Timers;
using System.Xml.Serialization;
using Timer = System.Timers.Timer;

namespace MagBot_FFXIV_v02
{
    //[DataContract(Name = "SkillCollection", Namespace = "")]
    public class Skill
    {
        //[XmlIgnore]
        public string Name { get; private set; }
        public int Cast { get; private set; }
        public int ReCast { get; set; }
        public int MPCost { get; private set; }
        public char Button { get; private set; }

        //[XmlIgnore]
        public Timer Timer { get; private set; }

        //[XmlIgnore]
        public bool Ready { get; set; }

        public Skill()
        {
            Ready = true;

            Timer = new Timer { Interval = ReCast + 500, AutoReset = false };
            Timer.Elapsed += OnTimedEvent;
        }

        public Skill(string name, float cast, float reCast, int mpCost, int button)
        {
            Name = name;
            Cast = (int)(cast*1000);
            ReCast = (int)(reCast*1000);
            MPCost = mpCost;
            Button = button.ToString(CultureInfo.InvariantCulture)[0];

            Ready = true;

            Timer = new Timer { Interval = ReCast + 500, AutoReset = false };
            Timer.Elapsed += OnTimedEvent;
        }

        //Runs when recast is up
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Ready = true;
        }

        public static Skill Deserialize(string path)
        {
            //Only works if each skill is stored as a seperate XML?
            using (var memoryStream = new StreamReader(path + @"\Skill.xml"))
            {
                var serializer = new XmlSerializer(typeof(Skill));
                var local = (Skill)serializer.Deserialize(memoryStream);
                //Skills now contains the entire XML
                local.PostCreateLogic();
                return local;
            }
        }

        public void PostCreateLogic()
        {
            Cast = Cast * 1000;
            ReCast = ReCast * 1000;
        }
    }
}
