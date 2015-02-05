using System;
using System.Globalization;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MagBot_FFXIV_v02
{

    internal class TweaksHandler
    {
        private readonly SynchronizationContext _uiContext;
        private readonly ManualResetEvent _mre;
        private Thread _memoryScannerThread;
        public readonly Timer AlwaysRunTimer;

        private readonly Player _player;

        public TweaksHandler(Player player)
        {
            _player = player;
            _mre = new ManualResetEvent(false);
            _uiContext = MainForm.Get.UISynchContext;

            //Setup Always Run Timer
            AlwaysRunTimer = new Timer { Interval = 2000, AutoReset = true };
            AlwaysRunTimer.Elapsed += (sender ,e) => OnTimedEvent_AlwaysRunTimer(_mre);
        }

        private void OnTimedEvent_AlwaysRunTimer(ManualResetEvent mre)
        {
            _player.UseSkill(Globals.Instance.SkillDictionary["Run"], mre);
        }

        private void StartScanning()
        {
            _memoryScannerThread = new Thread(s => MemoryScanner(_mre))
            {
                IsBackground = true,
                Name = "Thread to Keep Scanning for Value Changes"
            };
            _memoryScannerThread.Start();
        }

        private void MemoryScanner(ManualResetEvent mre)
        {
            //Just so WinForm has time to create itself before we start sending values to the label 
            mre.WaitOne(1000);

            //Continuously update values
            //DEADLOCK: The _value delegate method is called by main UI thread. But the main UI thread is also what waits for that call to finish
            //Solution: FarmingMre FormClosing. When this method is complete it will invoke a method that closes the form and does cleanup

            //Use WaitOne with a time-out, instead of sleep, as it can be cancelled with .Set
            while (!mre.WaitOne(50))
            {
                //A lambda expression is an anonymous method. You can run other methods from within this (including launch these methods through a delegate), as seen below
                //Send(delegate pointing to a method with one object paramter, object that goes into the delegate in the first parameter)
                _uiContext.Send(o => MainForm.Get.UpdateText("lbHP", _player.HP.ToString(CultureInfo.InvariantCulture)), null);
            }
            MainForm.Get.Invoke((Action)(() => MainForm.Get.StopScanning()));
        }
    }
}