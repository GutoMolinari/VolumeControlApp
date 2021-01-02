using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

        public static IEnumerable<AudioSessionControl2> GetAllAudioSessions()
        {
            using (var enumeratorMMDevice = new MMDeviceEnumerator())
            using (var device = enumeratorMMDevice.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
            using (var sessionManager = AudioSessionManager2.FromMMDevice(device))
            using (var enumeratorSessions = sessionManager.GetSessionEnumerator())
            {
                foreach (var audioSessionControl in enumeratorSessions)
                {
                    yield return audioSessionControl.QueryInterface<AudioSessionControl2>();
                }
            }
        }

        public static AudioSessionControl2 GetAudioSessionByProcessName(string processName)
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
