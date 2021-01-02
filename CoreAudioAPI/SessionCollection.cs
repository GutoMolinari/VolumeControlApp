using CoreAudioAPI.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CoreAudioAPI
{
    public class SessionCollection : IEnumerable<AudioSessionControl>
    {
        private readonly IAudioSessionEnumerator _audioSessionEnumerator;

        internal SessionCollection(IAudioSessionEnumerator audioSessionEnumerator)
        {
            this._audioSessionEnumerator = audioSessionEnumerator;
        }

        /// <summary>
        /// Returns session at index.
        /// </summary>
        public AudioSessionControl this[int index]
        {
            get
            {
                Marshal.ThrowExceptionForHR(_audioSessionEnumerator.GetSession(index, out var result));
                return new AudioSessionControl(result);
            }
        }

        /// <summary>
        /// Number of current sessions.
        /// </summary>
        public int Count
        {
            get
            {
                Marshal.ThrowExceptionForHR(_audioSessionEnumerator.GetCount(out var result));
                return result;
            }
        }

        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>AudioSession enumerator</returns>
        public IEnumerator<AudioSessionControl> GetEnumerator()
        {
            var count = Count;

            for (var index = 0; index < count; index++)
            {
                yield return this[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}