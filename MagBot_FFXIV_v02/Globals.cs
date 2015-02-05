using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MagBot_FFXIV_v02
{
    public class Globals
    {
        //Thread-safe implementation of the Singeleton design pattern
        //Static constructor (below instantiation) execute only when an instance of the class is created or a static member is referenced
        //In other words, this is lazy as it only happens when the property is accessed
        //Private constructor needed else compiler creates public one
        //Flaw: If you have static members other than Instance, the first reference to those members will involve creating the instance
        //The laziness of type initializers is only guaranteed by .NET when the type isn't marked with a special flag called beforefieldinit 
        //C# compiler marks all types which don't have a static constructor (i.e. a block which looks like a constructor but is marked static) as beforefieldinit
        //Therefore need the static constructor as well
        private static readonly Globals GlobalsInstance = new Globals();
        static Globals() { }

        private Globals() { }
        public static Globals Instance
        {
            get { return GlobalsInstance; }
        }

        //Dictionaries
        public IDictionary<string, Skill> SkillDictionary { get; private set; }
        public IDictionary<string, int> MemoryBaseOffsetDictionary { get; private set; }
        public IDictionary<string, int[]> MemoryAdditionalOffsetDictionary { get; private set; } //These offsets do not change with game update
        public KeySender KeySenderInstance { get; set; }
        
        //Loggers
        public Logger ApplicationLogger { get; set; }
        public Logger GameLogger { get; set; }

        private delegate DialogResult ShowMessagehandler(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);
        public DialogResult ShowMessage(string message, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            DialogResult result;
            if (MainForm.Get.InvokeRequired)
                result = (DialogResult)MainForm.Get.Invoke(new ShowMessagehandler(doUiShowMessage), new object[] { message, caption, buttons, icon });
            else
                result = doUiShowMessage(message, caption, buttons, icon);
            return result;
        }
        private DialogResult doUiShowMessage(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return string.IsNullOrWhiteSpace(message) ? DialogResult.None : MessageBox.Show(message, caption, buttons, icon);
        }

        public void InitializeDictionaries()
        {
            //Console.WriteLine("File exist (" + MainForm.FFXIVFolderPath + @"\offsets.xml" + "): " + File.Exists(MainForm.FFXIVFolderPath + @"\offsets.xml"));
            string path = MainForm.FFXIVFolderPath;

            //Load Offset lists
            try
            {
                var baseXElement = XDocument.Load(path + @"\MemoryBaseOffsets.xml").Root;
                if (baseXElement != null)
                    MemoryBaseOffsetDictionary =
                        baseXElement.Elements()
                            .ToDictionary(o => o.Name.LocalName,
                                o => int.Parse(o.Value.Substring(2), NumberStyles.AllowHexSpecifier));

                var offsetXElement = XDocument.Load(path + @"\MemoryAdditionalOffsets.xml").Root;
                if (offsetXElement != null)
                    MemoryAdditionalOffsetDictionary =
                        offsetXElement.Elements()
                            .ToDictionary(o => o.Name.LocalName,
                                o => o.Elements().Select(x => int.Parse(x.Value.Substring(2), NumberStyles.AllowHexSpecifier)).ToArray());

                //Load Skill list
                var skillXElement = XDocument.Load(path + @"\Skills.xml").Root;
                if (skillXElement != null)
                    SkillDictionary =
                        skillXElement.Elements()
                            .ToDictionary(e => e.Name.LocalName,
                                e =>
                                    new Skill(e.Name.LocalName, (float)e.Element("Cast"), (float)e.Element("ReCast"),
                                        (int)e.Element("Cost"), (string)e.Element("Button")));

                //Use XML deserialization to create an instance of SkillCollection class
                //Skills = (SkillCollection)SkillCollection.DataContractSerializer_Deserialize(Path.Combine(path, "Skills.xml"), typeof(SkillCollection));
                //Skills = (SkillCollection)SkillCollection.XmlSerializer_Deserialize(Path.Combine(path, "Skills.xml"), typeof(SkillCollection));

            }
            catch (DirectoryNotFoundException)
            {
                ShowMessage("Directory '" + path + "' not found, exiting application...");
                Application.Exit();
            }
            catch (FileNotFoundException)
            {
                ShowMessage("Required files not found, exiting application...");
                Application.Exit();
            }
        }
    }
}
