using CoreAudio;
using CoreAudio.Enums;
using System.Collections.Generic;

namespace VolumeApp
{
    public static class AudioSessionsHelper
    {
        public static string GetStringAudioState(AudioSessionState state)
        {
            switch (state)
            {
                case AudioSessionState.AudioSessionStateExpired:
                    return "Expired";
                case AudioSessionState.AudioSessionStateActive:
                    return "Active";
                case AudioSessionState.AudioSessionStateInactive:
                    return "Inactive";
                default:
                    return string.Empty;
            }
        }

        public static string GetStringFromMuted(bool mute)
        {
            return mute
                ? "Muted"
                : "Unmuted"
                ;
        }

        public static IEnumerable<AudioSessionControl> GetAllAudioSessions()
        {
            using (var enumeratorMMDevice = new MMDeviceEnumerator())
            using (var device = enumeratorMMDevice.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
            {
                foreach (var audioSessionControl in device.AudioSessionManager.Sessions)
                {
                    yield return audioSessionControl;
                }
            }
        }

        public static AudioSessionControl GetAudioSessionByProcessName(string processName)
        {
            foreach (var audioSession in GetAllAudioSessions())
            {
                if (audioSession.Process.ProcessName == processName)
                    return audioSession;
            }

            return null;
        }
    }
}
