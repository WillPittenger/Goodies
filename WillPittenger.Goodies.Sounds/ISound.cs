namespace WillPittenger.Goodies.Sounds
{
	/// <summary>
	/// Common interface for all sound classes declared by <see cref="Sounds"/>.  It declares a few methods that can be called even if you don't know the class
	/// you received.  All implementations are wrappers around .NET classes, but .NET has two different classes that <see cref="ISound"/> provides a way to
	/// treat as one.
	/// </summary>
	public interface ISound
	{
		/// <summary>
		/// Plays the sound.  This might be played asynchronously.  It won't loop.  For looped sounds, see <see cref="PlayLooping"/>.  To ensure you get
		/// synchronous sound, use <see cref="PlaySync"/>.
		/// </summary>
		void Play();

		/// <summary>
		/// Play the sound so that it loops continuously.  Call <see cref="Stop"/> to stop the playback loop.  Note that looping isn't supported by some
		/// predefined sounds in <see cref="PredefinedSounds"/>.  See that class for more details.
		/// </summary>
		void PlayLooping();

		/// <summary>
		/// Same as <see cref="Play"/> in some cases.  However, instances of <see cref="SoundFile"/> will attempt to play back the sound entirely before <see
		/// cref="PlaySync"/> returns.  For instances of <see cref="PredefinedSounds.SysSound"/>, this is the same as a call to <see cref="Play"/> as the sound
		/// files are preloaded by Windows.
		/// </summary>
		void PlaySync();

		/// <summary>
		/// If you started playback with <see cref="PlayLooping"/>, this stops playback.
		/// </summary>
		void Stop();
	}
}