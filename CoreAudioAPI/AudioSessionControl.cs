using CoreAudioAPI.Enums;
using CoreAudioAPI.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace CoreAudioAPI
{
    public class AudioSessionControl : IDisposable
    {
        private readonly IAudioSessionControl _audioSessionControl;
        private readonly IAudioSessionControl2 _audioSessionControl2;
        private readonly SimpleAudioVolume _simpleAudioVolume;

        private AudioSessionEvents _sessionEvents;
        private AudioSessionEvents SessionEventsClient
        {
            get
            {
                if (_sessionEvents == null)
                    RegisterEventClient();

                return _sessionEvents;
            }
        }

        public event EventHandler<AudioSessionDisplayNameChangedEventArgs> DisplayNameChanged
        {
            add { SessionEventsClient.DisplayNameChanged += value; }
            remove { SessionEventsClient.DisplayNameChanged -= value; }
        }

        public event EventHandler<AudioSessionIconPathChangedEventArgs> IconPathChanged
        {
            add { SessionEventsClient.IconPathChanged += value; }
            remove { SessionEventsClient.IconPathChanged -= value; }
        }

        public event EventHandler<AudioSessionSimpleVolumeChangedEventArgs> SimpleVolumeChanged
        {
            add { SessionEventsClient.SimpleVolumeChanged += value; }
            remove { SessionEventsClient.SimpleVolumeChanged -= value; }
        }

        public event EventHandler<AudioSessionChannelVolumeChangedEventArgs> ChannelVolumeChanged
        {
            add { SessionEventsClient.ChannelVolumeChanged += value; }
            remove { SessionEventsClient.ChannelVolumeChanged -= value; }
        }

        public event EventHandler<AudioSessionGroupingParamChangedEventArgs> GroupingParamChanged
        {
            add { SessionEventsClient.GroupingParamChanged += value; }
            remove { SessionEventsClient.GroupingParamChanged -= value; }
        }

        public event EventHandler<AudioSessionStateChangedEventArgs> StateChanged
        {
            add { SessionEventsClient.StateChanged += value; }
            remove { SessionEventsClient.StateChanged -= value; }
        }

        public event EventHandler<AudioSessionDisconnectedEventArgs> SessionDisconnected
        {
            add { SessionEventsClient.SessionDisconnected += value; }
            remove { SessionEventsClient.SessionDisconnected -= value; }
        }

        public AudioSessionControl(IAudioSessionControl audioSessionControl)
        {
            _audioSessionControl = audioSessionControl;
            _audioSessionControl2 = audioSessionControl as IAudioSessionControl2;

            //if (_audioSessionControl2 == null)
            //    throw new InvalidOperationException("Not supported on this version of Windows");

            if (_audioSessionControl is ISimpleAudioVolume simpleVolume)
                _simpleAudioVolume = new SimpleAudioVolume(simpleVolume);
        }

        public bool Mute
        {
            get { return _simpleAudioVolume.Mute; }
            set { _simpleAudioVolume.Mute = value; }
        }

        /// <summary>
        /// Allows the user to adjust the volume from 0.0 to 1.0
        /// </summary>
        public float Volume
        {
            get { return _simpleAudioVolume.Volume; }
            set { _simpleAudioVolume.Volume = value; }
        }

        /// <summary>
        /// The current state of the audio session.
        /// </summary>
        public AudioSessionState State
        {
            get
            {
                Marshal.ThrowExceptionForHR(_audioSessionControl.GetState(out var state));
                return state;
            }
        }

        /// <summary>
        /// The name of the audio session.
        /// </summary>
        public string DisplayName
        {
            get
            {
                Marshal.ThrowExceptionForHR(_audioSessionControl.GetDisplayName(out var displayName));
                return displayName;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    Marshal.ThrowExceptionForHR(_audioSessionControl.SetDisplayName(value, Guid.Empty));
            }
        }

        /// <summary>
        /// the path to the icon shown in the mixer.
        /// </summary>
        public string IconPath
        {
            get
            {
                Marshal.ThrowExceptionForHR(_audioSessionControl.GetIconPath(out var iconPath));
                return iconPath;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    Marshal.ThrowExceptionForHR(_audioSessionControl.SetIconPath(value, Guid.Empty));
            }
        }

        /// <summary>
        /// The session identifier of the audio session.
        /// </summary>
        public string GetSessionIdentifier
        {
            get
            {
                Marshal.ThrowExceptionForHR(_audioSessionControl2.GetSessionIdentifier(out var str));
                return str;
            }
        }

        /// <summary>
        /// The session instance identifier of the audio session.
        /// </summary>
        public string GetSessionInstanceIdentifier
        {
            get
            {
                Marshal.ThrowExceptionForHR(_audioSessionControl2.GetSessionInstanceIdentifier(out var str));
                return str;
            }
        }

        /// <summary>
        /// The process identifier of the audio session.
        /// </summary>
        public uint GetProcessID
        {
            get
            {
                Marshal.ThrowExceptionForHR(_audioSessionControl2.GetProcessId(out var pid));
                return pid;
            }
        }

        /// <summary>
        /// Is the session a system sounds session.
        /// </summary>
        public bool IsSystemSoundsSession
        {
            get
            {
                return (_audioSessionControl2.IsSystemSoundsSession() == 0x0);
            }
        }

        /// <summary>
        /// The grouping parameter of the audio session.
        /// </summary>
        public Guid GroupingParam
        {
            get
            {
                Marshal.ThrowExceptionForHR(_audioSessionControl.GetGroupingParam(out var groupingId));
                return groupingId;
            }
            set
            {
                Marshal.ThrowExceptionForHR(_audioSessionControl.SetGroupingParam(value, Guid.Empty));
            }
        }

        /// <summary>
        /// Enables or disables the default stream attenuation experience (auto-ducking) provided by the system.
        /// </summary>
        /// <param name="optOut">True to disable system auto-ducking, or false to enable.</param>
        public void SetDuckingPreference(bool optOut)
        {
            Marshal.ThrowExceptionForHR(_audioSessionControl2.SetDuckingPreference(optOut));
        }

        /// <summary>
        /// Registers the event client for callbacks
        /// </summary>
        private void RegisterEventClient()
        {
            UnregisterEventClient();
            
            _sessionEvents = new AudioSessionEvents();
            Marshal.ThrowExceptionForHR(_audioSessionControl.RegisterAudioSessionNotification(_sessionEvents));
        }

        /// <summary>
        /// Unregisters the event client from receiving callbacks
        /// </summary>
        private void UnregisterEventClient()
        {
            if (_sessionEvents == null)
                return;

            Marshal.ThrowExceptionForHR(_audioSessionControl.UnregisterAudioSessionNotification(_sessionEvents));
            _sessionEvents = null;
        }

        public void Dispose()
        {
            UnregisterEventClient();
            GC.SuppressFinalize(this);
        }

        ~AudioSessionControl()
        {
            Dispose();
        }
    }
}