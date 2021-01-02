using CoreAudio.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CoreAudio
{
    public class MMDeviceCollection : IEnumerable<MMDevice>
    {
        private readonly IMMDeviceCollection _mmDeviceCollection;

        internal MMDeviceCollection(IMMDeviceCollection collectionInterface)
        {
            _mmDeviceCollection = collectionInterface;
        }

        /// <summary>
        /// Get device by index
        /// </summary>
        /// <param name="index">Device index</param>
        /// <returns>Device at the specified index</returns>
        public MMDevice this[int index]
        {
            get
            {
                Marshal.ThrowExceptionForHR(_mmDeviceCollection.Item((uint)index, out var result));
                return new MMDevice(result);
            }
        }

        /// <summary>
        /// Device count
        /// </summary>
        public int Count
        {
            get
            {
                Marshal.ThrowExceptionForHR(_mmDeviceCollection.GetCount(out var result));
                return (int)result;
            }
        }

        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>Device enumerator</returns>
        public IEnumerator<MMDevice> GetEnumerator()
        {
            var count = Count;

            for (int index = 0; index < count; index++)
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