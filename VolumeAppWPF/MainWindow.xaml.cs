using CoreAudio;
using CoreAudio.Enums;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

        private AudioSessionControl valorantAudioSession;

        private CancellationTokenSource cancellation;

        public MainWindow()
        {
            InitializeComponent();
            btnAttachValorantAudio.Click += btnAttachValorantAudio_Click;
            btnToggleMuteValorantAudio.Click += btnToggleMuteValorantAudio_Click;

            Application.Current.Exit += delegate { cancellation?.Cancel(); if (valorantAudioSession != null) valorantAudioSession.Mute = false; };
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

            txtValorantAudioState.Text = AudioSessionsHelper.GetStringAudioState(valorantAudioSession.State);
            txtValorantAudioMuted.Text = AudioSessionsHelper.GetStringFromMuted(valorantAudioSession.Mute);

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

            var newMute = !valorantAudioSession.Mute;

            valorantAudioSession.Mute = newMute;
            txtValorantAudioMuted.Text = AudioSessionsHelper.GetStringFromMuted(newMute);
        }

        private void valorantAudioSession_StateChanged(object sender, AudioSessionStateChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var state = e.NewState;

                txtValorantAudioState.Text = AudioSessionsHelper.GetStringAudioState(state);

                if (state == AudioSessionState.AudioSessionStateExpired)
                {
                    cancellation.Cancel();
                    valorantAudioSession.Mute = false;
                    valorantAudioSession = null;
                }
            }));
        }

        private void valorantAudioSession_SimpleVolumeChanged(object sender, AudioSessionSimpleVolumeChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtValorantAudioMuted.Text = AudioSessionsHelper.GetStringFromMuted(e.IsMuted);
            }));
        }

        //
        public void ShitObserver(CancellationToken token)
        {
            if (valorantAudioSession == null)
                return;

            valorantAudioSession.Process.Refresh();
            var valorantWindowPtr = valorantAudioSession.Process.MainWindowHandle;
            var isMuted = valorantAudioSession.Mute;
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

                    if (valorantAudioSession.State == AudioSessionState.AudioSessionStateExpired)
                        return;
                }

                var focusedWindow = GetForegroundWindow();

                if (focusedWindow == valorantWindowPtr 
                    && isMuted)
                {
                    valorantAudioSession.Mute = 
                        isMuted = false;
                    continue;
                }

                if (focusedWindow != valorantWindowPtr 
                    && !isMuted)
                {
                    valorantAudioSession.Mute = 
                        isMuted = true;
                    continue;
                }
            }
        }

        private const string user32Dll = "user32.dll";
        [DllImport(user32Dll)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport(user32Dll)]
        public static extern int SetForegroundWindow(IntPtr hwnd);
    }
}