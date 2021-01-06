using CoreAudio;
using CoreAudio.Enums;
using System;
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
        private readonly ActiveWindowHooker windowHooker = new ActiveWindowHooker();

        public MainWindow()
        {
            InitializeComponent();
            btnAttachValorantAudio.Click += btnAttachValorantAudio_Click;
            btnToggleMuteValorantAudio.Click += btnToggleMuteValorantAudio_Click;

            windowHooker.ActiveWindowChanged += WindowHooker_ActiveWindowChanged;
            Application.Current.Exit += delegate { windowHooker.Stop(); if (valorantAudioSession != null) valorantAudioSession.Mute = false; };
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
            windowHooker.Init();
        }

        private void btnToggleMuteValorantAudio_Click(object sender, RoutedEventArgs e)
        {
            if (valorantAudioSession == null)
                return;

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
                    windowHooker.Stop();
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

        private void WindowHooker_ActiveWindowChanged(object sender, ActiveWindowChangedEventArgs e)
        {
            if (valorantAudioSession == null)
                return;

            valorantAudioSession.Process.Refresh();
            var valorantWindowHandle = valorantAudioSession.Process.MainWindowHandle;

            if (valorantWindowHandle == IntPtr.Zero)
                return;

            if (e.WindowHandle == valorantWindowHandle)
            {
                valorantAudioSession.Mute = false;
                return;
            }

            if (e.WindowHandle != valorantWindowHandle)
            {
                valorantAudioSession.Mute = true;
            }
        }
    }
}