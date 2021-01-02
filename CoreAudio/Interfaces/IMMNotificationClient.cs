using CoreAudio.Enums;
using CoreAudio.Externals;
using System;
using System.Runtime.InteropServices;

namespace CoreAudio.Interfaces
{
    /// <summary>
    /// Provides notifications when an audio endpoint device is added or removed, when the state
    /// or properties of a device change, or when there is a change in the default role assigned to a device.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd371417.aspx
    /// </remarks>
    [Guid("7991EEC9-7E89-4D85-8390-6C703CEC60C0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMNotificationClient
    {
        /// <summary>
        /// Notifies the client that the default audio endpoint device for a particular role has changed.
        /// </summary>
        /// <param name="deviceId">The endpoint ID string that identifies the audio endpoint device.</param>
        /// <param name="newState">The <see cref="DeviceState"/> constant that indicates the new state.</param>
        void OnDeviceStateChanged(
            [MarshalAs(UnmanagedType.LPWStr)] string deviceId,
            [MarshalAs(UnmanagedType.U4)] DeviceState newState
            );

        /// <summary>
        /// Indicates that a new audio endpoint device has been added.
        /// </summary>
        /// <param name="deviceId">The endpoint ID string that identifies the audio endpoint device.</param>
        void OnDeviceAdded([MarshalAs(UnmanagedType.LPWStr)] string deviceId);

        /// <summary>
        /// Indicates that an audio endpoint device has been removed.
        /// </summary>
        /// <param name="deviceId">The endpoint ID string that identifies the audio endpoint device.</param>
        void OnDeviceRemoved([MarshalAs(UnmanagedType.LPWStr)] string deviceId);

        /// <summary>
        /// Notifies the client that the default audio endpoint device for a particular role has changed.
        /// </summary>
        /// <param name="dataFlow">The data-flow direction of the endpoint device.</param>
        /// <param name="deviceRole">The device role of the audio endpoint device.</param>
        /// <param name="defaultDeviceId">The endpoint ID string that identifies the audio endpoint device.</param>
        void OnDefaultDeviceChanged(
            [MarshalAs(UnmanagedType.I4)] DataFlow dataFlow,
            [MarshalAs(UnmanagedType.I4)] Role deviceRole,
            [MarshalAs(UnmanagedType.LPWStr)] string defaultDeviceId
            );

        /// <summary>
        /// Indicates that the value of a property belonging to an audio endpoint device has changed.
        /// </summary>
        /// <param name="deviceId">The endpoint ID string that identifies the audio endpoint device.</param>
        /// <param name="propertyKey">A <see cref="PropertyKey"/> that specifies the type of property.</param>
        void OnPropertyValueChanged([MarshalAs(UnmanagedType.LPWStr)] string deviceId, PropertyKey propertyKey);
    }
}