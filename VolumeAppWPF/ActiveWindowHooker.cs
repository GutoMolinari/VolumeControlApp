using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VolumeApp
{
    public class ActiveWindowHooker
    {
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private const uint EVENT_SYSTEM_MINIMIZESTART = 22;
        private const uint EVENT_SYSTEM_MINIMIZEEND = 23;

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        private readonly WinEventDelegate winDelegate;
        private IntPtr hookPtr = IntPtr.Zero;

        public event EventHandler<ActiveWindowChangedEventArgs> ActiveWindowChanged;

        public ActiveWindowHooker()
        {
            winDelegate = new WinEventDelegate(WinEventProc);
        }

        ~ActiveWindowHooker()
        {
            Stop();
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            switch (eventType)
            {
                case EVENT_SYSTEM_FOREGROUND:
                case EVENT_SYSTEM_MINIMIZESTART:
                case EVENT_SYSTEM_MINIMIZEEND:
                    Task.Factory.StartNew(() => {
                        ActiveWindowChanged?.Invoke(this, new ActiveWindowChangedEventArgs(hwnd));
                    });
                    break;
            }
        }

        public void Init()
        {
            if (hookPtr != IntPtr.Zero)
                return;

            hookPtr = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_MINIMIZEEND, IntPtr.Zero, winDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
        }
        
        public void Stop()
        {
            if (hookPtr == IntPtr.Zero)
                return;

            UnhookWinEvent(hookPtr);
            hookPtr = IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
    }

    public class ActiveWindowChangedEventArgs : EventArgs
    {
        public ActiveWindowChangedEventArgs(IntPtr hwnd)
        {
            WindowHandle = hwnd;
        }

        public IntPtr WindowHandle { get; }

        private uint? _processId;
        public uint ProcessId
        {
            get
            {
                if (_processId == null)
                    _processId = GetWindowProcessId(WindowHandle);

                return _processId.Value;
            }
        }

        private string _windowTitle;
        public string WindowTitle 
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_windowTitle))
                    _windowTitle = GetWindowTitle(WindowHandle);

                return _windowTitle;
            }
        }

        private uint GetWindowProcessId(IntPtr hwnd)
        {
            GetWindowThreadProcessId(hwnd, out var pid);
            return pid;
        }

        private string GetWindowTitle(IntPtr hwnd)
        {
            const int nChars = 256;
            var strBuilder = new StringBuilder(nChars);

            if (GetWindowText(hwnd, strBuilder, nChars) > 0)
            {
                return strBuilder.ToString();
            }

            return null;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
    }
}