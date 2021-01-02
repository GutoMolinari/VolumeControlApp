namespace CoreAudio.Enums
{
    /// <summary>
    /// Defines constants that indicate the direction in which audio data flows between an audio endpoint device and an application.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd370828.aspx
    /// </remarks>
    public enum DataFlow
    {
        /// <summary>
        /// Audio data flows from the application to the audio endpoint device, which renders the stream.
        /// </summary>
        Render = 0,

        /// <summary>
        /// Audio data flows from the audio endpoint device that captures the stream, to the application.
        /// </summary>
        Capture = 1,

        /// <summary>
        /// Audio data can flow either from the application to the audio endpoint device, or from the audio endpoint device to the application.
        /// </summary>
        All = 2
    }
}