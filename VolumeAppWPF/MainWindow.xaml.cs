using CoreAudio;
using CoreAudio.Enums;
using Gma.System.MouseKeyHook;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace VolumeApp
{
    public partial class MainWindow : Window
    {
        private const string valorantProcessName = "VALORANT-Win64-Shipping";
        private const string riotServicesProcessName = "RiotClientServices";

        private AudioSessionControl valorantAudioSession;
        private AudioSessionControl riotServicesAudioSession;

        private readonly ActiveWindowHooker windowHooker = new ActiveWindowHooker();
        private IKeyboardMouseEvents mkbEvents;

        public MainWindow()
        {
            InitializeComponent();

            btnAttachValorantAudio.Click += btnAttachValorantAudio_Click;
            btnToggleMuteValorantAudio.Click += btnToggleMuteValorantAudio_Click;

            btnRiotClientServicesAudio.Click += btnRiotClientServicesAudio_Click;
            btnToggleMuteRiotClientServicesAudio.Click += btnToggleMuteRiotClientServicesAudio_Click;

            windowHooker.ActiveWindowChanged += WindowHooker_ActiveWindowChanged;
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            windowHooker.Stop();
            UnsubscribeMouseEvents();

            if (valorantAudioSession != null)
            {
                valorantAudioSession.Mute = false;
                valorantAudioSession.Dispose();
            }

            if (riotServicesAudioSession != null)
            {
                riotServicesAudioSession.Mute = false;
                riotServicesAudioSession.Dispose();
            }
        }

        #region Valorant


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

            if (e.WindowProcessId == valorantAudioSession.ProcessID)
            {
                valorantAudioSession.Mute = false;
                return;
            }

            if (e.WindowProcessId != valorantAudioSession.ProcessID)
            {
                valorantAudioSession.Mute = true;
            }
        }


        #endregion

        #region RiotServices


        private void btnRiotClientServicesAudio_Click(object sender, RoutedEventArgs e)
        {
            var task = Task.Run(() =>
            {
                return AudioSessionsHelper.GetAudioSessionByProcessName(riotServicesProcessName);
            });

            task.Wait();

            if (task.Result == null)
            {
                txtRiotClientServicesAudioState.Text = "Not found";
                return;
            }

            if (riotServicesAudioSession != null)
            {
                riotServicesAudioSession.Dispose();
            }

            riotServicesAudioSession = task.Result;

            txtRiotClientServicesAudioState.Text = AudioSessionsHelper.GetStringAudioState(riotServicesAudioSession.State);
            txtRiotClientServicesAudioMuted.Text = AudioSessionsHelper.GetStringFromMuted(riotServicesAudioSession.Mute);

            riotServicesAudioSession.StateChanged += riotServicesAudioSession_StateChanged;
            riotServicesAudioSession.SimpleVolumeChanged += riotServicesAudioSession_SimpleVolumeChanged;
            SubscribeMouseEvents();
        }

        private void btnToggleMuteRiotClientServicesAudio_Click(object sender, RoutedEventArgs e)
        {
            if (riotServicesAudioSession == null)
                return;

            var newMute = !riotServicesAudioSession.Mute;

            riotServicesAudioSession.Mute = newMute;
            txtRiotClientServicesAudioMuted.Text = AudioSessionsHelper.GetStringFromMuted(newMute);
        }

        private void riotServicesAudioSession_StateChanged(object sender, AudioSessionStateChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var state = e.NewState;

                txtRiotClientServicesAudioState.Text = AudioSessionsHelper.GetStringAudioState(state);

                if (state == AudioSessionState.AudioSessionStateExpired)
                {
                    riotServicesAudioSession.Mute = false;
                    riotServicesAudioSession = null;
                    UnsubscribeMouseEvents();
                }
            }));
        }

        private void riotServicesAudioSession_SimpleVolumeChanged(object sender, AudioSessionSimpleVolumeChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtRiotClientServicesAudioMuted.Text = AudioSessionsHelper.GetStringFromMuted(e.IsMuted);
            }));
        }

        private void SubscribeMouseEvents()
        {
            mkbEvents = Hook.GlobalEvents();
            mkbEvents.MouseClick += OnMouseClick;
        }

        private void UnsubscribeMouseEvents()
        {
            if (mkbEvents == null) return;

            mkbEvents.MouseClick -= OnMouseClick;

            mkbEvents.Dispose();
            mkbEvents = null;
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Middle)
                return;

            Task.Factory.StartNew(() =>
            {
                var newMute = !riotServicesAudioSession.Mute;
                riotServicesAudioSession.Mute = newMute;

                if (newMute)
                    Console.Beep(1000, 100);
            });
        }


        #endregion
    }
}