namespace CoreAudio.Enums
{
    /// <summary>
    /// Defines constants that indicate the current state of an audio session.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd370792.aspx
    /// </remarks>
    public enum AudioSessionState
    {
        /// <summary>
        /// The audio session is inactive.
        /// </summary>
        AudioSessionStateInactive = 0,

        /// <summary>
        /// The audio session is active.
        /// </summary>
        AudioSessionStateActive = 1,

        /// <summary>
        /// The audio session has expired.
        /// </summary>
        AudioSessionStateExpired = 2
    }
}