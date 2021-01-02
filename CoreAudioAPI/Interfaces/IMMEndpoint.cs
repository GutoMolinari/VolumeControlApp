using CoreAudioAPI.Enums;
using System;
using System.Runtime.InteropServices;

namespace CoreAudioAPI.Interfaces
{
    /// <summary>
    /// Represents an audio endpoint device.
    /// </summary>
    [Guid("1BE09788-6894-4089-8586-9A2A6C265AC5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IMMEndpoint
    {
        /// <summary>
        /// Indicates whether the endpoint is associated with a rendering device or a capture device.
        /// </summary>
        /// <param name="dataFlow">The data-flow direction of the endpoint device.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetDataFlow([Out][MarshalAs(UnmanagedType.I4)] out DataFlow dataFlow);
    }
}
