using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MagBot_FFXIV_v02
{
    internal class Utils
    {
        public static string pluralizeString(string value)
        {
            if (value == null || value.Length < 1) return "";

            string newValue = value;
            if (value.EndsWith("sh") || value.EndsWith("ch") || value.EndsWith("ss") || value.EndsWith("x"))
                newValue += "es";
            else if (!value.EndsWith("s"))
                newValue += "s";
                    // Force all plural so that singles do not add to hash in separate key from multiples.

            return newValue;
        }

        public static bool isHexDigit(char theChar)
        {
            // Digits Ok
            if (Char.IsDigit(theChar)) return true;

            // If not a letter then (and not the digit check fromk above), then cannot be a hex digit
            if (!Char.IsLetter(theChar)) return false;

            // Force to upper case for one logic test
            char asUpper = Char.ToUpper(theChar);

            // If it is a letter then it must be A through F
            if (asUpper < 'A' || asUpper > 'F') return false;

            return true;
        }

        public static Color InvalidFieldBackColor = Color.Tomato;

        public static string[] AimLabels = new string[]
        {"+5", "+4", "+3", "+2", "+1", "0", "-1", "-2", "-3", "-4", "-5"};

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="theList"></param>
        /// <param name="selectLabel">If null do not modify the selected index. If it is empty select first index. Otherwise select matching string.</param>
        /// <returns>Index of selected item or -1 if selected index not modified.</returns>
        public static int setComboBoxList(ComboBox control, string[] theList, string selectLabel)
        {
            int selectedIndex = -1;
            int newIndex;
            control.Items.Clear();
            foreach (string name in theList)
            {
                newIndex = control.Items.Add(name);
                if (!string.IsNullOrEmpty(selectLabel) && name == selectLabel) selectedIndex = newIndex;
            }
            if (selectLabel != null)
            {
                control.Text = selectLabel;
            }
            return control.SelectedIndex;
        }

        public static uint WM_CHAR = 0x102;
        public static uint WM_KEYDOWN = 0x100;
        public static uint WM_KEYUP = 0x101;
        public static uint WM_SYSKEYDOWN = 0x104;
        public static uint WM_SYSKEYUP = 0x105;
        public static uint WM_LPARAM_EXTENDED_KEY_MASK = 0x01000000;

        public static uint WM_LPARAM_SINGLE_KEYDOWN_MASK = 0x00000001;
            // Still must mask in scan code (bits 16-23) and extended flag (bit 24) as necessary

        public static uint WM_LPARAM_REPEAT_KEYDOWN_MASK = 0x40000001;
            // Still must mask in scan code (bits 16-23) and extended flag (bit 24) as necessary

        public static uint WM_LPARAM_SINGLE_KEYUP_MASK = 0xC0000001;
            // Still must mask in scan code (bits 16-23) and extended flag (bit 24) as necessary

        public static uint WM_LPARAM_SINGLE_SYSKEYDOWN_MASK = 0x20000001;
            // Still must mask in scan code (bits 16-23) and extended flag (bit 24) as necessary

        public static uint WM_LPARAM_REPEAT_SYSKEYDOWN_MASK = 0x60000001;
            // Still must mask in scan code (bits 16-23) and extended flag (bit 24) as necessary

        public static uint WM_LPARAM_SINGLE_SYSKEYUP_MASK = 0xE0000001;
            // Still must mask in scan code (bits 16-23) and extended flag (bit 24) as necessary

        public static uint WM_LPARAM_SCANCODE_UP_ARROW = 0x48;
        public static uint WM_LPARAM_SCANCODE_DOWN_ARROW = 0x50;
        public static uint WM_LPARAM_SCANCODE_LEFT_ARROW = 0x4B;
        public static uint WM_LPARAM_SCANCODE_RIGHT_ARROW = 0x4D;

        private const int KEY_UP_DOWN_TRANSITION_DELAY = 50; // milliseconds between the down and up part of a keystroke

        private const int WM_CHAR_MESSAGE_DELAY = 50;
            // delay between sending a repeat key simulating a key down for repeats

        private const int INTER_KEY_REPEAT_DELAY = 50;
            // delay between sending a repeat key simulating a key down for repeats

        private const int INTER_KEY_COMBO_DELAY = 50;
            // milliseconds between individual keys simulated in a multi-key combo

        // Need to specify the lParam as UIntPtr due to MSB of 32-bit value being 1 and as a signed IntPtr overflow occurs.
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, UIntPtr lParam);

        // If sends successfully then return value == 0

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, UIntPtr lParam);

        // If sends successfully then return value == 0

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        public const byte VK_KEY_SHIFTSTATE_BIT_MASK_SHIFT_KEY = 1;
            // Control is bit 2 while alt is bit 3, but we do not care for now

        public static Keys charToKey(char key)
        {
            bool shifted;
            return charToKey(key, out shifted);
        }

        public static Keys charToKey(char key, out bool shiftedKey)
        {
            short vkKey;
            byte vkKeyCode;
            byte vkKeyShiftState;

            vkKey = VkKeyScan(key);
            vkKeyCode = (byte) (vkKey & 0x00FF);
            vkKeyShiftState = (byte) (vkKey >> 8);

            if ((vkKeyShiftState & VK_KEY_SHIFTSTATE_BIT_MASK_SHIFT_KEY) != 0) shiftedKey = true;
            else shiftedKey = false;

            return (Keys) vkKeyCode;
        }

        public static bool ByteArrayToInt(byte[] array, int startIndex, out int value)
        {
            value = 0;

            try
            {
                value = BitConverter.ToInt32(array, startIndex);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static int[] ByteArrayToIntArray(byte[] array, int startIndex)
        {
            int[] values = null;
            int count = (array.Length - startIndex)/sizeof (int);
                // will ignore odd bytes at aend if not full set for an int

            if (count <= 0) count = 0;

            values = new int[count];

            if (!ByteArrayToIntArray(array, startIndex, count, ref values))
                values = new int[0];

            return values;
        }

        public static bool ByteArrayToIntArray(byte[] array, int startIndex, int outMaxCount, ref int[] outArray)
        {
            int workingInt;
            int workingOffset = startIndex;

            if (startIndex < 0 || outMaxCount < 0 || outArray == null || array == null) return false;

            for (int x = 0; x < outMaxCount; x++)
            {
                if (!ByteArrayToInt(array, workingOffset, out workingInt))
                    break;
                outArray[x] = workingInt;
                workingOffset += sizeof (int);
            }
            return true;
        }

        /// <summary>
        /// Forces a result within the boundaries specified.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="boundaryA"></param>
        /// <param name="boundaryB"></param>
        /// <returns>The value or one of the boundaries if value was out of bounds.</returns>
        public static int Bound(int value, int boundaryA, int boundaryB)
        {
            int min = Math.Min(boundaryA, boundaryB);
            int max = Math.Max(boundaryA, boundaryB);
            if (value < min) return min;
            else if (value > max) return max;
            return value;
        }

        public static bool inBounds(int value, int boundaryA, int boundaryB)
        {
            int min = Math.Min(boundaryA, boundaryB);
            int max = Math.Max(boundaryA, boundaryB);
            if (value < min || value > max) return false;
            return true;
        }

        private static Random randomizer = null;

        public static int getRandom(int limitA, int limitB)
        {
            //So two Random-objects aren't initialized. Having only one seed ensures numbers are truly random
            if (randomizer == null)
            {
                randomizer = new Random();
            }

            int min = Math.Min(limitA, limitB);
            int max = Math.Max(limitA, limitB);

            return randomizer.Next(max - min + 1) + min;
        }

        public static string ToHexString(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint) System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex.ToUpper();
        }

        public static string ToHexString(byte[] byteArray)
        {
            string hex = "";
            foreach (byte c in byteArray)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", c);
            }
            return hex.ToUpper();
        }

        public static string ToHexString(uint value)
        {
            return ToHexString(value, false);
        }

        public static string ToHexString(uint value, bool zeroPadToEvenLength)
        {
            string asHex = value.ToString("X");

            // If length is odd, then prepend a '0' so is even length if flagged
            if (zeroPadToEvenLength && (asHex.Length & 1) != 0) return "0" + asHex;
            return asHex.ToUpper();
        }

        /// <summary>
        /// Converts a string of hex characters to an array of bytes.
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns>Null if string null or zero length input string or string has invalid hexadecimal characters.</returns>
        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            // Length tests
            if (string.IsNullOrEmpty(hexString)) return null;

            // Allocate the byte array memory space now - we add 1 before dividing to cover case where string has odd number of nibbles
            byte[] outArray = new byte[(hexString.Length + 1)/2];

            char[] hexChar = hexString.ToUpper().ToCharArray();
            byte curStringChar;
            byte curOutByte = 0;
            byte nibbleValue = 0;
            int outIndex = 0;
            int nibbleCount = 0;
                // 0-based nibble count where nibble 0 & 1 are byte 0, nibble 2 & 3 are byte 1, nibble 4 & 5 are byte 2, etc.
            if (hexString.Length%2 == 1)
                nibbleCount++;
            for (int x = 0; x < hexString.Length; x++)
            {
                curStringChar = (byte) hexChar[x];
                if (curStringChar >= 48 && curStringChar <= 57)
                {
                    // Nibble is a 0 - 9 character.
                    nibbleValue = (byte) (curStringChar - 48);
                }
                else if (curStringChar >= 65 && curStringChar <= 70)
                {
                    // Nibble is an A - F character.
                    nibbleValue = (byte) (curStringChar - 55);
                        // 65 = 'A', but A in hex  = 10 so startAddress by 65 - 10 = 55;
                }
                else
                {
                    // Bad character
                    return null;
                }
                if (nibbleCount%2 == 0)
                {
                    // This is an upper nibble
                    curOutByte = (byte) (nibbleValue << 4);
                }
                else
                {
                    // This is a upper nibble
                    curOutByte += nibbleValue;

                    // Completed a byte
                    outArray[outIndex++] = curOutByte;
                }
                nibbleCount++;
            }
            return outArray;
        }

        public static string HexDump(byte[] bytes)
        {
            return Utils.HexDump(bytes, 0);
        }

        public static string HexDump(byte[] bytes, int offset)
        {
            if (bytes == null) return "<null>";
            int len = bytes.Length;
            StringBuilder result = new StringBuilder(((len + 15)/16)*78);
            char[] chars = new char[78];
            // fill all with blanks
            for (int i = 0; i < 75; i++) chars[i] = ' ';
            chars[76] = '\r';
            chars[77] = '\n';

            int currentOffset;
            for (int i1 = 0; i1 < len; i1 += 16)
            {
                currentOffset = offset + i1;

                chars[0] = HexChar(currentOffset >> 28);
                chars[1] = HexChar(currentOffset >> 24);
                chars[2] = HexChar(currentOffset >> 20);
                chars[3] = HexChar(currentOffset >> 16);
                chars[4] = HexChar(currentOffset >> 12);
                chars[5] = HexChar(currentOffset >> 8);
                chars[6] = HexChar(currentOffset >> 4);
                chars[7] = HexChar(currentOffset >> 0);

                int offset1 = 11;
                int offset2 = 60;

                for (int i2 = 0; i2 < 16; i2++)
                {
                    if (i1 + i2 >= len)
                    {
                        chars[offset1] = ' ';
                        chars[offset1 + 1] = ' ';
                        chars[offset2] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i1 + i2];
                        chars[offset1] = HexChar(b >> 8);
                        chars[offset1 + 1] = HexChar(b);
                        chars[offset2] = (b < 32 ? '·' : (char) b);
                    }
                    offset1 += (i2 == 8 ? 4 : 3);
                    offset2++;
                }
                result.Append(chars);
            }
            return result.ToString();
        }

        private static char HexChar(int value)
        {
            value &= 0xF;
            if (value >= 0 && value <= 9) return (char) ('0' + value);
            else return (char) ('A' + (value - 10));
        }

        public static void changeByteValue(byte oldValue, byte newValue, byte[] byteArray)
        {
            if (byteArray == null) return;
            for (int x = 0; x < byteArray.Length; x++)
            {
                if (byteArray[x] == oldValue) byteArray[x] = newValue;
            }
        }

        public static bool stringContains(string source, string[] checks)
        {
            // Default is case matters
            return stringContains(source, checks, true);
        }

        public static bool stringContains(string source, string[] checks, bool caseMatters)
        {
            string workingSource;
            if (!caseMatters) workingSource = source.ToLower();
            else workingSource = source;

            if (source == null || source.Length < 1 || checks == null || checks.Length < 1) return false;
            for (int x = 0; x < checks.Length; x++)
            {
                if (caseMatters)
                {
                    if (workingSource.Contains(checks[x])) return true;
                }
                else
                {
                    if (workingSource.Contains(checks[x].ToLower())) return true;
                }
            }
            return false;
        }

        public static float toDegrees(double radians)
        {
            return (float) (radians*(180/Math.PI));
        }
    }
}
