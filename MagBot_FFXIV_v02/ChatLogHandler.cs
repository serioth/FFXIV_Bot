using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MagBot_FFXIV_v02
{
    class ChatLogHandler
    {

        private List<String> _log;
        private ManualResetEvent _mreForAddressOffset;
        private volatile int _currentAddressOffset;
        private IntPtr _newLogAddressAfterLastPoll;
        private int _totalBytesReadOnLastPoll;

        private Timer _pollTimer;
        private object _locker;
        const int PollInterval = 2000;

        public ChatLogHandler()
        {
            LoadPointers();
        }

        public void StartChatLogMonitoring(ManualResetEvent mre)
        {
            _locker = new object();
            _log = new List<string>();

            _totalBytesReadOnLastPoll = 0;
            _newLogAddressAfterLastPoll = LogStartPointer;
            _currentAddressOffset = 0;

            //Start Thread that keeps _currentAddressOffset up to date
            _mreForAddressOffset = mre;
            new Thread(() => KeepLogOffsetCurrent(_mreForAddressOffset)).Start();

            //Start timer for polling
            _pollTimer = new Timer(PollInterval) { AutoReset = false };
            _pollTimer.Elapsed += PollTimerOnElapsed;
            _pollTimer.Start();
            
        }

        private void AddTextToLog(string textToAdd)
        {
            //Console.WriteLine(@"Raw Text: " + textToAdd);
            textToAdd = RemoveInvalidCharacters(textToAdd);
            var array = ParseLogText(textToAdd);
            foreach (var entry in array)
            {
                _log.Add(entry);
                //Access control in another class through property
                //If in another thread, you won't be allowed to modify the control, so use Send to marshall the call onto the UI thread
                MainForm.Get.UISynchContext.Send(o => MainForm.Get.AddToChatLog(entry), null);
            }
        }

        private void KeepLogOffsetCurrent(WaitHandle mre)
        {
            while (!mre.WaitOne(50))
            {
                var newAddressOffset = LengthOfLog();
                lock (_locker)
                {
                    if (newAddressOffset > _currentAddressOffset)
                    {
                        //Only update if new value is larger than existing
                        if(newAddressOffset>1000000) Console.WriteLine(@"!!!HIGH LogLength(): " + newAddressOffset);
                        _currentAddressOffset = newAddressOffset;
                    }
                }
            }
            MainForm.Get.Invoke((Action)(() => MainForm.Get.StopScanning()));
        }

        private void PollTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            int currentAddressOffset;

            lock (_locker)
            {
                //_currentAddressOffset is only updated if the offset is larger than previous one
                //So, currentOffset is either most recent, or bottom of previous range
                currentAddressOffset = _currentAddressOffset;
            }
            //Console.WriteLine(@"POLL: _totalBytesReadOnLastPoll: " + _totalBytesReadOnLastPoll + @". currentAddressOffset: " + currentAddressOffset);
            if (_totalBytesReadOnLastPoll <= currentAddressOffset) //Have to include equal, because it might be that non-wrap read, reads to end, and therefore, we never enter again
            {
                var lengthOfLog = LengthOfLog();
                var startOfLog = LogStartPointer;
                string newLogText;
                if (lengthOfLog < currentAddressOffset)
                {
                    Console.WriteLine(@"!!!WRAP!!!");
                    //It has wrapped since last time we polled
                    //We therefore first check if there is more to read at bottom
                    //If it is, we make sure the read the missing bytes
                    var bytesToRead = currentAddressOffset - _totalBytesReadOnLastPoll;
                    if (bytesToRead > 0)
                    {
                        //Console.WriteLine(@"Wrap1: _totalBytesReadOnLastPoll: " + _totalBytesReadOnLastPoll);
                        //Console.WriteLine(@"Wrap1: currentAddressOffset: " + currentAddressOffset);
                        //Console.WriteLine(@"Wrap1: _currentAddressOffset: " + _currentAddressOffset);
                        //Console.WriteLine(@"Wrap1: _newLogAddressAfterLastPoll: " + _newLogAddressAfterLastPoll);
                        //Console.WriteLine(@"Wrap1: bytesToRead: " + bytesToRead);
                        newLogText = GetLog(_newLogAddressAfterLastPoll, bytesToRead);
                        AddTextToLog(newLogText);
                    }
                    //Update global variables
                    lock (_locker) { _currentAddressOffset = 0; }
                    Console.WriteLine(@"ARE WE STUCK AT THE ABOVE LOCK (wrap1)?");
                    _totalBytesReadOnLastPoll = 0;
                    _newLogAddressAfterLastPoll = startOfLog;
                }
                else
                {
                    //It has not wrapped, so we just read the next needed bytes
                    var bytesToRead = currentAddressOffset - _totalBytesReadOnLastPoll;
                    if (bytesToRead > 0)
                    {
                        //if (bytesToRead > 10000) bytesToRead = 10000; //If we start the program far into the game
                        newLogText = GetLog(_newLogAddressAfterLastPoll, bytesToRead);
                        AddTextToLog(newLogText);
                    }
                    //Update global variables
                    _totalBytesReadOnLastPoll = _totalBytesReadOnLastPoll + bytesToRead;
                    _newLogAddressAfterLastPoll = ShiftPointer(_newLogAddressAfterLastPoll, bytesToRead, false);
                }
            }
            _pollTimer.Start();
        }

        private static string RemoveInvalidCharacters(string input)
        {
            //Remove SOT to EOT
            while (true)
            {
                var start = input.IndexOf('\u0002');
                var end = input.IndexOf('\u0003', start + 1);

                if (start == -1 || end == -1) break;

                var diff = end - start;
                input = input.Remove(start, diff + 1);
            }

            //Other cleanup
            input = input.Replace("î‚»", "");
            input = input.Replace("ò«", "");
            input = input.Replace("ê", "");
            input = input.Replace("î¯", "");

            return input;
        }

        private static IEnumerable<string> ParseLogText(string input)
        {
            input = input.Trim();
            input = input.Substring(3, input.Length-3); //This is to avoid that we split the first entry and get an empty string
            string[] splitters = { "54D" };
            var output = input.Split(splitters, StringSplitOptions.None);
            //output = output.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return output;
        }

        private int LengthOfLog()
        {
            var address = LogCounterFirstEmptyBytePointer;

            IntPtr currentLengthOfLogPointer;
            if (address == LogCounterStartPointer)
            {
                currentLengthOfLogPointer = address;
            }
            else
            {
                currentLengthOfLogPointer = IntPtr.Subtract(address, 4);
            }

            int logLengthCurrent;
            MemoryHandler.Instance.ReadInt(currentLengthOfLogPointer, out logLengthCurrent);
            return logLengthCurrent;
        }

        private string GetLog(IntPtr startAddress, int bytesToPull)
        {
            string log;
            MemoryHandler.Instance.ReadString(startAddress, bytesToPull, out log);
            return log;
        }

        private int LengthOfLastLogEntry
        {
            get
            {
                int address;
                MemoryHandler.Instance.ReadInt(_logCounterFirstEmptyBytePointer, out address);
                var previousAddress = new IntPtr(address - 8); //Gets length of log up to previous post

                int logLengthPrevious;
                MemoryHandler.Instance.ReadInt(previousAddress, out logLengthPrevious);
                return LengthOfLog() - logLengthPrevious;
            }
        }

        private IntPtr ShiftPointer(IntPtr address, int bytesToShift, bool subtract = true)
        {
            var shiftedIntPtr = subtract ? IntPtr.Subtract(address, bytesToShift) : IntPtr.Add(address, bytesToShift);
            return shiftedIntPtr;
        }

        private string LastLogEntry()
        {
            var lengthOfLastLogPost = LengthOfLastLogEntry; //Maybe we need this to be hex?
            int firstEmptyByteAddress;
            MemoryHandler.Instance.ReadInt(_logFirstEmptyBytePointer, out firstEmptyByteAddress);
            var startAddress = new IntPtr(firstEmptyByteAddress - lengthOfLastLogPost); //Gets length of log up to previous post

            var lastLogEntry = GetLog(startAddress, lengthOfLastLogPost);
            return lastLogEntry;
        }

        private string GetUnpulledLogEntries(ref int bytesPulled)
        {
            var currentBytesPulled = bytesPulled;
            var totalBytesToPull = LengthOfLog() - currentBytesPulled;
            bytesPulled = currentBytesPulled + totalBytesToPull;

            int addressToFirstByteInLog;
            MemoryHandler.Instance.ReadInt(_logStartPointer, out addressToFirstByteInLog);

            var startAddressTemp = addressToFirstByteInLog + currentBytesPulled;
            var startAddress = new IntPtr(startAddressTemp);

            var logEntries = GetLog(startAddress, totalBytesToPull);

            if (logEntries.Length == 0) logEntries = null;

            return logEntries;
        }

        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings. 
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,  
            // we should return Empty. 
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }

        static string CleanupString(string input)
        {
            const string pattern = "\\s+";
            const string replacement = " ";
            var rgx = new Regex(pattern);
            var result = rgx.Replace(input, replacement);

            return result;
        }

        private static string Replace(string input, string start, string end, string newValue)
        {
            while (true)
            {
                var startIndex = input.IndexOf(start, StringComparison.Ordinal);
                var endIndex = input.IndexOf(end, startIndex + 1, StringComparison.Ordinal);

                if (startIndex == -1 || endIndex == -1) break;

                var diff = endIndex - startIndex + end.Length;
                input = input.Remove(startIndex, diff + 1);
                input = input.Insert(startIndex, newValue);
            }
            return input;
        }

        private IntPtr _logCounterStartPointer;
        private IntPtr LogCounterStartPointer
        {
            get
            {
                int address;
                MemoryHandler.Instance.ReadInt(_logCounterStartPointer, out address);
                return new IntPtr(address);
            }
        }

        private IntPtr _logCounterFirstEmptyBytePointer;
        private IntPtr LogCounterFirstEmptyBytePointer
        {
            get
            {
                int address;
                MemoryHandler.Instance.ReadInt(_logCounterFirstEmptyBytePointer, out address);
                return new IntPtr(address);
            }
        }

        private IntPtr _logCounterEndPointer;
        private IntPtr LogCounterEndPointer
        {
            get
            {
                int address;
                MemoryHandler.Instance.ReadInt(_logCounterEndPointer, out address);
                return new IntPtr(address);
            }
        }

        private IntPtr _logStartPointer;
        private IntPtr LogStartPointer
        {
            get
            {
                int address;
                MemoryHandler.Instance.ReadInt(_logStartPointer, out address);
                return new IntPtr(address);
            }
        }

        private IntPtr _logFirstEmptyBytePointer;
        private IntPtr LogFirstEmptyBytePointer
        {
            get
            {
                int address;
                MemoryHandler.Instance.ReadInt(_logFirstEmptyBytePointer, out address);
                return new IntPtr(address);
            }
        }

        private IntPtr _logEndPointer;
        private IntPtr LogEndPointer
        {
            get
            {
                int address;
                MemoryHandler.Instance.ReadInt(_logEndPointer, out address);
                return new IntPtr(address);
            }
        }

        private void LoadPointers()
        {
            //This will find find the addresses that store the addresses to log and logCounter
            //So we have to read the pointer to find the updated address, then read the address aagin to find the value
            var offsets = new int[3];
            offsets[0] = Globals.Instance.MemoryBaseOffsetDictionary["Log"];
            offsets[1] = Globals.Instance.MemoryAdditionalOffsetDictionary["LogCounterStart"][0]; //18

            offsets[2] = Globals.Instance.MemoryAdditionalOffsetDictionary["LogCounterStart"][1];
            _logCounterStartPointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[2] = Globals.Instance.MemoryAdditionalOffsetDictionary["LogCounterFirstEmptyByte"][1];
            _logCounterFirstEmptyBytePointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[2] = Globals.Instance.MemoryAdditionalOffsetDictionary["LogCounterEnd"][1];
            _logCounterEndPointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[2] = Globals.Instance.MemoryAdditionalOffsetDictionary["LogStart"][1];
            _logStartPointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[2] = Globals.Instance.MemoryAdditionalOffsetDictionary["LogFirstEmptyByte"][1];
            _logFirstEmptyBytePointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[2] = Globals.Instance.MemoryAdditionalOffsetDictionary["LogEnd"][1];
            _logEndPointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);
        }
    }
}
