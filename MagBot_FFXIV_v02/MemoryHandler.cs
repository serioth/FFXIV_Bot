using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace MagBot_FFXIV_v02
{
    //Factory design pattern: Get method returns an instance of its class, based on the input string
    public class ProcessInfo
    {
        private readonly string _processName = "";

        public string Name
        {
            get { return _processName; }
        }

        private readonly int _processId = -1;

        public int Id
        {
            get { return _processId; }
        }

        private ProcessInfo(string name, int id)
        {
            _processId = id;
            _processName = name;
        }

        public static bool Get(string name, out ProcessInfo[] info)
        {
            Process[] processList = Process.GetProcessesByName(name);
            //Creates array that will be filled with instances of ProcessInfo (returned through parameter)
            info = new ProcessInfo[processList.Length];

            for (var i = 0; i < processList.Length; i++)
            {
                info[i] = new ProcessInfo(name, processList[i].Id); //Fill each Array slot with a ProcessInfo object
            }
            return true;
        }
    }

    public class MemoryHandler : IDisposable
    {
        //Thread-safe Singleton design pattern
        private static readonly MemoryHandler MemoryHandlerInstance = new MemoryHandler();

        static MemoryHandler()
        {
        }

        private MemoryHandler()
        {
        }

        public static MemoryHandler Instance
        {
            get { return MemoryHandlerInstance; }
        }

        // Define the delegate that is used to report problems
        public enum MemoryResult
        {
            InvalidArguments = -2,
            Other = -1,
            Success = 0,
            PartialCopy = 299,
        }

        public delegate void ErrorHandler(MemoryResult lastError, UInt32 lastErrorSystemCode, string details);

        private ErrorHandler _errorHandler;
        private MemoryResult _lastMemoryResult = MemoryResult.Success;

        private Process _process;
        private int _processId;
        private IntPtr _processHandle = IntPtr.Zero;
        private IntPtr _processMainWindowHandle;
        private IntPtr _baseAddress;
        private UInt32 _lastErrorSystemCode;

        /// <summary>
        /// The wildcard character for signature scanning
        /// </summary>
        private const int WildcardChar = 63; // the character '?';

        /// <summary>
        /// Used for filtering which regions to scan
        /// </summary>
        private const int MemCommit = 0x1000;

        private const int PageReadwrite = 0x04;
        private const int PageWritecopy = 0x08;
        private const int PageExecuteReadwrite = 0x40;
        private const int PageExecuteWritecopy = 0x80;
        private const int PageGuard = 0x100;

        private const int Writable =
            PageReadwrite | PageWritecopy | PageExecuteReadwrite | PageExecuteWritecopy | PageGuard;

        /// <summary>
        /// Our list of memory regions for this process
        /// </summary>
        private List<MemoryApi.MemoryBasicInformation> MemoryRegions { get; set; }

        private SortedList<Int32, int> _memoryRegionsSortedIndices;

        /// <summary>
        /// Where to return the pointer from
        /// </summary>
        public enum ScanResultType
        {
            /// <summary>
            /// Read in the pointer before the signature
            /// </summary>
            ValueBeforeSig,

            /// <summary>
            /// Read in the pointer after the signature
            /// </summary>
            ValueAfterSig,

            /// <summary>
            /// Read the address at the start of where it found the signature
            /// </summary>
            AddressStartOfSig,

            /// <summary>
            /// Read the value at the start of the wildcard
            /// </summary>
            ValueAtWildCard,
        }

        public bool ProcessIsRunning()
        {
            bool isRunning;
            try
            {
                isRunning = !_process.HasExited;
            }
            catch
            {
                // Must be an issue so act like not running
                isRunning = false;
            }
            return isRunning;
        }

        private void ResetError()
        {
            _lastErrorSystemCode = 0;
            _lastMemoryResult = MemoryResult.Success;
        }

        private void HandleError(string details)
        {
            _lastErrorSystemCode = MemoryApi.GetLastError();
            if (Enum.IsDefined(typeof(MemoryResult), (int)_lastErrorSystemCode))
                _lastMemoryResult = (MemoryResult)_lastErrorSystemCode;
            else
                _lastMemoryResult = MemoryResult.Other;

            if (_errorHandler != null)
            {
                _errorHandler(_lastMemoryResult, _lastErrorSystemCode, details);
            }
        }

        public UInt32 LastErrorSystemCode
        {
            get { return _lastErrorSystemCode; }
        }

        public MemoryResult LastMemoryResult
        {
            get { return _lastMemoryResult; }
        }

        public void Initialize(int processId, ErrorHandler errorHandler)
        {
            _process = null;
            _processId = 0;
            _processMainWindowHandle = IntPtr.Zero;
            _baseAddress = IntPtr.Zero;
            _errorHandler = errorHandler;

            // get a reference to the process
            try
            {
                _process = Process.GetProcessById(processId);
            }
            catch
            {
                _process = null;
                return;
            }

            _processId = _process.Id;
            _processMainWindowHandle = _process.MainWindowHandle;
            _baseAddress = GetBaseAddress();

            //Open the process and load all memory regions of game into the MemoryRegions list
            //Each entry in list is a struct (MemoryBasicInformation)
            _processHandle = MemoryApi.OpenProcess(MemoryApi.ProcessAccessFlags.All, 1, (uint)_processId);
            MemoryRegions = new List<MemoryApi.MemoryBasicInformation>();
            LoadMemoryRegions();
        }

        public void Initialize(string processName, ErrorHandler errorHandler)
        {
            _process = null;
            _processId = 0;
            _processMainWindowHandle = IntPtr.Zero;
            _baseAddress = IntPtr.Zero;
            _errorHandler = errorHandler;
            Process[] processList;

            try
            {
                processList = Process.GetProcessesByName(processName);
            }
            catch
            {
                _process = null;
                return;
            }

            if (processList.Length < 1) return;

            // get a reference to the process
            _process = processList[0];
            _processId = _process.Id;
            _processMainWindowHandle = _process.MainWindowHandle;
            _baseAddress = GetBaseAddress();

            //Open the process and load all memory regions of game into the MemoryRegions list
            //Each entry in list is a struct (MemoryBasicInformation)
            _processHandle = MemoryApi.OpenProcess(MemoryApi.ProcessAccessFlags.All, 1, (uint)_processId);
            MemoryRegions = new List<MemoryApi.MemoryBasicInformation>();
            LoadMemoryRegions();
        }


        //Code to ensure cleanup of unmanaged resources.
        //Destructor. Implicitly calls Finalize on the base class of the object when GC determines the object is no longer needed
        //If your class uses unmanaged resources you can use the destructor to release those properly.
        //Don't assume that GC knows how those are to be released. Do not use empty Destructors, that is meaningless.
        //If it doesn't use managed resources a destructor is rarely ever necessary.

        // Flag: Has Dispose already been called? 
        private bool _disposed;

        // Public implementation of Dispose pattern callable by consumers.
        //If consumer calls it, don't have the GC attempt to clean up the managed resources again (therefore SupressFinalize)
        public void Dispose()
        {
            const string message = "Dispose() in MemoryHandler called by consumer...";
            Globals.Instance.ApplicationLogger.Log(message);
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here. 
                //
            }

            // Free any unmanaged objects here. 
            CloseHandle();
            _disposed = true;
        }

        ~MemoryHandler()
        {
            const string message = "Dispose() in MemoryHandler called in Finalizer/Destructor...";
            Globals.Instance.ApplicationLogger.Log(message);
            Dispose(false);
        }

        public Process Process
        {
            get { return _process; }
        }

        public int ProcessId
        {
            get { return _process.Id; }
        }

        public IntPtr MainWindowHandle
        {
            get { return _processMainWindowHandle; }
        }

        public IntPtr BaseAddress
        {
            get { return _baseAddress; }
        }


        /// <summary>
        /// Will open the process and get the handle for reading. If successful returns true.
        /// </summary>
        public bool OpenProcess(int processId)
        {
            // get reference to the process
            _process = Process.GetProcessById(processId);

            // open the process
            _processHandle = MemoryApi.OpenProcess(MemoryApi.ProcessAccessFlags.All, 1, (uint)processId);

            if (_processHandle == null)
            {
                return false;
            }

            MemoryRegions = new List<MemoryApi.MemoryBasicInformation>();

            LoadMemoryRegions();

            return true;
        }

        /// <summary>
        /// Will close the handle and stop reading the process
        /// </summary>
        public void CloseHandle()
        {
            try
            {
                MemoryApi.CloseHandle(_processHandle);
            }
            catch (SEHException sehEx)
            {
                HandleError("Failed to close handle. Message " + sehEx.Message + "; Message length: " +
                            sehEx.Message.Length.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Gets the base address of a specified module/DLL name.
        /// </summary>
        /// <param name="moduleName">If null or zero length then returns base address of main module.</param>
        /// <returns>IntPtr.Zero on error.</returns>
        public IntPtr GetBaseAddress(string moduleName = null)
        {
            var baseAddress = IntPtr.Zero;
            ProcessModuleCollection modules = _process.Modules;

            if (string.IsNullOrEmpty(moduleName))
            {
                // Caller wants lowest address so no specific address given
                baseAddress = _process.MainModule.BaseAddress;
            }
            else
            {
                foreach (ProcessModule i in modules)
                {
                    if (i.ModuleName != moduleName) continue;
                    baseAddress = i.BaseAddress;
                    break;
                }
            }
            return baseAddress;
        }

        public IntPtr GetPointerFromOffsets(int[] offsets)
        {
            //If only one member in int-array, then loop is not run and we return the LvlOnePointer
            //Else, need to loop through offsets, read value, update pointer, and repeate until all offsets are done
            IntPtr thePointer = GetLvlOneAddressFromBaseOffset(offsets[0]);
            for (var x = 1; x < offsets.Length; x++)
            {
                int valueAtAddress;
                ReadInt(thePointer, out valueAtAddress);
                thePointer = new IntPtr(valueAtAddress + offsets[x]);
            }
            return thePointer;
        }

        public IntPtr GetLvlOneAddressFromBaseOffset(int offset)
        {
            return IntPtr.Add(BaseAddress, offset);
        }

        public void VirtualQuery()
        {
            const long maxAddress = 0x7fffffff;
            long address = 0;
            do
            {
                MemoryApi.MemoryBasicInformation m;
                int result = MemoryApi.VirtualQueryEx(Process.GetCurrentProcess().Handle, (IntPtr)address, out m,
                    (uint)Marshal.SizeOf(typeof(MemoryApi.MemoryBasicInformation)));
                Console.WriteLine(@"{0}-{1} : {2} bytes result={3}", m.BaseAddress,
                    (uint)m.BaseAddress + m.RegionSize - 1, m.RegionSize, result);
                if (address == (long)m.BaseAddress + m.RegionSize)
                    break;
                address = (long)m.BaseAddress + m.RegionSize;
            } while (address <= maxAddress);
        }

        public void SwitchWindow()
        {
            if (MemoryApi.GetForegroundWindow() == MainWindowHandle)
                return;

            IntPtr foregroundWindowHandle = MemoryApi.GetForegroundWindow();
            uint currentThreadId = MemoryApi.GetCurrentThreadId();
            uint temp;
            uint foregroundThreadId = MemoryApi.GetWindowThreadProcessId(foregroundWindowHandle, out temp);
            MemoryApi.AttachThreadInput(currentThreadId, foregroundThreadId, true);
            MemoryApi.SetForegroundWindow(MainWindowHandle);
            MemoryApi.AttachThreadInput(currentThreadId, foregroundThreadId, false);

            while (MemoryApi.GetForegroundWindow() != MainWindowHandle)
            {
            }
        }

        public int MakeLong(int low, int high)
        {
            return (high << 16) | (low & 0xffff);
        }

        public MemoryResult ReadMemoryToInt(IntPtr memoryAddress, int bytesToRead, out int outInt)
        {
            ResetError();
            int inSize = bytesToRead;
            outInt = 0;
            var result = MemoryResult.Success;

            if (inSize <= 0) inSize = 1;
            else if (inSize > 4) inSize = 4;
            if (memoryAddress == IntPtr.Zero) return MemoryResult.InvalidArguments;

            switch (inSize)
            {
                case 1:
                    byte outByte;
                    result = ReadByte(memoryAddress, out outByte);
                    if (result != MemoryResult.Success)
                        HandleError("ReadMemoryToInt 1 - Address " + memoryAddress.ToString("X") + "; Read: " + outByte);
                    outInt = outByte;
                    break;
                case 2:
                    short outShort;
                    result = ReadShort(memoryAddress, out outShort);
                    if (result != MemoryResult.Success)
                        HandleError("ReadMemoryToInt 2 - Address " + memoryAddress.ToString("X") + "; Read: " + outShort);
                    outInt = outShort;
                    break;
                case 3:
                    int readCount;
                    byte[] buffer;
                    result = ReadMemory(memoryAddress, inSize, out readCount, out buffer);
                    if (result != MemoryResult.Success)
                    {
                        HandleError("ReadMemoryToInt 3 - Address " + memoryAddress.ToString("X") + "; Size: " + inSize +
                                    "; Read: " + readCount);
                        break;
                    }
                    if (BitConverter.IsLittleEndian)
                    {
                        // Order in array is MSB, middle byte, LSB
                        outInt = buffer[0] + buffer[1] << 8 + buffer[2] << 16;
                    }
                    else
                    {
                        // Order in array is LSB, middle byte, MSB
                        outInt = buffer[2] + buffer[1] << 8 + buffer[0] << 16;
                    }
                    break;
                case 4:
                    result = ReadInt(memoryAddress, out outInt);
                    if (result != MemoryResult.Success)
                        HandleError("ReadMemoryToInt 4 - Address " + memoryAddress.ToString("X") + "; Read: " + outInt);
                    break;
            }
            return result;
        }

        /// <summary>
        /// Reads a pre-specified number of bytes and returns number of bytes read aas well as the values read, through its parameters
        /// </summary>
        public MemoryResult ReadMemory(IntPtr memoryAddress, int bytesToRead, out int bytesRead,
            out byte[] outByteArray)
        {
            ResetError();
            var buffer = new byte[bytesToRead];

            if (!MemoryApi.ReadProcessMemory(_processHandle, memoryAddress, buffer, bytesToRead, out bytesRead)
                || bytesToRead != bytesRead)
                HandleError("ReadMemory Array - Address " + memoryAddress.ToString("X") + "; Size: " + bytesToRead +
                            "; Read: " + bytesRead);

            outByteArray = buffer;
            return LastMemoryResult;
        }

        public MemoryResult ReadMemory(IntPtr memoryAddress, int bytesToRead, ref byte[] outBuffer, out int bytesRead)
        {
            ResetError();
            bytesRead = 0;
            if (outBuffer == null || bytesToRead < 0) return MemoryResult.InvalidArguments;

            int workingReadCount = Math.Min(outBuffer.Length, bytesToRead);

            if (!MemoryApi.ReadProcessMemory(_processHandle, memoryAddress, outBuffer, workingReadCount, out bytesRead)
                || bytesToRead != bytesRead)
                HandleError("ReadMemory Buffer - Address " + memoryAddress.ToString("X") + "; Size: " + workingReadCount +
                            "; Read: " + bytesRead);

            return LastMemoryResult;
        }

        public MemoryResult ReadInt(IntPtr memoryAddress, out int outInt)
        {
            ResetError();
            var buffer = new byte[sizeof(int)];

            int ptrBytesRead; // error checking
            if (!MemoryApi.ReadProcessMemory(_processHandle, memoryAddress, buffer, sizeof(int), out ptrBytesRead)
                || sizeof(int) != ptrBytesRead)
                HandleError("ReadInt - Address " + memoryAddress.ToString("X") + "; Read: " + ptrBytesRead);

            outInt = BitConverter.ToInt32(buffer, 0);
            return LastMemoryResult;
        }

        public MemoryResult ReadFloat(IntPtr memoryAddress, out float outFloat)
        {
            ResetError();
            var buffer = new byte[sizeof(float)];

            int ptrBytesRead; // error checking
            if (!MemoryApi.ReadProcessMemory(_processHandle, memoryAddress, buffer, sizeof(float), out ptrBytesRead)
                || sizeof(int) != ptrBytesRead)
                HandleError("ReadFloat - Address " + memoryAddress.ToString("X") + "; Read: " + ptrBytesRead);

            outFloat = BitConverter.ToSingle(buffer, 0);
            return LastMemoryResult;
        }

        public MemoryResult ReadShort(IntPtr memoryAddress, out short outShort)
        {
            ResetError();
            var buffer = new byte[2];

            int ptrBytesRead; // error checking
            if (!MemoryApi.ReadProcessMemory(_processHandle, memoryAddress, buffer, sizeof(short), out ptrBytesRead)
                || sizeof(short) != ptrBytesRead)
                HandleError("ReadShort - Address " + memoryAddress.ToString("X") + "; Read: " + ptrBytesRead);

            outShort = BitConverter.ToInt16(buffer, 0);
            return LastMemoryResult;
        }

        public MemoryResult ReadByte(IntPtr memoryAddress, out byte outByte)
        {
            ResetError();
            var buffer = new byte[1];

            int ptrBytesRead; // error checking
            if (!MemoryApi.ReadProcessMemory(_processHandle, memoryAddress, buffer, sizeof(byte), out ptrBytesRead)
                || sizeof(byte) != ptrBytesRead)
                HandleError("ReadByte - Address " + memoryAddress.ToString("X") + "; Read: " + ptrBytesRead);

            outByte = buffer[0];
            return LastMemoryResult;
        }

        public MemoryResult ReadStruct<T>(IntPtr address, out T outStruct)
        {
            ResetError();
            var cnt = 0;
            byte[] buffer;
            MemoryResult result = ReadMemory(address, Marshal.SizeOf(typeof(T)), out cnt, out buffer);
            // Will call error handler if needed so we do not have to

            if (cnt > 0)
            {
                GCHandle pinned = GCHandle.Alloc(buffer, GCHandleType.Pinned);

                try
                {
                    outStruct = (T)Marshal.PtrToStructure(pinned.AddrOfPinnedObject(), typeof(T));
                    return result;
#if DEBUG
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ReadStruct: Unable to coerce foreign data into <" + typeof(T).ToString() + ">: " +
                                    ex.Message);
#else
         } catch {
#endif
                }
                finally
                {
                    pinned.Free();
                }
            }

            outStruct = default(T);
            return result;
        }

        public MemoryResult ReadArray<T>(IntPtr address, int count, out T[] outArray)
        {
            ResetError();
            if (count <= 0)
            {
                outArray = default(T[]);
                return LastMemoryResult;
            }

            //Read in the number of bytes required for each array index
            int cnt;
            byte[] buffer;
            MemoryResult result = ReadMemory(address, Marshal.SizeOf(typeof(T)) * count, out cnt, out buffer);
            // Calls error handler if needed so we do not have to here

            if (cnt > 0)
            {
                GCHandle pinned = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    //Read in the structure at each position and coerce it into its slot
                    var output = new T[count];
                    IntPtr current = pinned.AddrOfPinnedObject();

                    for (var i = 0; i < count; i++)
                    {
                        output[i] = (T)Marshal.PtrToStructure(current, typeof(T));
                        current = (IntPtr)((int)current + Marshal.SizeOf(output[i])); //advance to the next index
                    } // @ for (int i = 0; i < Count; i++)

                    outArray = output;
                    return result;
#if DEBUG
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ReadStruct: Unable to coerce foreign data into <" + typeof(T).ToString() + ">: " +
                                    ex.Message);
#else
                } catch {
#endif
                }
                finally
                {
                    pinned.Free();
                }
            }
            outArray = default(T[]);
            return result;
        }

        public MemoryResult ReadString(IntPtr address, int maxLen, out string outString)
        {
            ResetError();
            // read the memory into a buffer
            int cnt;
            byte[] buffer;
            //Console.WriteLine(@"String Length: " + maxLen);
            MemoryResult result = ReadMemory(address, maxLen, out cnt, out buffer);
            // Calls error handler if needed so we do not have to here

            if (cnt > 0)
            {
                GCHandle pinned = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    outString = Marshal.PtrToStringAnsi(pinned.AddrOfPinnedObject());
                    return result;
#if DEBUG
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ReadString: Unable to coerce foreign data into string: " + ex.Message);
#else
         } catch {
#endif
                }
                finally
                {
                    pinned.Free();
                }
            }
            outString = String.Empty;
            return result;
        }

        public MemoryResult WriteProcessMemory(IntPtr memoryAddress, byte[] bytesToWrite, out int bytesWritten)
        {
            ResetError();
            if (
                MemoryApi.WriteProcessMemory(_processHandle, memoryAddress, bytesToWrite, (uint)bytesToWrite.Length,
                    out bytesWritten) == 0)
                HandleError("WriteProcessMemory - Address " + memoryAddress.ToString("X") + "; Written: " + bytesWritten);
            return LastMemoryResult;
        }

        /// <summary>
        //SENDING COMMANDS, METHOD 1: SendInput()
        //Had to use ScanCodes as opposed to sending with wVk, as in games the commands are input'ed with Direct 3D (direct input)
        //The below allows for sending both a text character or a virtual key code
        //Note: When you hold a key down, the keyboard actually sends keydown,keydown,keydown,keydown,... and keyup when you finally release it. The exception to this is CTRL, SHIFT and ALT which don't repeat
        /// </summary>
        public static void HumanKeyPress(MemoryApi.KeyCode[] vk)
        {
            foreach (var k in vk)
            {
                PressKey(k, true);
            }
            Thread.Sleep(Utils.getRandom(100, 300));
            foreach (var k in vk)
            {
                PressKey(k, false);
            }
        }

        public static void HumanKeyPress(MemoryApi.KeyCode vk)
        {
            PressKey(vk, true);
            Thread.Sleep(Utils.getRandom(100, 300));
            PressKey(vk, false);
        }

        public static void PressKey(char ch, bool press)
        {
            ushort vk = MemoryApi.VkKeyScan(ch);
            PressKey((MemoryApi.KeyCode)vk, press);
        }

        public static void PressKey(MemoryApi.KeyCode vk, bool press)
        {
            var lowOrderByte = (ushort)((ushort)vk & 0xff);
            var scanCode = (ushort)MemoryApi.MapVirtualKey(lowOrderByte, 0);

            //Console.WriteLine("SendInput:: VK: " + (ushort)vk + " (" + vk + ") <-> SC: " + (ushort)(scanCode & 0xff));

            if (press)
                KeyDown(scanCode);
            else
                KeyUp(scanCode);
        }

        private static void KeyDown(ushort scanCode)
        {
            //Console.WriteLine("Key Down (SC): " + (ushort)(scanCode & 0xff));
            var inputs = new MemoryApi.Input[1];

            inputs[0].type = MemoryApi.INPUT_KEYBOARD;
            inputs[0].ki.wScan = scanCode;
            inputs[0].ki.dwFlags = MemoryApi.KEYEVENTF_SCANCODE;
            inputs[0].ki.time = 0;
            inputs[0].ki.dwExtraInfo = IntPtr.Zero;

            uint intReturn = MemoryApi.SendInput(1, inputs, Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key: " + scanCode);
            }
        }

        private static void KeyUp(ushort scanCode)
        {
            //Console.WriteLine("Key Up (SC): " + scanCode);
            var inputs = new MemoryApi.Input[1];

            inputs[0].type = MemoryApi.INPUT_KEYBOARD;
            inputs[0].ki.wScan = (ushort)(scanCode & 0xff);
            inputs[0].ki.dwFlags = MemoryApi.KEYEVENTF_SCANCODE | MemoryApi.KEYEVENTF_KEYUP;
            inputs[0].ki.time = 0;
            inputs[0].ki.dwExtraInfo = IntPtr.Zero;

            uint intReturn = MemoryApi.SendInput(1, inputs, Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key: " + scanCode);
            }
        }

        public static bool IsKeyPushedDown(MemoryApi.KeyCode vKey)
        {
            return 0 != (MemoryApi.GetAsyncKeyState((int)vKey) & 0x8000);
        }

        private static bool GetKeyboardState(byte[] keyStates)
        {
            if (keyStates == null)
                throw new ArgumentNullException("keyStates");
            if (keyStates.Length != 256)
                throw new ArgumentException(@"The buffer must be 256 bytes long.", "keyStates");
            return MemoryApi.NativeGetKeyboardState(keyStates);
        }

        private static IEnumerable<byte> GetKeyboardState()
        {
            var keyStates = new byte[256];
            if (!GetKeyboardState(keyStates))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return keyStates;
        }

        public static bool AnyKeyPressed()
        {
            IEnumerable<byte> keyState = GetKeyboardState();
            // skip the mouse buttons
            return keyState.Skip(8).Any(state => (state & 0x80) != 0);
        }

        public static bool IsKeyLocked(MemoryApi.KeyCode vk)
        {
            return (((ushort)MemoryApi.GetKeyState((int)vk)) & 0xffff) != 0;
        }

        /// <summary>
        /// Will load all the memory region information of the program into a list for faster scanning
        /// </summary>
        private void LoadMemoryRegions()
        {
            // the current address being scanned
            var address = new IntPtr();

            _memoryRegionsSortedIndices = new SortedList<Int32, int>();
            var count = 0;

            while (true)
            {
                // get the memory information for the first region
                var memInfo = new MemoryApi.MemoryBasicInformation();
                int result = MemoryApi.VirtualQueryEx(_process.Handle, address, out memInfo,
                    (uint)Marshal.SizeOf(memInfo));

                // virtualqueryex will return 0 when we're out of range of the application
                if (0 == result)
                    break;

                // filter out any that don't have commit or aren't writable
                if (0 != (memInfo.State & MemCommit) && 0 != (memInfo.Protect & Writable) &&
                    0 == (memInfo.Protect & PageGuard))
                {
                    // store the information
                    MemoryRegions.Add(memInfo);
                    _memoryRegionsSortedIndices.Add(memInfo.BaseAddress.ToInt32(), count++);
                }

                // move to the next memory region
                address = new IntPtr(memInfo.BaseAddress.ToInt32() + memInfo.RegionSize);
            }
        }

        /// <summary>
        /// Searches the loaded process for the given byte signature
        /// </summary>
        /// <param name="signature">The hex pattern to search for</param>
        /// <param name="searchType">What type of result to return</param>
        /// <returns>The pointer found at the matching location</returns>
        public IntPtr FindSignature(string signature, ScanResultType searchType)
        {
            return FindSignature(signature, 0, searchType);
        }

        /// <summary>
        /// <para>Searches the loaded process for the given byte signature.</para>
        /// <para>Uses the character ? as a wildcard</para>
        /// </summary>
        /// <param name="signature">The hex pattern to search for</param>
        /// <param name="startAddress">An startAddress to add to the pointer VALUE</param>
        /// <param name="searchType">What type of result to return</param>
        /// <returns>The pointer found at the matching location</returns>
        public IntPtr FindSignature(string signature, int startAddress, ScanResultType searchType)
        {
            // make sure we have a valid signature
            if (signature.Length == 0 || signature.Length % 2 != 0)
                throw new Exception("FindSignature(): Invalid signature");

            for (var index = 0; index < _memoryRegionsSortedIndices.Count; index++)
            {
                int workingIndex = _memoryRegionsSortedIndices.Values[index];

                MemoryApi.MemoryBasicInformation region = MemoryRegions[_memoryRegionsSortedIndices.Values[index]];

                // Skip memory regions until we find one that contains the startAddress but startAddress of zero means skip none
                if (startAddress > 0)
                {
                    if (region.BaseAddress.ToInt32() + region.RegionSize < startAddress ||
                        region.BaseAddress.ToInt32() > startAddress)
                        continue;
                }

                var buffer = new byte[region.RegionSize];
                var bytesRead = 0;

                // ReadProcessMemory will return 0 if some form of error occurs
                if (
                    !MemoryApi.ReadProcessMemory(_process.Handle, region.BaseAddress, buffer, (int)region.RegionSize,
                        out bytesRead))
                {
                    // get the error code thrown from ReadProcessMemory
                    int errorCode = Marshal.GetLastWin32Error();

                    // For now, if error reading, we will still search what amount was able to be read so no exception throwing.
                }

                var bufferOffset = 0;
                if (startAddress > 0 && region.BaseAddress.ToInt32() < startAddress &&
                    region.BaseAddress.ToInt32() + bytesRead > startAddress)
                {
                    // Since requested startAddress is somewhere in the current regions address space, set startAddress from beginning of region
                    bufferOffset = startAddress - region.BaseAddress.ToInt32();
                }
                IntPtr searchResult = FindSignature(buffer, signature, bufferOffset, searchType);

                // If we found our signature, we're done
                if (IntPtr.Zero == searchResult) continue;

                // if we passed the ! flag we want the beginning address of where it found the sig
                if (ScanResultType.AddressStartOfSig == searchType)
                    searchResult = new IntPtr(region.BaseAddress.ToInt32() + searchResult.ToInt32());

                return searchResult;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Searches the buffer for the given hex string and returns the pointer matching the first wildcard location, or the pointer following the pattern if not using wildcards.
        /// Prefix with &lt;&lt; to always return the pointer preceding the match or &gt;&gt; to always return the pointer following (regardless of wildcards)
        /// </summary>
        /// <param name="buffer">The source binary buffer to search within</param>
        /// <param name="signature">A hex string representation of a sequence of bytes to search for</param>
        /// <param name="offset">An offset to where in buffer to start search for pattern.</param>
        /// <param name="searchType">Search before, at or after wildcard</param>
        /// <returns>A pointer at the matching location</returns>
        private static IntPtr FindSignature(byte[] buffer, string signature, int offset, ScanResultType searchType)
        {
            // Since this is a hex string make sure the characters are entered in pairs.
            if (signature.Length == 0 || signature.Length % 2 != 0) return IntPtr.Zero;

            // Convert the signature text to a binary array
            byte[] pattern = SigToByte(signature, WildcardChar);

            if (pattern == null) return IntPtr.Zero;

            //Find the start index of the first wildcard. if no wildcards then the bytes following the match
            var pos = 0;
            for (pos = 0; pos < pattern.Length; pos++)
            {
                if (pattern[pos] == WildcardChar) break;
            }

            //Search for the pattern in the buffer. Convert the bytes to an int and return as a pointer
            //If not using wildcards then use the faster Horspool algorithm
            var idx = -1;
            idx = pos == pattern.Length
                ? Horspool(buffer, pattern, offset)
                : BNDM(buffer, pattern, WildcardChar, offset);

            // If the sig was not found then exit
            if (idx < 0) return IntPtr.Zero;

            // Grab the 4 byte pointer at the location requested
            switch (searchType)
            {
                case ScanResultType.ValueBeforeSig:
                    //always grab the pointer in front of the sig
                    return (IntPtr)(BitConverter.ToInt32(buffer, idx - 4));
                case ScanResultType.ValueAfterSig:
                    // always grab the pointer following the sig
                    return (IntPtr)(BitConverter.ToInt32(buffer, idx + pattern.Length));
                case ScanResultType.AddressStartOfSig:
                    // return the address at the end of where the signature was found
                    return (IntPtr)(idx);
                case ScanResultType.ValueAtWildCard:
                default:
                    // always pointer starting at the first wildcard. if no wildcard is being used, then the rear
                    return (IntPtr)(BitConverter.ToInt32(buffer, idx + pos));
            }
        }

        /// <summary>Backward Nondeterministic Dawg Matching search algorithm</summary>
        /// <param name="buffer">The haystack to search within</param>
        /// <param name="pattern">The needle to locate</param>
        /// <param name="wildcard">The byte to treat as a wildcard character. Note that this only matches one char for one char and does not expand.</param>
        /// <param name="offset">The startAddress into the buffer to begin searching for the pattern.</param>
        /// <returns>The index the pattern was found at, or -1 if not found</returns>
        private static int BNDM(byte[] buffer, byte[] pattern, byte wildcard, int offset = 0)
        {
            // This code is based on: 
            // http://johannburkard.de/software/stringsearch/
            // http://www-igm.univ-mlv.fr/~lecroq/string/bndm.html

            var end = pattern.Length < 32 ? pattern.Length : 32;
            var b = new int[256];

            // Pre-process
            var j = 0;
            for (var i = 0; i < end; ++i)
            {
                if (pattern[i] == wildcard) j |= (1 << end - i - 1);
            }

            if (j != 0)
            {
                for (var i = 0; i < b.Length; i++) b[i] = j;
            }

            j = 1;
            for (var i = end - 1; i >= 0; --i, j <<= 1) b[pattern[i]] |= j;

            // Perform search
            var pos = offset;
            while (pos <= buffer.Length - pattern.Length)
            {
                j = pattern.Length - 1;
                int last = pattern.Length;
                var d = -1;

                while (d != 0)
                {
                    d &= b[buffer[pos + j]];

                    if (d != 0)
                    {
                        if (j == 0) return pos;

                        last = j;
                    }

                    --j;
                    d <<= 1;
                }
                pos += last;
            }
            return -1;
        }

        /// <summary>Boyer-Moore-Horspool search algorithm</summary>
        /// <param name="buffer">The haystack to search within</param>
        /// <param name="pattern">The needle to locate</param>
        /// <param name="offset">The startAddress into the buffer to begin searching for the pattern.</param>
        /// <returns>The index the pattern was found at, or -1 if not found</returns>
        private static int Horspool(byte[] buffer, byte[] pattern, int offset = 0)
        {
            // Based on: http://www-igm.univ-mlv.fr/~lecroq/string/node18.html

            var bcs = new int[256];
            int scan;

            // Build the Bad Char Skip table
            for (scan = 0; scan < 256; scan = scan + 1)
                bcs[scan] = pattern.Length;

            int last = pattern.Length - 1;

            for (scan = 0; scan < last; scan = scan + 1)
                bcs[pattern[scan]] = last - scan;

            // perform string matching
            int hidx = offset;
            int hlen = buffer.Length;
            int nlen = pattern.Length;

            while (hidx <= hlen - nlen)
            {
                for (scan = last; buffer[hidx + scan] == pattern[scan]; scan = scan - 1)
                {
                    if (scan == 0) return hidx;
                }
                hidx += bcs[buffer[hidx + last]];
            }
            return -1;
        }

        private static readonly int[] HexTable =
        {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
            0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
        };

        /// <summary>
        /// Convert a hex string to a binary array while preserving any wildcard characters.
        /// </summary>
        /// <param name="signature">A hex string "signature"</param>
        /// <param name="wildcard">The byte to treat as the wildcard</param>
        /// <returns>The converted binary array. Null if the conversion failed.</returns>
        private static byte[] SigToByte(string signature, byte wildcard)
        {
            var pattern = new byte[signature.Length / 2];
            try
            {
                for (int x = 0, i = 0; i < signature.Length; i += 2, x += 1)
                {
                    if (signature[i] == wildcard) pattern[x] = wildcard;
                    else
                        pattern[x] =
                            (byte)
                                (HexTable[Char.ToUpper(signature[i]) - '0'] << 4 |
                                 HexTable[Char.ToUpper(signature[i + 1]) - '0']);
                }
                return pattern;
            }
            catch
            {
                return null;
            }
        }
    }
}