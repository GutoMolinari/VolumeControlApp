using CSCore.CoreAudioAPI;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VolumeApp
{
    public partial class MainWindow : Window
    {
        //private const string valorantProcessName = "mpc-hc64";
        //private const string valorantProcessName = "firefox";
        private const string valorantProcessName = "VALORANT-Win64-Shipping";
        private const string riotServicesProcessName = "RiotClientServices";

        private AudioSessionControl2 valorantAudioSession;

        private CancellationTokenSource cancellation;

        public MainWindow()
        {
            InitializeComponent();
            btnAttachValorantAudio.Click += btnAttachValorantAudio_Click;
            btnToggleMuteValorantAudio.Click += btnToggleMuteValorantAudio_Click;

            Application.Current.Exit += delegate { cancellation?.Cancel(); if (valorantAudioSession != null) valorantAudioSession.QueryInterface<SimpleAudioVolume>().IsMuted = false; };
        }

        //
        private void btnAttachValorantAudio_Click(object sender, RoutedEventArgs e)
        {
            var task = Task.Run(() =>
            {
                return AudioSessionsHelper.GetAudioSessionByProcessName(valorantProcessName);
            });

            task.Wait();

            if (task.Result == null)
            {
                txtValorantAudioState.Text = "Not found";
                return;
            }

            if (valorantAudioSession != null)
            {
                valorantAudioSession.Dispose();
            }

            valorantAudioSession = task.Result;

            txtValorantAudioState.Text = AudioSessionsHelper.GetStringAudioState(valorantAudioSession.SessionState);
            txtValorantAudioMuted.Text = GetStringFromMuted(valorantAudioSession.QueryInterface<SimpleAudioVolume>().IsMuted);

            valorantAudioSession.StateChanged += valorantAudioSession_StateChanged;
            valorantAudioSession.SimpleVolumeChanged += valorantAudioSession_SimpleVolumeChanged;

            cancellation?.Cancel();

            cancellation = new CancellationTokenSource();
            Task.Factory.StartNew(() => ShitObserver(cancellation.Token), cancellation.Token );
        }

        private void btnToggleMuteValorantAudio_Click(object sender, RoutedEventArgs e)
        {
            if (valorantAudioSession == null)
            {
                return;
            }

            var simpleAudio = valorantAudioSession.QueryInterface<SimpleAudioVolume>();
            var newMute = !simpleAudio.IsMuted;

            simpleAudio.IsMuted = newMute;
            txtValorantAudioMuted.Text = GetStringFromMuted(newMute);
        }

        private void valorantAudioSession_StateChanged(object sender, AudioSessionStateChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var state = e.NewState;

                txtValorantAudioState.Text = AudioSessionsHelper.GetStringAudioState(state);

                if (state == AudioSessionState.AudioSessionStateExpired)
                {
                    cancellation.Cancel();
                    valorantAudioSession.QueryInterface<SimpleAudioVolume>().IsMuted = false;
                    valorantAudioSession = null;
                }
            });
        }

        private void valorantAudioSession_SimpleVolumeChanged(object sender, AudioSessionSimpleVolumeChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtValorantAudioMuted.Text = GetStringFromMuted(e.IsMuted);
            });
        }

        //
        private string GetStringFromMuted(bool isMuted)
        {
            return isMuted
                ? "Muted"
                : "Unmuted"
                ;
        }

        public void ShitObserver(CancellationToken token)
        {
            if (valorantAudioSession == null)
                return;

            valorantAudioSession.Process.Refresh();
            var valorantWindowPtr = valorantAudioSession.Process.MainWindowHandle;
            var simpleAudio = valorantAudioSession.QueryInterface<SimpleAudioVolume>();
            var isMuted = simpleAudio.IsMuted;
            var count = 0L;

            if (valorantWindowPtr == IntPtr.Zero)
                return;

            while (true)
            {
                Thread.Sleep(1000);
                count++;

                if (count % 60 == 0)
                {
                    if (token.IsCancellationRequested)
                    {
                        Debug.WriteLine("task canceled");
                        return;
                    }

                    if (valorantAudioSession.SessionState == AudioSessionState.AudioSessionStateExpired)
                        return;
                }

                var focusedWindow = GetForegroundWindow();

                if (focusedWindow == valorantWindowPtr 
                    && isMuted)
                {
                    simpleAudio.IsMuted = 
                        isMuted = false;
                    continue;
                }

                if (focusedWindow != valorantWindowPtr 
                    && !isMuted)
                {
                    simpleAudio.IsMuted = 
                        isMuted = true;
                    continue;
                }
            }
        }

        private const string user32Dll = "user32.dll";
        [System.Runtime.InteropServices.DllImport(user32Dll)]
        public static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport(user32Dll)]
        public static extern int SetForegroundWindow(IntPtr hwnd);
    }
}