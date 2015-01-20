using System;
using System.Globalization;
using System.Threading;

namespace MagBot_FFXIV_v02
{

    //TODO: CHAT LOG CHECK TIMER: Every 500ms
    //TODO: FIX THAT PET AOEs
    internal class Worker
    {
        private readonly SynchronizationContext _uiContext;
        public ManualResetEvent MRE { get; private set; }
        private Thread _memoryScannerThread;

        private readonly Player _player;

        public Worker(Player player)
        {
            _player = player;
            MRE = new ManualResetEvent(false);
            _uiContext = MainForm.Get.UISynchContext;
            StartScanning();
        }

        private void StartScanning()
        {
            _memoryScannerThread = new Thread(MemoryScanner)
            {
                IsBackground = true,
                Name = "Thread to Keep Scanning for Value Changes"
            };
            _memoryScannerThread.Start();
        }

        private void MemoryScanner()
        {
            //Just so WinForm has time to create itself before we start sending values to the label 
            MRE.WaitOne(1000);

            //Continuously update values
            //DEADLOCK: The _value delegate method is called by main UI thread. But the main UI thread is also what waits for that call to finish
            //Solution: Cancel FormClosing. When this method is complete it will invoke a method that closes the form and does cleanup

            //Use WaitOne with a time-out, instead of sleep, as it can be cancelled with .Set
            while (!MRE.WaitOne(50))
            {
                //A lambda expression is an anonymous method. You can run other methods from within this (including launch these methods through a delegate), as seen below
                //Send(delegate pointing to a method with one object paramter, object that goes into the delegate in the first parameter)
                _uiContext.Send(o => MainForm.Get.UpdateText("lbHP", _player.HP.ToString(CultureInfo.InvariantCulture)), null);
            }
            MainForm.Get.Invoke((Action)(() => MainForm.Get.StopScanning()));
        }
    }
}