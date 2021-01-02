using CoreAudioAPI.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace CoreAudioAPI
{
    public class AudioSessionManager
    {
        private readonly IAudioSessionManager2 _audioSessionManager2;

        private SessionCollection _sessions;
        /// <summary>
        /// Returns list of sessions of current device.
        /// </summary>
        public SessionCollection Sessions
        {
            get
            {
                if (_sessions == null)
                    RefreshSessions();

                return _sessions;
            }
        }

        private SimpleAudioVolume _simpleAudioVolume;
        /// <summary>
        /// SimpleAudioVolume object for adjusting the volume for the user session
        /// </summary>
        public SimpleAudioVolume SimpleAudioVolume
        {
            get
            {
                if (_simpleAudioVolume == null)
                {
                    _audioSessionManager2.GetSimpleAudioVolume(Guid.Empty, 0, out var simpleAudioInterface);
                    _simpleAudioVolume = new SimpleAudioVolume(simpleAudioInterface);
                }

                return _simpleAudioVolume;
            }
        }

        private AudioSessionControl _audioSessionControl;
        /// <summary>
        /// AudioSessionControl object for registring for callbacks and other session information
        /// </summary>
        public AudioSessionControl AudioSessionControl
        {
            get
            {
                if (_audioSessionControl == null)
                {
                    _audioSessionManager2.GetAudioSessionControl(Guid.Empty, 0, out var audioSessionControlInterface);
                    _audioSessionControl = new AudioSessionControl(audioSessionControlInterface);
                }

                return _audioSessionControl;
            }
        }

        /// <summary>
        /// Occurs when audio session has been added (for example run another program that use audio playback).
        /// </summary>
        public event SessionCreatedDelegate OnSessionCreated;
        public delegate void SessionCreatedDelegate(object sender, IAudioSessionControl newSession);
        private AudioSessionNotification _audioSessionNotification;

        internal AudioSessionManager(IAudioSessionManager audioSessionManager)
        {
            _audioSessionManager2 = audioSessionManager as IAudioSessionManager2;
            RegisterNotifications();
        }

        internal void FireSessionCreated(IAudioSessionControl newSession)
        {
            RefreshSessions();
            OnSessionCreated?.Invoke(this, newSession);
        }

        /// <summary>
        /// Refresh session of current device.
        /// </summary>
        public void RefreshSessions()
        {
            if (_audioSessionManager2 != null)
            {
                Marshal.ThrowExceptionForHR(_audioSessionManager2.GetSessionEnumerator(out var sessionEnum));
                _sessions = new SessionCollection(sessionEnum);
            }
        }

        private void RegisterNotifications()
        {
            if (_audioSessionManager2 != null)
            {
                _audioSessionNotification = new AudioSessionNotification(this);
                Marshal.ThrowExceptionForHR(_audioSessionManager2.RegisterSessionNotification(_audioSessionNotification));
            }
        }

        private void UnregisterNotifications()
        {
            _sessions = null;

            if (_audioSessionNotification != null && _audioSessionManager2 != null)
            {
                Marshal.ThrowExceptionForHR(_audioSessionManager2.UnregisterSessionNotification(_audioSessionNotification));
                _audioSessionNotification = null;
            }
        }

        public void Dispose()
        {
            UnregisterNotifications();
            GC.SuppressFinalize(this);
        }

        ~AudioSessionManager()
        {
            Dispose();
        }
    }
}