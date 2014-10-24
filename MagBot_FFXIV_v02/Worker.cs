using System;
using System.Globalization;
using System.Threading;

namespace MagBot_FFXIV_v02
{
    internal class Worker
    {
        //ISynchronizeInvoke object
        private readonly SynchronizationContext _uiContext;

        private delegate void UpdateTextDelegate(string label, string value);
        private UpdateTextDelegate _updateTextDelegate;

        public ManualResetEvent MRE { get; private set; }

        private Thread MemoryScannerThread { get; set; }

        private readonly Player _player;

        public Worker(Player player)
        {
            _player = player;

            MRE = new ManualResetEvent(false);

            _uiContext = MainForm.Get.UISynchContext;
            _updateTextDelegate = MainForm.Get.UpdateText;
            
            StartScanning();
        }

        private void StartScanning()
        {
            MemoryScannerThread = new Thread(MemoryScanner)
            {
                IsBackground = true,
                Name = "Thread to Keep Scanning for Value Changes"
            };

            MemoryScannerThread.Start();
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
                //SEND:
                //First parameter: A delegate pointing to a method with one object paramter
                //Second paramter: The object that goes into the delegate in the first parameter
                _uiContext.Send(o => _updateTextDelegate(o as string, _player.HP.ToString(CultureInfo.InvariantCulture)), "lbHP");

                _uiContext.Send(o => _updateTextDelegate("lbMaxHP", _player.MaxHP.ToString(CultureInfo.InvariantCulture)), null);

                _uiContext.Send(o => _updateTextDelegate("lbMP", _player.MP.ToString(CultureInfo.InvariantCulture)), null);

                _uiContext.Send(o => _updateTextDelegate("lbMaxMP", _player.MaxMP.ToString(CultureInfo.InvariantCulture)), null);

                _uiContext.Send(o => _updateTextDelegate("lbTP", _player.TP.ToString(CultureInfo.InvariantCulture)), null);

                _uiContext.Send(o => _updateTextDelegate("lbMaxTP", _player.MaxTP.ToString(CultureInfo.InvariantCulture)), null);

                _uiContext.Send(o => MainForm.Get.UpdateText("lbXCoordinate", _player.XCoordinate.ToString(CultureInfo.InvariantCulture)), null);

                _uiContext.Send(o => MainForm.Get.UpdateText("lbYCoordinate", _player.YCoordinate.ToString(CultureInfo.InvariantCulture)), null);

                _uiContext.Send(o => MainForm.Get.UpdateText("lbZCoordinate", _player.ZCoordinate.ToString(CultureInfo.InvariantCulture)), null);

                _uiContext.Send(o => MainForm.Get.UpdateText("lbFacing", _player.FacingAngle.ToString(CultureInfo.InvariantCulture)), null);
            }

            MainForm.Get.Invoke((Action)(() => MainForm.Get.StopScanning()));
        }
    }
}