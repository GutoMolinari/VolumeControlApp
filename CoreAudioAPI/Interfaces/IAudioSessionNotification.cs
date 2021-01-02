using System.Runtime.InteropServices;

namespace CoreAudioAPI.Interfaces
{
    /// <summary>
    /// Provides notification when an audio session is created.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd370969.aspx
    /// </remarks>
    [Guid("641DD20B-4D41-49CC-ABA3-174B9477BB08")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioSessionNotification
    {
        /// <summary>
        /// Notifies the registered processes that the audio session has been created.
        /// </summary>
        /// <param name="sessionControl">The <see cref="IAudioSessionControl"/> interface of the audio session that was created.</param>
        /// <returns>An HRESULT code indicating whether the operation succeeded of failed.</returns>
        [PreserveSig]
        int OnSessionCreated([In] IAudioSessionControl sessionControl);
    }
}