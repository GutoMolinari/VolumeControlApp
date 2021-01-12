using System.Windows;
using System.Windows.Controls;

namespace VolumeApp
{
    public partial class AudioSessionStrip : UserControl
    {
        private static readonly DependencyProperty audioSessionNameDataProperty = DependencyProperty.Register(nameof(AudioSessionName), typeof(string), typeof(AudioSessionStrip));
        private static readonly DependencyProperty audioStateDataProperty = DependencyProperty.Register(nameof(AudioStateTextBox), typeof(string), typeof(AudioSessionStrip));
        private static readonly DependencyProperty audioMutedDataProperty = DependencyProperty.Register(nameof(AudioMutedTextBox), typeof(string), typeof(AudioSessionStrip));
        
        public string AudioSessionName
        {
            get => GetValue(audioSessionNameDataProperty).ToString();
            set => SetValue(audioSessionNameDataProperty, value);
        }

        public string AudioStateTextBox
        {
            get => GetValue(audioStateDataProperty).ToString();
            set => SetValue(audioStateDataProperty, value);
        }

        public string AudioMutedTextBox
        {
            get => GetValue(audioMutedDataProperty).ToString();
            set => SetValue(audioMutedDataProperty, value);
        }

        public event RoutedEventHandler ClickAttach
        {
            add { btnAttachAudio.Click += value; }
            remove { btnAttachAudio.Click -= value; }
        }

        public event RoutedEventHandler ClickMute
        {
            add { btnToggleMuteAudio.Click += value; }
            remove { btnToggleMuteAudio.Click -= value; }
        }

        //public event RoutedEventHandler Click
        //{
        //    add { btnToggleMuteAudio.AddHandler(System.Windows.Controls.Primitives.ButtonBase.ClickEvent, value); }
        //    remove { btnToggleMuteAudio.RemoveHandler(System.Windows.Controls.Primitives.ButtonBase.ClickEvent, value); }
        //}

        public AudioSessionStrip()
        {
            InitializeComponent();
            this.Unloaded += AudioSessionStrip_Unloaded;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void AudioSessionStrip_Unloaded(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
