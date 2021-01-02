using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VolumeAppTest
{
    public sealed class WindowsHooks
    {
        private WinEventDelegate dEvent;
        private List<IntPtr> HookPointers = new List<IntPtr>();

        public WindowsHooks()
        {
            dEvent = this.WinEvent;
            SetHook(EVENT_SYSTEM_FOREGROUND);
            SetHook(EVENT_OBJECT_NAMECHANGE);
        }

        private void SetHook(uint eventCode)
        {
            IntPtr pHook = SetWinEventHook(eventCode, eventCode, IntPtr.Zero, dEvent, 0, 0, WINEVENT_OUTOFCONTEXT);
            if (IntPtr.Zero.Equals(pHook))
                throw new System.ComponentModel.Win32Exception();
            HookPointers.Add(pHook);
        }

        private void WinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject,
                              int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            try
            {
                switch (eventType)
                {
                    case EVENT_SYSTEM_FOREGROUND:
                    case EVENT_OBJECT_NAMECHANGE:
                        // DO THINGS
                        break;
                }
            }
            catch (Exception ex)
            {
                string message = $"{ex.Message}\r\n({ex.GetType()})\r\n{ex.StackTrace}";
                MessageBox.Show(message, "Vocola Internal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Stop()
        {
            for (int i = 0; i < HookPointers.Count; i++)
            {
                if (HookPointers[i] != IntPtr.Zero)
                    UnhookWinEvent(HookPointers[i]);
                HookPointers[i] = IntPtr.Zero;
            }
            dEvent = null;
        }

        ~WindowsHooks()
        {
            Stop();
        }

        #region Windows API

        private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        private const uint EVENT_OBJECT_NAMECHANGE = 0x800C;
        private const uint WINEVENT_OUTOFCONTEXT = 0x0000;

        private delegate void WinEventDelegate(
            IntPtr hWinEventHook,
            uint eventType,
            IntPtr hWnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime);

        [DllImport("User32.dll", SetLastError = true)]
        private static extern IntPtr SetWinEventHook(
            uint eventMin,
            uint eventMax,
            IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc,
            uint idProcess,
            uint idThread,
            uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(
            IntPtr hWinEventHook
            );

        #endregion
    }
}
