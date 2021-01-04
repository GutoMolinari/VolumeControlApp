using CoreAudio.Enums;
using CoreAudio.Externals;
using CoreAudio.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace CoreAudio
{
    public class MMDevice : IDisposable
    {
        private readonly IMMDevice _mmDevice;

        private static readonly Guid IDD_IAudioSessionManager = new Guid("BFA971F1-4D5E-40BB-935E-967039BFBEE4");

        private AudioSessionManager _audioSessionManager;
        public AudioSessionManager AudioSessionManager
        {
            get
            {
                if (_audioSessionManager == null)
                    InitializeAudioSessionManager();

                return _audioSessionManager;
            }
        }

        private PropertyStore _propertyStore;
        private PropertyStore PropertyStore
        {
            get
            {
                if (_propertyStore == null)
                    InitializePropertyInformation();

                return _propertyStore;
            }
        }

        internal MMDevice(IMMDevice realDevice)
        {
            _mmDevice = realDevice;
        }

        private void InitializeAudioSessionManager()
        {
            Marshal.ThrowExceptionForHR(_mmDevice.Activate(IDD_IAudioSessionManager, ClsCtx.ALL, IntPtr.Zero, out var result));
            _audioSessionManager = new AudioSessionManager(result as IAudioSessionManager);
        }

        /// <summary>
        /// Initializes the device's property store.
        /// </summary>
        /// <param name="stgmAccess">The storage-access mode to open store for.</param>
        /// <remarks>Administrative client is required for Write and ReadWrite modes.</remarks>
        private void InitializePropertyInformation(StorageAccessMode stgmAccess = StorageAccessMode.Read)
        {
            Marshal.ThrowExceptionForHR(_mmDevice.OpenPropertyStore(stgmAccess, out var propstore));
            _propertyStore = new PropertyStore(propstore);
        }

        #region Properties


        /// <summary>
        /// Friendly name for the endpoint
        /// </summary>
        public string FriendlyName
        {
            get
            {
                return PropertyStore.GetPropertyValue(PropertyKeys.PKEY_Device_FriendlyName);
            }
        }

        /// <summary>
        /// Friendly name of device
        /// </summary>
        public string DeviceFriendlyName
        {
            get
            {
                return PropertyStore.GetPropertyValue(PropertyKeys.PKEY_DeviceInterface_FriendlyName);
            }
        }

        /// <summary>
        /// Icon path of device
        /// </summary>
        public string IconPath
        {
            get
            {
                return PropertyStore.GetPropertyValue(PropertyKeys.PKEY_Device_IconPath);
            }
        }

        /// <summary>
        /// Device Instance Id of Device
        /// </summary>
        public string InstanceId
        {
            get
            {
                return PropertyStore.GetPropertyValue(PropertyKeys.PKEY_Device_InstanceId);
            }
        }

        /// <summary>
        /// Device ID
        /// </summary>
        public string ID
        {
            get
            {
                Marshal.ThrowExceptionForHR(_mmDevice.GetId(out var result));
                return result;
            }
        }

        /// <summary>
        /// Data Flow
        /// </summary>
        public DataFlow DataFlow
        {
            get
            {
                (_mmDevice as IMMEndpoint).GetDataFlow(out var result);
                return result;
            }
        }

        /// <summary>
        /// Device State
        /// </summary>
        public DeviceState State
        {
            get
            {
                Marshal.ThrowExceptionForHR(_mmDevice.GetState(out var result));
                return result;
            }
        }


        #endregion

        public override string ToString()
        {
            return FriendlyName;
        }

        ~MMDevice()
        {
            Dispose();
        }

        public void Dispose()
        {
            this._audioSessionManager?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
