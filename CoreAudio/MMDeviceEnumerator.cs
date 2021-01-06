using CoreAudio.Enums;
using CoreAudio.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CoreAudio
{
    [ComImport]
    [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
    internal class MMDeviceEnumeratorCOM
    { }

    public class MMDeviceEnumerator : IDisposable
    {
        private IMMDeviceEnumerator _mmDeviceEnumerator;
        private readonly List<IMMNotificationClient> _notificationClients = new List<IMMNotificationClient>();

        public MMDeviceEnumerator()
        {
            _mmDeviceEnumerator = new MMDeviceEnumeratorCOM() as IMMDeviceEnumerator;
        }

        /// <summary>
        /// Enumerate Audio Endpoints
        /// </summary>
        /// <param name="dataFlow">Desired DataFlow</param>
        /// <param name="dwStateMask">State Mask</param>
        /// <returns>Device Collection</returns>
        public MMDeviceCollection EnumerateAudioEndPoints(DataFlow dataFlow, DeviceState stateMask)
        {
            Marshal.ThrowExceptionForHR(_mmDeviceEnumerator.EnumAudioEndpoints(dataFlow, stateMask, out var result));
            return new MMDeviceCollection(result);
        }

        /// <summary>
        /// Get Default Endpoint
        /// </summary>
        /// <param name="dataFlow">Data Flow</param>
        /// <param name="role">Role</param>
        /// <returns>Device</returns>
        public MMDevice GetDefaultAudioEndpoint(DataFlow dataFlow, Role role)
        {
            var hresult = _mmDeviceEnumerator.GetDefaultAudioEndpoint(dataFlow, role, out var device);

            const int E_NOTFOUND = unchecked((int)0x80070490);
            if (hresult == E_NOTFOUND)
            {
                return null;
            }
            
            Marshal.ThrowExceptionForHR(hresult);
            return new MMDevice(device);
        }

        /// <summary>
        /// Get device by ID
        /// </summary>
        /// <param name="id">Device ID</param>
        /// <returns>Device</returns>
        public MMDevice GetDevice(string id)
        {
            Marshal.ThrowExceptionForHR(_mmDeviceEnumerator.GetDevice(id, out var device));
            return new MMDevice(device);
        }

        /// <summary>
        /// Registers a call back for Device Events
        /// </summary>
        /// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface</param>
        /// <returns></returns>
        public int RegisterEndpointNotificationCallback([In][MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
        {
            if (_notificationClients.Contains(client))
            {
                return 0;
            }

            var hresult =_mmDeviceEnumerator.RegisterEndpointNotificationCallback(client);
            Marshal.ThrowExceptionForHR(hresult);

            _notificationClients.Add(client);
            return hresult;
        }

        /// <summary>
        /// Unregisters a call back for Device Events
        /// </summary>
        /// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface </param>
        /// <returns></returns>
        public int UnregisterEndpointNotificationCallback([In][MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
        {
            if (!_notificationClients.Contains(client))
            {
                return 0;
            }

            var hresult = _mmDeviceEnumerator.UnregisterEndpointNotificationCallback(client);
            Marshal.ThrowExceptionForHR(hresult);

            _notificationClients.Remove(client);
            return hresult;
        }

        ~MMDeviceEnumerator()
        {
            Dispose();
        }

        public void Dispose()
        {
            for (var i = _notificationClients.Count - 1; i >= 0; i--)
            {
                UnregisterEndpointNotificationCallback(_notificationClients[i]);
            }

            if (_mmDeviceEnumerator != null)
            {
                Marshal.ReleaseComObject(_mmDeviceEnumerator);
                _mmDeviceEnumerator = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}