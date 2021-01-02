using CoreAudioAPI.Enums;
using System;
using System.Runtime.InteropServices;

namespace CoreAudioAPI.Interfaces
{
    /// <summary>
    /// Provides notifications of session-related events such as changes in the volume level, display name, and session state.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd368289.aspx
    /// </remarks>
    [Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioSessionEvents
    {
        /// <summary>
        /// Notifies the client that the display name for the session has changed.
        /// </summary>
        /// <param name="displayName">The new display name for the session.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
        [PreserveSig]
        int OnDisplayNameChanged(
            [In][MarshalAs(UnmanagedType.LPWStr)] string displayName,
            [In] ref Guid eventContext
            );

        /// <summary>
        /// Notifies the client that the display icon for the session has changed.
        /// </summary>
        /// <param name="iconPath">The path for the new display icon for the session.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
        [PreserveSig]
        int OnIconPathChanged(
            [In][MarshalAs(UnmanagedType.LPWStr)] string iconPath,
            [In] ref Guid eventContext
            );

        /// <summary>
        /// Notifies the client that the volume level or muting state of the session has changed.
        /// </summary>
        /// <param name="volume">The new volume level for the audio session.</param>
        /// <param name="isMuted">The new muting state.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
        [PreserveSig]
        int OnSimpleVolumeChanged(
            [In][MarshalAs(UnmanagedType.R4)] float volume,
            [In][MarshalAs(UnmanagedType.Bool)] bool isMuted,
            [In] ref Guid eventContext
            );

        /// <summary>
        /// Notifies the client that the volume level of an audio channel in the session submix has changed.
        /// </summary>
        /// <param name="channelCount">The channel count.</param>
        /// <param name="newVolumesArrayPtr">Pointer to an float array of volumes corresponding with each channel index.</param>
        /// <param name="channelIndex">The number of the channel whose volume level changed.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
        [PreserveSig]
        int OnChannelVolumeChanged(
            [In][MarshalAs(UnmanagedType.U4)] uint channelCount,
            [In][MarshalAs(UnmanagedType.SysInt)] IntPtr newVolumesArrayPtr,
            [In][MarshalAs(UnmanagedType.U4)] uint channelIndex,
            [In] ref Guid eventContext
            );

        /// <summary>
        /// Notifies the client that the grouping parameter for the session has changed.
        /// </summary>
        /// <param name="groupingId">The new grouping parameter for the session.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
        [PreserveSig]
        int OnGroupingParamChanged(
            [In] ref Guid groupingId,
            [In] ref Guid eventContext
            );

        /// <summary>
        /// Notifies the client that the stream-activity state of the session has changed.
        /// </summary>
        /// <param name="state">The new session state.</param>
        /// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
        [PreserveSig]
        int OnStateChanged([In] AudioSessionState state);

        /// <summary>
        /// Notifies the client that the session has been disconnected.
        /// </summary>
        /// <param name="disconnectReason">The reason that the audio session was disconnected.</param>
        /// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
        [PreserveSig]
        int OnSessionDisconnected([In] AudioSessionDisconnectReason disconnectReason);
    }
}