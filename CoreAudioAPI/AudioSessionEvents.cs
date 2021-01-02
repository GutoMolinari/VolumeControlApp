using CoreAudioAPI.Enums;
using CoreAudioAPI.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace CoreAudioAPI
{
    /// <summary>
    /// Provides notifications of session-related events such as changes in the volume level, display name, and session state.
    /// For more information, see <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd368289(v=vs.85).aspx"/>.
    /// </summary>
    [Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8")]
    internal sealed class AudioSessionEvents : IAudioSessionEvents
    {
        /// <summary>
        /// Occurs when the display name for the session has changed.
        /// </summary>
        public event EventHandler<AudioSessionDisplayNameChangedEventArgs> DisplayNameChanged;

        /// <summary>
        /// Occurs when the display icon for the session has changed.
        /// </summary>
        public event EventHandler<AudioSessionIconPathChangedEventArgs> IconPathChanged;

        /// <summary>
        /// Occurs when the volume level or muting state of the session has changed.
        /// </summary>
        public event EventHandler<AudioSessionSimpleVolumeChangedEventArgs> SimpleVolumeChanged;

        /// <summary>
        /// Occurs when the volume level of an audio channel in the session submix has changed.
        /// </summary>
        public event EventHandler<AudioSessionChannelVolumeChangedEventArgs> ChannelVolumeChanged;

        /// <summary>
        /// Occurs when the grouping parameter for the session has changed.
        /// </summary>
        public event EventHandler<AudioSessionGroupingParamChangedEventArgs> GroupingParamChanged;

        /// <summary>
        /// Occurs when the stream-activity state of the session has changed.
        /// </summary>
        public event EventHandler<AudioSessionStateChangedEventArgs> StateChanged;

        /// <summary>
        /// Occurs when the session has been disconnected.
        /// </summary>
        public event EventHandler<AudioSessionDisconnectedEventArgs> SessionDisconnected;

        /// <summary>
        /// Notifies the client that the display name for the session has changed.
        /// </summary>
        /// <param name="newDisplayName">The new display name for the session. </param>
        /// <param name="eventContext">The event context value.</param>
        /// <returns>HRESULT</returns>
        int IAudioSessionEvents.OnDisplayNameChanged(string newDisplayName, ref Guid eventContext)
        {
            DisplayNameChanged?.Invoke(this, new AudioSessionDisplayNameChangedEventArgs(newDisplayName, eventContext));
            return 0;
        }

        /// <summary>
        /// Notifies the client that the display icon for the session has changed.
        /// </summary>
        /// <param name="newIconPath">The path for the new display icon for the session.</param>
        /// <param name="eventContext">The event context value.</param>
        /// <returns>HRESULT</returns>
        int IAudioSessionEvents.OnIconPathChanged(string newIconPath, ref Guid eventContext)
        {
            IconPathChanged?.Invoke(this, new AudioSessionIconPathChangedEventArgs(newIconPath, eventContext));
            return 0;
        }

        /// <summary>
        /// Notifies the client that the volume level or muting state of the audio session has changed.
        /// </summary>
        /// <param name="newVolume">
        /// The new volume level for the audio session. This parameter is a value in the range 0.0 to 1.0, 
        /// where 0.0 is silence and 1.0 is full volume (no attenuation).
        /// </param>
        /// <param name="newMute">The new muting state. If TRUE, muting is enabled. If FALSE, muting is disabled.</param>
        /// <param name="eventContext">The event context value.</param>
        /// <returns>HRESULT</returns>
        int IAudioSessionEvents.OnSimpleVolumeChanged(float newVolume, bool newMute, ref Guid eventContext)
        {
            SimpleVolumeChanged?.Invoke(this, new AudioSessionSimpleVolumeChangedEventArgs(newVolume, newMute, eventContext));
            return 0;
        }

        /// <summary>
        /// Notifies the client that the volume level of an audio channel in the session submix has changed.
        /// </summary>
        /// <param name="channelCount">The number of channels in the session submix.</param>
        /// <param name="newChannelVolumeArrayPtr">An array of volume levels. Each element is a value of type float that specifies the volume level for a particular channel. Each volume level is a value in the range 0.0 to 1.0, where 0.0 is silence and 1.0 is full volume (no attenuation). The number of elements in the array is specified by the ChannelCount parameter.</param>
        /// <param name="changedChannel">The number of the channel whose volume level changed.</param>
        /// <param name="eventContext">The event context value.</param>
        /// <returns></returns>
        int IAudioSessionEvents.OnChannelVolumeChanged(
            uint channelCount, 
            IntPtr newChannelVolumeArrayPtr,
            uint changedChannel, 
            ref Guid eventContext
            )
        {
            ChannelVolumeChanged?.Invoke(this, new AudioSessionChannelVolumeChangedEventArgs(channelCount, newChannelVolumeArrayPtr, changedChannel, eventContext));
            return 0;
        }

        /// <summary>
        /// Notifies the client that the grouping parameter for the session has changed.
        /// </summary>
        /// <param name="newGroupingParam">The new grouping parameter for the session. This parameter points to a grouping-parameter GUID.</param>
        /// <param name="eventContext">The event context value.</param>
        /// <returns>HRESULT</returns>
        int IAudioSessionEvents.OnGroupingParamChanged(ref Guid newGroupingParam, ref Guid eventContext)
        {
            GroupingParamChanged?.Invoke(this, new AudioSessionGroupingParamChangedEventArgs(newGroupingParam, eventContext));
            return 0;
        }

        /// <summary>
        /// Notifies the client that the stream-activity state of the session has changed.
        /// </summary>
        /// <param name="newState">The new session state.</param>
        /// <returns>HRESULT</returns>
        int IAudioSessionEvents.OnStateChanged(AudioSessionState newState)
        {
            StateChanged?.Invoke(this, new AudioSessionStateChangedEventArgs(newState));
            return 0;
        }

        /// <summary>
        /// Notifies the client that the audio session has been disconnected.
        /// </summary>
        /// <param name="disconnectReason">The reason that the audio session was disconnected.</param>
        /// <returns>HRESULT</returns>
        int IAudioSessionEvents.OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
        {
            SessionDisconnected?.Invoke(this, new AudioSessionDisconnectedEventArgs(disconnectReason));
            return 0;
        }
    }

    /// <summary>
    /// A base class for all event-args classes which specify an <see cref="EventContext"/> value.
    /// </summary>
    public abstract class AudioSessionEventContextEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the event context value.
        /// </summary>
        public Guid EventContext { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioSessionEventContextEventArgs"/> class.
        /// </summary>
        /// <param name="eventContext">The event context value.</param>
        protected AudioSessionEventContextEventArgs(Guid eventContext)
        {
            EventContext = eventContext;
        }
    }

    /// <summary>
    /// Provides data for the <see cref="AudioSessionEvents.ChannelVolumeChanged"/> event.
    /// </summary>
    public class AudioSessionChannelVolumeChangedEventArgs : AudioSessionEventContextEventArgs
    {
        /// <summary>
        /// Gets the number of audio channels in the session submix.
        /// </summary>
        public uint ChannelCount { get; private set; }

        /// <summary>
        /// Gets the volume level for each audio channel. Each volume level is a value in the range 0.0 to 1.0, where 0.0 is silence and 1.0 is full volume.
        /// </summary>
        public float[] ChannelVolumes { get; private set; }

        /// <summary>
        /// Gets the index of the audio channel that changed. Use this value as an index into the <see cref="ChannelVolumes"/>.
        /// If the session submix contains n channels, the channels are numbered from 0 to n– 1. If more than one channel might have changed, the value of ChangedChannel is (DWORD)(–1).
        /// </summary>
        public uint ChangedChannel { get; private set; }

        /// <summary>
        /// Gets the volume of the channel specified by the <paramref name="channelIndex"/>.
        /// </summary>
        /// <param name="channelIndex">The zero-based index of the channel.</param>
        /// <returns>Volume level of the specified channelIndex in the range 0.0 to 1.0, where 0.0 is silence and 1.0 is full volume.</returns>
        public float this[int channelIndex]
        {
            get { return ChannelVolumes[channelIndex]; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioSessionChannelVolumeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="channelCount">The number of channels.</param>
        /// <param name="channelVolumes">Volumes of the channels.</param>
        /// <param name="changedChannel">Number of channel volumes changed.</param>
        /// <param name="eventContext">Userdefined event context.</param>
        public AudioSessionChannelVolumeChangedEventArgs(
            uint channelCount, 
            IntPtr newChannelVolumeArrayPtr, 
            uint changedChannel,
            Guid eventContext
            ) : base(eventContext)
        {
            var channelVolumes = new float[channelCount];
            Marshal.Copy(newChannelVolumeArrayPtr, channelVolumes, 0, (int)channelCount);

            if (channelCount < 0 || channelCount != channelVolumes.Length)
                throw new ArgumentOutOfRangeException(nameof(channelCount));

            ChannelCount = channelCount;
            ChannelVolumes = channelVolumes;
            ChangedChannel = changedChannel;
        }
    }

    /// <summary>
    /// Provides data for the <see cref="AudioSessionEvents.SessionDisconnected"/> event.
    /// </summary>
    public class AudioSessionDisconnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the reason that the audio session was disconnected.
        /// </summary>
        public AudioSessionDisconnectReason DisconnectReason { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioSessionDisconnectedEventArgs"/>  class.
        /// </summary>
        /// <param name="disconnectReason">The reason that the audio session was disconnected.</param>
        public AudioSessionDisconnectedEventArgs(AudioSessionDisconnectReason disconnectReason)
        {
            DisconnectReason = disconnectReason;
        }
    }

    /// <summary>
    /// Provides data for the <see cref="AudioSessionEvents.DisplayNameChanged"/> event.
    /// </summary>
    public class AudioSessionDisplayNameChangedEventArgs : AudioSessionEventContextEventArgs
    {
        /// <summary>
        /// Gets the new display name the session.
        /// </summary>
        public string NewDisplayName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioSessionDisplayNameChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newDisplayName">Thew new display name of the session.</param>
        /// <param name="eventContext">The event context value.</param>
        public AudioSessionDisplayNameChangedEventArgs(string newDisplayName, Guid eventContext)
            : base(eventContext)
        {
            NewDisplayName = newDisplayName;
        }
    }

    /// <summary>
    /// Provides data for the <see cref="AudioSessionEvents.GroupingParamChanged"/> event.
    /// </summary>
    public class AudioSessionGroupingParamChangedEventArgs : AudioSessionEventContextEventArgs
    {
        /// <summary>
        /// Gets the new grouping parameter for the session.
        /// </summary>
        public Guid NewGroupingParam { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioSessionGroupingParamChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newGroupingParam">The new grouping parameter for the session.</param>
        /// <param name="eventContext">The event context value.</param>
        public AudioSessionGroupingParamChangedEventArgs(Guid newGroupingParam, Guid eventContext)
            : base(eventContext)
        {
            NewGroupingParam = newGroupingParam;
        }
    }

    /// <summary>
    /// Provides data for the <see cref="AudioSessionEvents.IconPathChanged"/> event.
    /// </summary>
    public class AudioSessionIconPathChangedEventArgs : AudioSessionEventContextEventArgs
    {
        /// <summary>
        /// Gets the path for the new display icon for the session.
        /// </summary>
        public string NewIconPath { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioSessionIconPathChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newIconPath">The path for the new display icon for the session.</param>
        /// <param name="eventContext">The event context value.</param>
        public AudioSessionIconPathChangedEventArgs(string newIconPath, Guid eventContext)
            : base(eventContext)
        {
            NewIconPath = newIconPath;
        }
    }

    /// <summary>
    /// Provides data for the <see cref="AudioSessionEvents.SimpleVolumeChanged"/> event.
    /// </summary>
    public class AudioSessionSimpleVolumeChangedEventArgs : AudioSessionEventContextEventArgs
    {
        /// <summary>
        /// Gets the new volume level for the audio session. 
        /// </summary>
        /// <remarks>The value is a value in the range 0.0 to 1.0, where 0.0 is silence and 1.0 is full volume (no attenuation).</remarks>
        public float NewVolume { get; private set; }

        /// <summary>
        /// Gets the new muting state.
        /// </summary>
        /// <remarks>If true, muting is enabled. If false, muting is disabled.</remarks>
        public bool IsMuted { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioSessionSimpleVolumeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newVolume">The new volume level for the audio session. This parameter is a value in the range 0.0 to 1.0, where 0.0 is silence and 1.0 is full volume (no attenuation).</param>
        /// <param name="isMuted">The muting state. If true, muting is enabled. If false, muting is disabled.</param>
        /// <param name="eventContext">The event context value.</param>
        public AudioSessionSimpleVolumeChangedEventArgs(float newVolume, bool isMuted, Guid eventContext)
            : base(eventContext)
        {
            if (newVolume < 0 || newVolume > 1)
                throw new ArgumentOutOfRangeException("newVolume");
            NewVolume = newVolume;
            IsMuted = isMuted;
        }
    }

    /// <summary>
    /// Provides data for the <see cref="AudioSessionEvents.StateChanged"/> event.
    /// </summary>
    public class AudioSessionStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the new session state.
        /// </summary>
        public AudioSessionState NewState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioSessionStateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newState"></param>
        public AudioSessionStateChangedEventArgs(AudioSessionState newState)
        {
            NewState = newState;
        }
    }
}