using CoreAudio.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace CoreAudio
{
    public class SimpleAudioVolume : IDisposable
    {
        private readonly ISimpleAudioVolume simpleAudioVolume;

        internal SimpleAudioVolume(ISimpleAudioVolume realSimpleVolume)
        {
            simpleAudioVolume = realSimpleVolume;
        }

        /// <summary>
        /// Allows the user to adjust the volume from 0.0 to 1.0
        /// </summary>
        public float Volume
        {
            get
            {
                Marshal.ThrowExceptionForHR(simpleAudioVolume.GetMasterVolume(out var result));
                return result;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException($"value should be between 0.0 and 1.0");

                Marshal.ThrowExceptionForHR(simpleAudioVolume.SetMasterVolume(value, Guid.Empty));
            }
        }

        public bool Mute
        {
            get
            {
                Marshal.ThrowExceptionForHR(simpleAudioVolume.GetMute(out var result));
                return result;
            }
            set
            {
                Marshal.ThrowExceptionForHR(simpleAudioVolume.SetMute(value, Guid.Empty));
            }
        }

        ~SimpleAudioVolume()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}