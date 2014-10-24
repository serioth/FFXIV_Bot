using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace MagBot_FFXIV_v02
{
    public class KeySender
    {
        // Need to specify the lParam as UIntPtr due to MSB of 32-bit value being 1 and as a signed IntPtr overflow occurs.
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, UIntPtr lParam); // If sends successfully then return value == 0
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam); // If sends successfully then return value == 0
        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);
        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        private const uint MvkVkToChar = 2; // Virtual key code to char
        private const uint MvkVkToVsc = 0; // Virtual key code to virtual scan code
        private const uint MvkVscToVk = 1; // Virtual scan code to virtual key code extended
        private const uint MvkVscToVkEx = 3; // Virtual scan code to virtual key code extended (differentiates left/right version of keys)

        private const uint WmChar = 0x102;
        private const uint WmKeyDown = 0x100;
        private const uint WmKeyUp = 0x101;
        private const uint WmSysKeyDown = 0x104;
        private const uint WmSysKeyUp = 0x105;

        private const uint WmLParamMaskExtendedKey = 0x01000000;
        private const uint WmLParamMaskKeyDownSingle = 0x00000001; // Still must mask in scan code (bits 16-23) and extended flag (bit 24) as necessary
        private const uint WmLParamMaskKeyDownRepeat = 0x40000001; // Still must mask in scan code (bits 16-23) and extended flag (bit 24) as necessary
        private const uint WmLParamMaskKeyUpSingle = 0xC0000001; // Still must mask in scan code (bits 16-23) and extended flag (bit 24) as necessary
        private const uint WmLParamMaskSysKeyDownSingle = 0x20000001; // Still must mask in scan code (bits 16-23) and extended flag (bit 24) as necessary
        private const uint WmLParamMaskSysKeyDownRepeat = 0x60000001; // Still must mask in scan code (bits 16-23) and extended flag (bit 24) as necessary
        private const uint WmLParamMaskSysKeyUpSingle = 0xE0000001;  // Still must mask in scan code (bits 16-23) and extended flag (bit 24) as necessary

        private const uint VkKeyScanCodeMask = 0x00FF;
        private const uint VkKeyScanCodeShiftedMask = 0x0100;

        private const uint WmLparamScancodeUpArrow = 0x48;
        private const uint WmLparamScancodeDownArrow = 0x50;
        private const uint WmLparamScancodeLeftArrow = 0x4B;
        private const uint WmLparamScancodeRightArrow = 0x4D;

        public const byte VkKeyShiftStateMaskShift = 1; // Control is bit 2 while alt is bit 3, but we do not care for now
        public const byte VkKeyShiftStateMaskControl = 2;
        public const byte VkKeyShiftStateMaskAlt = 4;

        public const short VkShift = 0x10;
        public const short VkControl = 0x11;
        public const short VkMenu = 0x12;

        public const int ModifierNone = 0x00;
        public const int ModifierShift = 0x01;
        public const int ModifierControl = 0x02;
        public const int ModifierAlt = 0x04;
        public const int ModifierRepeat = 0x08;

        readonly IntPtr _windowHandle;
        public KeySender(IntPtr sendToWindowHandle)
        {
            _windowHandle = sendToWindowHandle;
        }
        public bool Send(Keys key, int modifiers = ModifierNone)
        {
            switch (key)
            {
                case Keys.Down:
                case Keys.Up:
                case Keys.Left:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.Insert:
                case Keys.PageDown:
                case Keys.PageUp:
                case Keys.Enter:
                case Keys.Escape:
                case Keys.Menu:
                case Keys.ControlKey:
                case Keys.Alt:
                    if (!SendDown(key, modifiers)) return false;

                    // Insert a delay so that the auto generated WM_CHAR is inserted in the correct spot
                    Thread.Sleep(Utils.getRandom(100,200));

                    if (!SendUp(key, modifiers)) return false;
                    break;
                default:
                    // If key code is a basic character then we will just send it like a basic character
                    char theChar = toChar(key);
                    if (theChar != 0) return Send(theChar, modifiers);
                    break;
            }
            return true;
        }

        public bool SendKey(Keys key, int modifiers = ModifierNone)
        {
            if (!SendDown(key, modifiers)) return false;

            // Insert a delay so that the auto generated WM_CHAR is inserted in the correct spot
            Thread.Sleep(Utils.getRandom(100, 200));

            if (!SendUp(key, modifiers)) return false;

            return true;
        }

        public bool Send(string characters, int interCharacterDelay)
        {
            char[] chatChars = characters.ToCharArray();
            for (int x = 0; x < chatChars.Length; x++)
            {
                // If we have sent a character before and sending another one, then need to do the inter character delay if provided.
                if (x > 0 && interCharacterDelay > 0) Thread.Sleep(interCharacterDelay);
                Send(chatChars[x]);
            }
            return true;
        }

        private const string DoAsWmCharSet = "`~1234567890-=!@#$%^&*()_+qwertyuiop[]\\asdfghjkl;\'zxcvbnm,./QWERTYUIOP{}|ASDFGHJKL:\"ZXCVBNM<>?";

        public bool Send(char theChar, int modifiers = ModifierNone)
        {
            // If it is normal character or just shifted then send simple like
            if (DoAsWmCharSet.Contains(theChar) && !isAlted(modifiers) && !isControlled(modifiers))
            {
                short vkKey = VkKeyScan(theChar);
                uint lParam = ToWmLParam(theChar, WmChar, isRepeated(modifiers));
                if (!PostMessage(_windowHandle, WmChar, (IntPtr)theChar, (UIntPtr)lParam)) return false;
            }
            else
            {
                if (!SendDown(theChar, modifiers)) return false;

                // Insert a delay so that the auto generated WM_CHAR is inserted in the correct spot
                Thread.Sleep(Utils.getRandom(200, 300));

                if (!SendUp(theChar, modifiers)) return false;
            }
            return true;
        }
        public bool SendUp(Keys key, int modifiers = ModifierNone)
        {
            uint wmMessage = WmKeyUp;

            if (isAlted(modifiers)) wmMessage = WmSysKeyUp;

            // Send the actual key first since we do things in reverase order on the Up processing
            uint lParam = ToWmLParam(key, wmMessage, isRepeated(modifiers));
            if (!PostMessage(_windowHandle, wmMessage, (IntPtr)key, (UIntPtr)lParam)) return false;

            // See if need to send any Ups for modifier keys. This is done in reverse order of the downs.
            char modifierChar;
            uint modifierlParam;
            if (isAlted(modifiers))
            {
                // For some reason the key up is not a WM_SYSKEYUP even though what starts it is a WM_SYSKEYDOWN
                modifierChar = (char)VkMenu;
                modifierlParam = ToWmLParam(modifierChar, WmKeyUp);
                if (!PostMessage(_windowHandle, WmKeyUp, (IntPtr)modifierChar, (UIntPtr)modifierlParam)) return false;
            }
            if (isControlled(modifiers))
            {
                modifierChar = (char)VkControl;
                modifierlParam = ToWmLParam(modifierChar, WmKeyUp);
                if (!PostMessage(_windowHandle, WmKeyUp, (IntPtr)modifierChar, (UIntPtr)modifierlParam)) return false;
            }
            if (isShifted(modifiers))
            {
                modifierChar = (char)VkShift;
                modifierlParam = ToWmLParam(modifierChar, WmKeyUp);
                if (!PostMessage(_windowHandle, WmKeyUp, (IntPtr)modifierChar, (UIntPtr)modifierlParam)) return false;
            }
            return true;
        }
        public bool SendDown(Keys key, int modifiers = ModifierNone)
        {
            // See if need to send any Downs for modifier keys. This is done in reverse order when the Ups are generated.
            uint wmMessage = WmKeyDown;
            char modifierChar;
            uint modifierlParam;
            if (isShifted(modifiers))
            {
                modifierChar = (char)VkShift;
                modifierlParam = ToWmLParam(modifierChar, WmKeyDown);
                if (!PostMessage(_windowHandle, WmKeyDown, (IntPtr)modifierChar, (UIntPtr)modifierlParam)) return false;
            }
            if (isControlled(modifiers))
            {
                modifierChar = (char)VkControl;
                modifierlParam = ToWmLParam(modifierChar, WmKeyDown);
                if (!PostMessage(_windowHandle, WmKeyDown, (IntPtr)modifierChar, (UIntPtr)modifierlParam)) return false;
            }
            if (isAlted(modifiers))
            {
                wmMessage = WmSysKeyDown;
                modifierChar = (char)VkMenu;
                modifierlParam = ToWmLParam(modifierChar, WmSysKeyDown);
                if (!PostMessage(_windowHandle, WmSysKeyDown, (IntPtr)modifierChar, (UIntPtr)modifierlParam)) return false;
            }

            uint lParam = ToWmLParam(key, wmMessage, isRepeated(modifiers));
            if (!PostMessage(_windowHandle, wmMessage, (IntPtr)key, (UIntPtr)lParam)) return false;

            return true;
        }
        public bool SendDown(char theChar, int modifiers = ModifierNone)
        {
            uint wmMessage = WmKeyDown;
            short vkKeyScanCode = VkKeyScan(theChar);

            // See if the key needs shifting
            bool shiftNeeded = (vkKeyScanCode & VkKeyScanCodeShiftedMask) == VkKeyScanCodeShiftedMask;

            // Get just the virtual key code with no shifted status
            var vkKey = (short)(vkKeyScanCode & VkKeyScanCodeMask);

            // See if need to send any Downs for modifier keys. This is done in reverse order when the Ups are generated.
            char modifierChar;
            uint modifierlParam;
            if (isShifted(modifiers) || shiftNeeded)
            {
                modifierChar = (char)VkShift;
                modifierlParam = ToWmLParam(modifierChar, WmKeyDown);
                if (!PostMessage(_windowHandle, WmKeyDown, (IntPtr)modifierChar, (UIntPtr)modifierlParam)) return false;
            }
            if (isControlled(modifiers))
            {
                modifierChar = (char)VkControl;
                modifierlParam = ToWmLParam(modifierChar, WmKeyDown);
                if (!PostMessage(_windowHandle, WmKeyDown, (IntPtr)modifierChar, (UIntPtr)modifierlParam)) return false;
            }
            if (isAlted(modifiers))
            {
                wmMessage = WmSysKeyDown;
                modifierChar = (char)VkMenu;
                modifierlParam = ToWmLParam(modifierChar, WmSysKeyDown);
                if (!PostMessage(_windowHandle, WmSysKeyDown, (IntPtr)modifierChar, (UIntPtr)modifierlParam)) return false;
            }
            uint lParam = ToWmLParam(theChar, wmMessage, isRepeated(modifiers));

            if (modifiers != ModifierNone) Thread.Sleep(Utils.getRandom(200, 300));

            if (!PostMessage(_windowHandle, wmMessage, (IntPtr)vkKey, (UIntPtr)lParam)) return false;

            return true;
        }
        public bool SendUp(char theChar, int modifiers = ModifierNone)
        {
            uint wmMessage = WmKeyUp;

            short vkKeyScanCode = VkKeyScan(theChar);

            // See if shift is needed
            bool shiftNeeded = (vkKeyScanCode & VkKeyScanCodeShiftedMask) == VkKeyScanCodeShiftedMask;

            // Get just te virtual key code
            var vkKey = (short)(vkKeyScanCode & VkKeyScanCodeMask);

            if (isAlted(modifiers)) wmMessage = WmSysKeyUp;

            // Send the actual key first since we do things in reverase order on the Up processing
            uint lParam = ToWmLParam(theChar, wmMessage, isRepeated(modifiers));
            if (!PostMessage(_windowHandle, wmMessage, (IntPtr)vkKey, (UIntPtr)lParam)) return false;

            // See if need to send any Ups for modifier keys. This is done in reverse order of the downs.
            char modifierChar;
            uint modifierlParam;
            if (isAlted(modifiers))
            {
                // For some reason the key up is not a WM_SYSKEYUP even though what starts it is a WM_SYSKEYDOWN
                modifierChar = (char)VkMenu;
                modifierlParam = ToWmLParam(modifierChar, WmKeyUp);
                if (!PostMessage(_windowHandle, WmKeyUp, (IntPtr)modifierChar, (UIntPtr)modifierlParam)) return false;
            }
            if (isControlled(modifiers))
            {
                modifierChar = (char)VkControl;
                modifierlParam = ToWmLParam(modifierChar, WmKeyUp);
                if (!PostMessage(_windowHandle, WmKeyUp, (IntPtr)modifierChar, (UIntPtr)modifierlParam)) return false;
            }
            if (isShifted(modifiers) || shiftNeeded)
            {
                modifierChar = (char)VkShift;
                modifierlParam = ToWmLParam(modifierChar, WmKeyUp);
                if (!PostMessage(_windowHandle, WmKeyUp, (IntPtr)modifierChar, (UIntPtr)modifierlParam)) return false;
            }
            return true;
        }

        private char toChar(Keys key) { return (char)MapVirtualKey((uint)key, MvkVkToChar); }
        public  Keys ToKey(char theChar) { int modifiers; return ToKey(theChar, out modifiers); }

        private Keys ToKey(char theChar, out int modifiers)
        {
            modifiers = 0;

            short vkKey = VkKeyScan(theChar);
            var vkKeyCode = (byte)(vkKey & 0x00FF);
            var vkKeyShiftState = (byte)(vkKey >> 8);

            if ((vkKeyShiftState & VkKeyShiftStateMaskShift) != 0) modifiers |= ModifierShift;
            if ((vkKeyShiftState & VkKeyShiftStateMaskControl) != 0) modifiers |= ModifierControl;
            if ((vkKeyShiftState & VkKeyShiftStateMaskAlt) != 0) modifiers |= ModifierAlt;

            return (Keys)vkKeyCode;
        }

        private bool isShifted(int modifiers) { if ((modifiers & ModifierShift) != 0) return true; return false; }
        private bool isAlted(int modifiers) { if ((modifiers & ModifierAlt) != 0) return true; return false; }
        private bool isControlled(int modifiers) { if ((modifiers & ModifierControl) != 0) return true; return false; }
        private bool isRepeated(int modifiers) { if ((modifiers & ModifierRepeat) != 0) return true; return false; }

        public uint ToWmLParam(char theChar, uint wmMessage, bool isRepeating = false)
        {
            uint lParam = 0;

            if (wmMessage == WmKeyDown)
            {
                lParam = isRepeating ? WmLParamMaskKeyDownRepeat : WmLParamMaskKeyDownSingle;
            }
            else if (wmMessage == WmKeyUp) lParam = WmLParamMaskKeyUpSingle;
            else if (wmMessage == WmSysKeyDown)
            {
                lParam = isRepeating ? WmLParamMaskSysKeyDownRepeat : WmLParamMaskSysKeyDownSingle;
            }
            else if (wmMessage == WmSysKeyUp) lParam = WmLParamMaskSysKeyUpSingle;

            Keys key = ToKey(theChar);
            switch (key)
            {
                case Keys.Down:
                case Keys.Up:
                case Keys.Left:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.Insert:
                case Keys.PageDown:
                case Keys.PageUp:
                    lParam |= WmLParamMaskExtendedKey;
                    break;
                default:
                    // All other keys are as is
                    break;
            }

            short vkKey = VkKeyScan(theChar);

            // Clear the shift state
            vkKey &= (short)VkKeyScanCodeMask;

            uint scanCode = MapVirtualKey((uint)vkKey, MvkVkToVsc);
            lParam |= (scanCode << 16);

            return lParam;
        }

        public uint ToWmLParam(Keys key, uint wmMessage, bool isRepeating = false)
        {
            uint lParam = 0;

            if (wmMessage == WmKeyDown)
            {
                lParam = isRepeating ? WmLParamMaskKeyDownRepeat : WmLParamMaskKeyDownSingle;
            }
            else if (wmMessage == WmSysKeyDown)
            {
                lParam = isRepeating ? WmLParamMaskSysKeyDownRepeat : WmLParamMaskSysKeyDownSingle;
            }
            else if (wmMessage == WmKeyUp) lParam = WmLParamMaskKeyUpSingle;
            else if (wmMessage == WmSysKeyUp) lParam = WmLParamMaskSysKeyUpSingle;
            else return 0;

            // Check if it is an extended key
            switch (key)
            {
                case Keys.Down:
                case Keys.Up:
                case Keys.Left:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.Insert:
                case Keys.PageDown:
                case Keys.PageUp:
                    lParam |= WmLParamMaskExtendedKey;
                    break;
                default:
                    // All other keys are as is
                    break;
            }

            uint scanCode = MapVirtualKey((uint)key, MvkVkToVsc);
            lParam |= (scanCode << 16);

            return lParam;
        }
    }
}
