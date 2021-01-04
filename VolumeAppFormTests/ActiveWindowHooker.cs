using System;
using System.Runtime.InteropServices;
using System.Text;

namespace VolumeAppTest
{
    public class ActiveWindowHooker
    {
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        private WinEventDelegate dele = null;

        private IntPtr hookPtr;

        public ActiveWindowHooker()
        {
            dele = new WinEventDelegate(WinEventProc);
        }

        ~ActiveWindowHooker()
        {
            UnhookWinEvent(hookPtr);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public void Init()
        {
            hookPtr = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            var handler = ActiveWindowChanged;
            if (handler != null)
            {
                handler.Invoke(this, new ActiveWindowChangedEventArgs(GetActiveWindowTitle()));
            }
        }

        public event EventHandler<ActiveWindowChangedEventArgs> ActiveWindowChanged;

        private uint GetActiveWindowProcessId()
        {
            IntPtr handle = GetForegroundWindow();
            GetWindowThreadProcessId(handle, out var pid);

            return pid;
        }

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
    }

    public class ActiveWindowChangedEventArgs : EventArgs
    {
        public ActiveWindowChangedEventArgs(uint processId)
        {
            ProcessId = processId;
        }

        public ActiveWindowChangedEventArgs(string windowTitle)
        {
            WindowTitle = windowTitle;
        }

        public uint ProcessId { get; }

        public string WindowTitle { get; }
    }
}