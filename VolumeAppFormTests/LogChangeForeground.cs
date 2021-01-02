using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace VolumeAppFormTests
{
    public partial class LogChangeForeground : Form
    {
        delegate void WinEventDelegate(
            IntPtr hWinEventHook, 
            uint eventType,
            IntPtr hwnd, 
            int idObject, 
            int idChild, 
            uint dwEventThread, 
            uint dwmsEventTime
            );

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(
            uint eventMin, 
            uint eventMax, 
            IntPtr hmodWinEventProc, 
            WinEventDelegate lpfnWinEventProc, 
            uint idProcess,
            uint idThread, 
            uint dwFlags
            );

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private const uint WINEVENT_OUTOFCONTEXT = 0;

        private WinEventDelegate procDelegate;
        private IntPtr hhook;

        public LogChangeForeground()
        {
            InitializeComponent();

            // Need to ensure delegate is not collected while we're using it,
            // storing it in a class field is simplest way to do this.
            procDelegate = new WinEventDelegate(WinEventProc);


            // Listen for foreground changes across all processes/threads on current desktop...
            hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, procDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            Log.Text += $"{hwnd} - {GetActiveWindowTitle(hwnd):x8}{Environment.NewLine}------{Environment.NewLine}";
            //Log.Text += string.Format("Foreground changed to {0:x8} \r\n", hwnd.ToInt32());
        }

        private void LogEveryChangeInWindowsForground_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnhookWinEvent(hhook);
        }


        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private string GetActiveWindowTitle(IntPtr hwnd)
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);

            if (GetWindowText(hwnd, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
    }
}
