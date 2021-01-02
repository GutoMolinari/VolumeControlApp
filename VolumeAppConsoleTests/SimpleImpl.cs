using CoreAudioAPI;
using CoreAudioAPI.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VolumeAppTest
{
    public class SimpleImpl
    {
        private const string valorantProcessName = "VALORANT-Win64-Shipping";
        private const string riotServicesProcessName = "RiotClientServices";
        private const string mpcHcProcessName = "mpc-hc64";

        private static AudioSessionControl valorantAudio;
        private static AudioSessionControl riotServicesAudio;
        private static AudioSessionControl mpcHcAudio;

        public static void Main()
        {
            InitiateAudioSessions();

            if (valorantAudio != null)
                valorantAudio.Mute = false;

            if (riotServicesAudio != null)
                riotServicesAudio.Mute = false;

            if (mpcHcAudio != null)
            {
                mpcHcAudio.Mute = true;
                Console.WriteLine($"{mpcHcProcessName} muted");
            }

            if (SetMuteProcess(mpcHcProcessName, false))
            {
                Console.WriteLine($"{mpcHcProcessName} unmuted");
            }
        }

        public static bool SetMuteProcess(string processName, bool mute)
        {
            foreach (var session in GetAllAudioSessions())
            {
                var process = Process.GetProcessById((int)session.GetProcessID);

                if (process.ProcessName != processName)
                    continue;

                session.Mute = mute;
                return true;
            }

            return false;
        }

        public static void InitiateAudioSessions()
        {
            foreach (var session in GetAllAudioSessions())
            {
                var process = Process.GetProcessById((int)session.GetProcessID);

                if (process.ProcessName == valorantProcessName)
                {
                    valorantAudio = session;
                }

                if (process.ProcessName == riotServicesProcessName)
                {
                    riotServicesAudio = session;
                }

                if (process.ProcessName == mpcHcProcessName)
                {
                    mpcHcAudio = session;
                }
            }
        }

        public static IEnumerable<AudioSessionControl> GetAllAudioSessions()
        {
            using (var deviceEnumerator = new MMDeviceEnumerator())
            using (var defaultMMDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
            {
                foreach (var session in defaultMMDevice.AudioSessionManager.Sessions)
                {
                    yield return session;
                }
            }
        }

        public static AudioSessionControl GetAudioSessionByProcessName(string processName)
        {
            foreach (var session in GetAllAudioSessions())
            {
                var process = Process.GetProcessById((int)session.GetProcessID);

                if (process.ProcessName == processName)
                    return session;
            }

            return null;
        }
    }
}