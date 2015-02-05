using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace MagBot_FFXIV_v02
{
    public partial class MainForm : Form
    {
        //Should really be loaded from config file
        private const string ProcessName = "ffxiv";
        private const string FFXIVFolder = @"My Games\FINAL FANTASY XIV - A Realm Reborn\Bot Config Files";

        private bool _getProcessResult;
        private Player _player;
        private FarmingHandler _farmingHandler;
        private TweaksHandler _tweaksHandler;
        private ChatLogHandler _chatLogHandler;
        private Thread _farmingThread;
        private Timer _farmingTimer;
        public int FarmingSecondsPassed;
        private ManualResetEvent _chatLogMre;
        private RouteManager _farmingRouteManager;

        public SynchronizationContext UISynchContext { get; private set; }

        //When stopping exp farming, this delegate can be invoked to fix button visibility
        //We here demonstrate the use of the predeclared Action delegate with lambda expression (to make it anonymous)
        public readonly Action SEFDelegate;

        public static MainForm Get { get; private set; }

        private bool ClosePending { get; set; }

        public MainForm()
        {
            InitializeComponent();
            Get = this;
            UISynchContext = SynchronizationContext.Current;

            SEFDelegate = () =>
            {
                Globals.Instance.GameLogger.Log("SEFDelegate called, fixing control visibility...");
                EnableChildrenControls(FarmingTab);
                _farmingTimer.Stop();
                btStopExpFarming.Enabled = false;
            };
        }

        public static string FFXIVFolderPath { get; private set; }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //1)Make sure config folder exist and start application Logging
            string documentsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            FFXIVFolderPath = Path.Combine(documentsDir, FFXIVFolder);
            if (!Directory.Exists(FFXIVFolderPath))
            {
                var message = "FFXIV config directory (" + FFXIVFolderPath + ") does not exist, please select new folder...";
                Globals.Instance.ShowMessage(message);

                // Show the FolderBrowserDialog
                var folderBrowserDialog1 = new FolderBrowserDialog();
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    FFXIVFolderPath = folderBrowserDialog1.SelectedPath;
                }
                else
                {
                    Application.Exit();
                    return;
                }
            }

            //Instantiate Loggers
            Globals.Instance.ApplicationLogger = new Logger("ApplicationLog");
            Globals.Instance.GameLogger = new Logger("GameLog");

            //2 Load XML Files and initialize other globals dictionaries
            Globals.Instance.InitializeDictionaries();

            //3) Find, open and attach to selected process
            ProcessInfo[] pi;
            _getProcessResult = ProcessInfo.Get(ProcessName, out pi);
            if (_getProcessResult && (pi.Length == 1))
            {
                MemoryHandler.Instance.Initialize(pi[0].Id, null);
            }
            else
            {
                const string message = "Could not load process, exiting application...";
                Globals.Instance.ShowMessage(message);
                Globals.Instance.ApplicationLogger.Log(message);
                Application.Exit();
                return;
            }

            //4) Create KeySender instance
            Globals.Instance.KeySenderInstance = new KeySender(MemoryHandler.Instance.MainWindowHandle);

            //5) Create Player instance
            _player = new Player(Globals.Instance.MemoryBaseOffsetDictionary["Player"]);

            //Check that Memory Pointers have not changed
            if (!(_player.HP > 10 && _player.HP < 10000))
            {
                const string message = "Memory addresses have changed, please update XML file(s). Exiting application...";
                Globals.Instance.ShowMessage(message);
                Globals.Instance.ApplicationLogger.Log(message);
                Application.Exit();
            }

            //Start ChatLogHandler
            _chatLogMre = new ManualResetEvent(false);
            _chatLogHandler = new ChatLogHandler();
            _chatLogHandler.StartChatLogMonitoring(_chatLogMre);

            //Instantiate _tweaksHandler
            _tweaksHandler = new TweaksHandler(_player);
        }

        private void tab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as TabControl) == null) return;
            var page = (sender as TabControl).SelectedTab;
            
            //Create instances
            if (page != null && page.Name == "FarmingTab")
            {
                if(_farmingRouteManager == null) _farmingRouteManager = new RouteManager();
                if(_farmingHandler == null) _farmingHandler = new FarmingHandler(_player);
            }
        }

        private void btStartExpFarming_Click(object sender, EventArgs e)
        {
            //Check validity of input
            if (_farmingRouteManager.Routes.Count == 0 || _farmingRouteManager.Routes.Any(route => route.Points.Count < 3))
            {
                Globals.Instance.ShowMessage(@"Please add at least three points to every route.");
                return;
            }

            //Disables all controls except for stop button
            DisableChildrenControls(FarmingTab);
            EnableControl(btStopExpFarming);

            //Target List
            var targetList = tbEnemyNames.Text.Trim();
            var targetArray = targetList.Split(',').Select(target => target.Trim()).ToArray();

            //Set up timer to end expFarming automatically
            _farmingTimer = new Timer { Interval = 1000, AutoReset = true }; //Ticks every second
            var secondsToRun = Convert.ToInt32(nudHours.Text) * 3600 + Convert.ToInt32(nudMinutes.Text) * 60;
            FarmingSecondsPassed = 0;

            var selectedEscapeRoute = _farmingRouteManager.Routes[cbEscapeRoutes.SelectedIndex];

            if (rbBattle.Checked)
            {
                //Just takes the first row in selection, if selected several rows. SubItem[0] is the waypoint index
                if (cbStandStill.Checked)
                {
                    _farmingThread = new Thread(() => _farmingHandler.StartStandStillExpFarming(selectedEscapeRoute));
                    _farmingTimer.Elapsed += (s, a) => OnTimedEvent_FarmingTimer(secondsToRun);
                }
                else
                {
                    var selectedRoute = _farmingRouteManager.Routes[Convert.ToInt32(lvWaypoints.SelectedItems[0].Text) - 1];
                    _farmingThread = new Thread(() => _farmingHandler.StartExpFarming(selectedRoute, selectedEscapeRoute, targetArray));
                    _farmingTimer.Elapsed += (s, a) => OnTimedEvent_FarmingTimer(secondsToRun, selectedEscapeRoute);
                }
            }
            else if (rbDoL.Checked)
            {
                var selectedRoute = _farmingRouteManager.Routes[Convert.ToInt32(lvWaypoints.SelectedItems[0].Text) - 1];
                _farmingThread = new Thread(() => _farmingHandler.StartGathering(selectedRoute, selectedEscapeRoute));
                _farmingTimer.Elapsed += (s, a) => OnTimedEvent_FarmingTimer(secondsToRun, selectedEscapeRoute);
            }
            _farmingThread.IsBackground = true;
            _farmingThread.Name = "FarmingHandler Thread";
            _farmingThread.Start();
            _farmingTimer.Start();

            //Give focus to window
            MemoryHandler.Instance.SwitchWindow();
        }

        private void OnTimedEvent_FarmingTimer(int totalSecondsToRun, Route escapeRoute = null)
        {
            FarmingSecondsPassed++;
            var ts = TimeSpan.FromSeconds(FarmingSecondsPassed);
            var timeElapsed = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", ts.Hours, ts.Minutes, ts.Seconds);
            UISynchContext.Send(o => lbFarmingElapsedTime.Text = timeElapsed, null);
            if (FarmingSecondsPassed != totalSecondsToRun) return;

            _farmingTimer.Stop();
            Globals.Instance.GameLogger.Log("Farming timer up, proceeding to escape route...");
            _farmingHandler.StopApp();
            _farmingThread.Join();
            _farmingHandler.FarmingMre.Reset();

            if (escapeRoute != null)
            {
                Task.Run(() => _farmingHandler.FinalActions(escapeRoute, _farmingHandler.FarmingMre));
            }
        }

        private void btStopExpFarming_Click(object sender, EventArgs e)
        {
            _farmingHandler.StopApp();
        }

        private void btNewRoute_Click(object sender, EventArgs e)
        {
            NewRoute(_farmingRouteManager, lvWaypoints, cbEscapeRoutes);
        }

        private void btRecWaypoint_Click(object sender, EventArgs e)
        {
            RecordWaypoint(_farmingRouteManager, lvWaypoints, cbEscapeRoutes);
        }

        private void btDelWaypoint_Click(object sender, EventArgs e)
        {
            DeleteWaypoint(_farmingRouteManager, lvWaypoints, cbEscapeRoutes);
        }

        private void btLoadRoutes_Click(object sender, EventArgs e)
        {
            LoadRoutes(_farmingRouteManager, lvWaypoints, cbEscapeRoutes);
        }

        private void btSaveRoutes_Click(object sender, EventArgs e)
        {
            SaveRoutes(_farmingRouteManager);
        }

        private void rbDoL_CheckedChanged(object sender, EventArgs e)
        {
            cbStandStill.Enabled = !rbDoL.Checked;
        }

        private void cbAlwaysRun_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAlwaysRun.Checked) _tweaksHandler.AlwaysRunTimer.Start();
            else _tweaksHandler.AlwaysRunTimer.Stop();
        }

        private void lvWaypoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbStandStill.Checked) return;
            ListViewButtonFixer(_farmingRouteManager, lvWaypoints, btStartExpFarming);
        }

        private void cbStandStill_CheckChanged(object sender, EventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                if (cbEscapeRoutes.Items.Count == 0)
                {
                    Globals.Instance.ShowMessage("Please select an escape route first.");
                    cb.Checked = false;
                    return;
                }
                var escapeRoute = _farmingRouteManager.Routes[cbEscapeRoutes.SelectedIndex];
                btStartExpFarming.Enabled = escapeRoute.Points.Count > 2;
            }
        }

        private void NewRoute(RouteManager rm, ListView lv, ComboBox escapeRoutes)
        {
            rm.Routes.Add(new Route("Route " + (rm.Routes.Count + 1)));
            RecordWaypoint(rm, lv, escapeRoutes);
        }

        private void RecordWaypoint(RouteManager rm, ListView lv, ComboBox escapeRoutes)
        {
            if (rm.Routes.Count == 0)
            {
                NewRoute(rm, lv, escapeRoutes);
                return;
            }

            var route = rm.Routes.Last();

            var wp = _player.WaypointLocation;

            if (route.Points.Count > 0)
            {
                for (var i = 0; i < route.Points.Count; i++)
                {
                    var distanceBetweenTargets = route.Points[i].Distance(wp);

                    if (!(distanceBetweenTargets < 4)) continue;
                    MessageBox.Show(@"Point too close to one of the other points.",
                        @"Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                    return;
                }
            }

            route.Points.Add(wp);
            UpdateWaypointList(rm, lv, escapeRoutes);
        }

        private void DeleteWaypoint(RouteManager rm, ListView lv, ComboBox escapeRoutes)
        {
            for (var i = 0; i < lv.Items.Count; i++)
            {
                if (!lv.Items[i].Selected) continue;
                var route = rm.Routes[Convert.ToInt32(lv.Items[i].SubItems[0].Text) - 1];
                var waypoint = route.Points[Convert.ToInt32(lv.Items[i].SubItems[1].Text) - 1];

                route.Points.Remove(waypoint);
                if (route.Points.Count == 0)
                {
                    rm.Routes.Remove(route);
                }
                UpdateWaypointList(rm, lv, escapeRoutes);
            }
        }

        private void LoadRoutes(RouteManager rm, ListView lv, ComboBox escapeRoutes)
        {
            var openFileDialog1 = new OpenFileDialog
            {
                Filter = @"XML Files (.xml)|*.xml",
                FilterIndex = 1,
                Multiselect = false
            };

            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            rm.LoadRouteManager(openFileDialog1.FileName);
            UpdateWaypointList(rm, lv, escapeRoutes);
        }

        private void SaveRoutes(RouteManager rm)
        {
            if (rm.Routes.Count == 0 || rm.Routes.Any(route => route.Points.Count < 3))
            {
                Globals.Instance.ShowMessage(@"Please add at least three points to every route before saving.", @"Save Error");
                return;
            }

            var saveFileDialog1 = new SaveFileDialog
            {
                Filter = @"XML files (*.xml)|*.xml",
                DefaultExt = "xml",
                Title = @"Save"
            };
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                rm.Save(saveFileDialog1.FileName);
            }
        }

        private void UpdateWaypointList(RouteManager rm, ListView lv, ComboBox escapeRoutes)
        {
            escapeRoutes.Items.Clear();

            for (var i = 0; i < lv.Items.Count; i++)
            {
                lv.Items[i].Remove();
                i--;
            }

            for (var i = 0; i < rm.Routes.Count; i++)
            {
                for (var j = 0; j < rm.Routes[i].Points.Count; j++)
                {
                    var lvi = new ListViewItem((i + 1).ToString(CultureInfo.InvariantCulture), 0);
                    lvi.SubItems.Add((j + 1).ToString(CultureInfo.InvariantCulture));
                    lvi.SubItems.Add(rm.Routes[i].Points[j].X.ToString(CultureInfo.InvariantCulture));
                    lvi.SubItems.Add(rm.Routes[i].Points[j].Y.ToString(CultureInfo.InvariantCulture));
                    lvi.SubItems.Add(rm.Routes[i].Points[j].Z.ToString(CultureInfo.InvariantCulture));
                    lv.Items.Add(lvi);
                }

                escapeRoutes.Items.Add(rm.Routes[i].Name);
            }

            if (lv.Items.Count <= 0) return;
            escapeRoutes.SelectedIndex = 0;
            lv.Items[0].Selected = true;
        }

        private void ListViewButtonFixer(RouteManager rm, ListView lv, Button startButton)
        {
            //If nothing is selected then you shouldn't be allowed to delete.
            //Only start exp farming if you have 2 or more points
            //Start exp farming from selected route, if none is selected then disable the button
            //If cbStandStill is checked, it does not matter if anything is selected 
            if (lv.SelectedItems.Count > 0)
            {
                var selectedRoute = Convert.ToInt32(lv.SelectedItems[0].Text) - 1;
                startButton.Enabled = rm.Routes[selectedRoute].Points.Count > 2;
            }
            else
            {
                startButton.Enabled = false;
            }
        }

        //Not ideal way to communicate with UI, because
        //1) UI thread has to deal with logic that depends on what thread the method was called from
        //2) If My worker class that calls this method was a library, it means that it would have to assume that
        //it is used with a Windows Form because WPF and Metro etc does not use Invoke, they use SynchronizationContext
        //And by using SynchronizationContext you can avoid this code and just use UpdateText(), below
        public void UIThreadInvoke(Control control, Action code)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(code);
                return;
            }
            code.Invoke();
        }

        public void UpdateText(string labelName, string text)
        {
            var label = (Label)Controls.Find(labelName, true).FirstOrDefault();
            if (label != null) label.Text = text;
        }

        public string GetText(string labelName)
        {
            var label = (Label)Controls.Find(labelName, true).FirstOrDefault();
            return label != null ? label.Text : null;
        }

        public void AddToChatLog(string text)
        {
            tbChatLog.AppendText(Environment.NewLine + text);
        }

        private void DisableChildrenControls(Control con)
        {
            foreach (Control c in con.Controls)
            {
                DisableChildrenControls(c);
            }
            con.Enabled = false;
        }

        private void EnableChildrenControls(Control con)
        {
            foreach (Control c in con.Controls)
            {
                EnableChildrenControls(c);
            }
            con.Enabled = true;
        }

        private void EnableControl(Control con)
        {
            if (con != null)
            {
                con.Enabled = true;
                EnableControl(con.Parent);
            }
        }

        private void checkValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var hp = _player.HP.ToString(CultureInfo.InvariantCulture);
            var maxHp = _player.MaxHP.ToString(CultureInfo.InvariantCulture);
            var mp = _player.MP.ToString(CultureInfo.InvariantCulture);
            var maxMp = _player.MaxMP.ToString(CultureInfo.InvariantCulture);
            var tp = _player.TP.ToString(CultureInfo.InvariantCulture);
            var gp = _player.GP.ToString(CultureInfo.InvariantCulture);
            var maxGp = _player.MaxGP.ToString(CultureInfo.InvariantCulture);
            var xCoor = _player.XCoordinate.ToString(CultureInfo.InvariantCulture);
            var yCoor = _player.YCoordinate.ToString(CultureInfo.InvariantCulture);
            var zCoor = _player.ZCoordinate.ToString(CultureInfo.InvariantCulture);
            var facing = _player.FacingAngle.ToString(CultureInfo.InvariantCulture);

            var combinedMessage = "HP: " + hp + "/" + maxHp + Environment.NewLine + ". MP: " + mp + "/" + maxMp + Environment.NewLine + ". TP: " + tp + ". GP: " + gp + "/" + maxGp + Environment.NewLine + ". X, Y, Z: " + xCoor + ", " + yCoor + ", " + zCoor + Environment.NewLine + "Facing direction (radians): " + facing;

            Globals.Instance.ShowMessage(combinedMessage);
        }

        public void StopScanning()
        {
            if (!ClosePending) return;
            const string message = "External thread is closing the application...";
            Globals.Instance.ApplicationLogger.Log(message);
            Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var button = sender as Button;
            if (button != null && string.Equals(button.Name, @"CloseButton"))
            {
                //FormClosing event raised by a user created button action
                //const string message = "Form closing by user action, exiting application...";
                //Globals.Instance.ShowMessage(message);
                //Globals.Instance.LoggerDictionary["ApplicationLog"].Log(message);
            }
            else
            {
                //FormClosing event raised by program or the X in top right corner
                //Do cleanup work (stop threads and clean up unmanaged resources)

                if (_chatLogMre != null &&!_chatLogMre.WaitOne(0))
                {
                    _chatLogMre.Set(); //This exits the loops
                    ClosePending = true;
                    e.Cancel = true;
                    return;
                }

                if (Globals.Instance.ApplicationLogger != null) Globals.Instance.ApplicationLogger.Log("FormClosing event raised by program. Cleaning up resources, then exiting application...");

                if (_farmingHandler != null)
                {
                    if (_farmingThread != null && !_farmingHandler.FarmingMre.WaitOne(0)) btStopExpFarming_Click(sender, e);
                    Console.WriteLine("HERE");
                    if (_farmingThread != null && _farmingThread.IsAlive) _farmingThread.Join(); //Wait with disposing GameLogger until thread is terminated
                    Console.WriteLine("NOT HERE");
                    Globals.Instance.GameLogger.Dispose();
                }

                MemoryHandler.Instance.Dispose();
                if (Globals.Instance.ApplicationLogger != null) Globals.Instance.ApplicationLogger.Dispose();
            }
        }
    }
}