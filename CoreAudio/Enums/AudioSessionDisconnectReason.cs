namespace CoreAudio.Enums
{
    /// <summary>
    /// Defines constants that indicate a reason for an audio session being disconnected.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: Unknown
    /// </remarks>
    public enum AudioSessionDisconnectReason
    {
        /// <summary>
        /// The user removed the audio endpoint device.
        /// </summary>
        DisconnectReasonDeviceRemoval = 0,

        /// <summary>
        /// The Windows audio service has stopped.
        /// </summary>
        DisconnectReasonServerShutdown = 1,

        /// <summary>
        /// The stream format changed for the device that the audio session is connected to.
        /// </summary>
        DisconnectReasonFormatChanged = 2,

        /// <summary>
        /// The user logged off the WTS session that the audio session was running in.
        /// </summary>
        DisconnectReasonSessionLogoff = 3,

        /// <summary>
        /// The WTS session that the audio session was running in was disconnected.
        /// </summary>
        DisconnectReasonSessionDisconnected = 4,

        /// <summary>
        /// The (shared-mode) audio session was disconnected to make the audio endpoint device available for an exclusive-mode connection.
        /// </summary>
        DisconnectReasonExclusiveModeOverride = 5
    }
}