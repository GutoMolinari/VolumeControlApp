using CoreAudioAPI.Interfaces;
using System.Runtime.InteropServices;

namespace CoreAudioAPI
{
    internal class AudioSessionNotification : IAudioSessionNotification
    {
        private readonly AudioSessionManager audioSessionManager;

        internal AudioSessionNotification(AudioSessionManager audioSessionManager)
        {
            this.audioSessionManager = audioSessionManager;
        }

        [PreserveSig]
        public int OnSessionCreated(IAudioSessionControl newSession)
        {
            audioSessionManager.FireSessionCreated(newSession);
            return 0;
        }
    }
}