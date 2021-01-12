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
#if DEBUG
        private const string valorantProcessName = "mpc-hc64";
        private const string riotServicesProcessName = "mpc-hc64";
#else
        private const string valorantProcessName = "VALORANT-Win64-Shipping";
        private const string riotServicesProcessName = "RiotClientServices";
#endif

        private AudioSessionControl valorantAudioSession;
        private AudioSessionControl riotServicesAudioSession;

        private readonly ActiveWindowHooker windowHooker = new ActiveWindowHooker();
        private IKeyboardMouseEvents mkbEvents;

        public MainWindow()
        {
            InitializeComponent();

            valorantStrip.ClickAttach += btnAttachValorantAudio_Click;
            valorantStrip.ClickMute += btnToggleMuteValorantAudio_Click;

            rioServicesStrip.ClickAttach += btnRiotClientServicesAudio_Click;
            rioServicesStrip.ClickMute += btnToggleMuteRiotClientServicesAudio_Click;

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
                valorantStrip.AudioStateTextBox = "Not found";
                return;
            }

            if (valorantAudioSession != null)
            {
                valorantAudioSession.Dispose();
            }

            valorantAudioSession = task.Result;

            valorantStrip.AudioStateTextBox = AudioSessionsHelper.GetStringAudioState(valorantAudioSession.State);
            valorantStrip.AudioMutedTextBox = AudioSessionsHelper.GetStringFromMuted(valorantAudioSession.Mute);

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
            valorantStrip.AudioMutedTextBox = AudioSessionsHelper.GetStringFromMuted(newMute);
        }

        private void valorantAudioSession_StateChanged(object sender, AudioSessionStateChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var state = e.NewState;

                valorantStrip.AudioStateTextBox = AudioSessionsHelper.GetStringAudioState(state);

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
                valorantStrip.AudioMutedTextBox = AudioSessionsHelper.GetStringFromMuted(e.IsMuted);
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
                rioServicesStrip.AudioStateTextBox = "Not found";
                return;
            }

            if (riotServicesAudioSession != null)
            {
                riotServicesAudioSession.Dispose();
            }

            riotServicesAudioSession = task.Result;

            rioServicesStrip.AudioStateTextBox = AudioSessionsHelper.GetStringAudioState(riotServicesAudioSession.State);
            rioServicesStrip.AudioMutedTextBox = AudioSessionsHelper.GetStringFromMuted(riotServicesAudioSession.Mute);

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
            rioServicesStrip.AudioMutedTextBox = AudioSessionsHelper.GetStringFromMuted(newMute);
        }

        private void riotServicesAudioSession_StateChanged(object sender, AudioSessionStateChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var state = e.NewState;

                rioServicesStrip.AudioStateTextBox = AudioSessionsHelper.GetStringAudioState(state);

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
                rioServicesStrip.AudioMutedTextBox = AudioSessionsHelper.GetStringFromMuted(e.IsMuted);
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