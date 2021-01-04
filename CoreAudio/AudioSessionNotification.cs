using CoreAudio.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace CoreAudio
{
    internal class AudioSessionNotification : IAudioSessionNotification
    {
        internal event EventHandler<IAudioSessionControl> OnSessionCreatedInternal;

        internal AudioSessionNotification()
        { }

        [PreserveSig]
        int IAudioSessionNotification.OnSessionCreated(IAudioSessionControl newSession)
        {
            OnSessionCreatedInternal?.Invoke(this, newSession);
            return 0;
        }
    }
}