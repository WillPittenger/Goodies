namespace WillPittenger.Goodies.Sounds;

using Tools.Ext;

/// <summary>
/// Wraps a <see cref="System.IO.FileInfo"/> instance by passing that path to a new <see cref="System.Media.SoundPlayer"/> instance.  We store that.
/// </summary>
/// <remarks>
/// <para>The public constructors require the file to exist.  Those constructors that are marked <c><b>internal</b></c> are for use by this API.  It uses them
/// to create some of the predefined instances inside <see cref="PredefinedSounds"/>.</para>
/// </remarks>
public class SoundFile : ISound
{
	#region Constructors
		/// <summary>
		/// Constructs a <see cref="SoundFile"/> for the specified path.
		/// </summary>
		/// <param name="strFilePath">The path of the file you want to play.  This file must already exist.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="strFilePath"/> was <c><b>null</b></c></exception>
		/// <exception cref="System.ArgumentException"><paramref name="strFilePath"/> was an empty string</exception>
		/// <exception cref="System.IO.FileNotFoundException">The path in <paramref name="strFilePath"/> either wasn't a valid path or the file didn't exist.
		/// </exception>
		/// <exception cref="System.UriFormatException">The URL value specified by <paramref name="strFilePath"/> can't be resolved.</exception>
		public SoundFile(string strFilePath)
		{
			System.Diagnostics.Debug.Assert(strFilePath != null && strFilePath != "" && System.IO.File.Exists(strFilePath));
			switch(strFilePath)
			{
				case null:
					throw new System.ArgumentNullException(nameof(strFilePath));
				case "":
					throw new System.ArgumentException("Empty path given when a valid path was expected.", nameof(strFilePath));
			}
			if(!System.IO.File.Exists(strFilePath))
				throw new System.IO.FileNotFoundException("A valid path is required.", nameof(strFilePath));

			player = new(new SFML.Audio.SoundBuffer(strFilePath));

			fileSrc = new(strFilePath);
		}

		/// <summary>
		/// Specialized constructor corresponding to <see cref="SoundFile(string)"/>, but with the option to prepend the Windows media folder on the front.  If <paramref
		/// name="bUseSysMediaDir"/> is <c><b>true</b></c>, the constructor doesn't check to see if the file exists.  Otherwise, it does.
		/// </summary>
		/// <param name="strFilePath">The path of the file you want to play.  This file must already exist unless <paramref name="bUseSysMediaDir"/> is <c><b>false</b></c>.
		/// </param>
		/// <param name="bUseSysMediaDir">If <c><b>true</b></c>, the file is assumed to be a predefined file by Windows.  Furthermore, the constructor skips checks to ensure the
		/// file exists as not all predefined sound files exist on all Windows installations.  If <c><b>false</b></c>, the file must exist in the exact path specified.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="strFilePath"/> was <c><b>null</b></c></exception>
		/// <exception cref="System.ArgumentException"><paramref name="strFilePath"/> was an empty string</exception>
		/// <exception cref="System.IO.FileNotFoundException">The path in <paramref name="strFilePath"/> either wasn't a valid path or the file didn't exist.</exception>
		/// <exception cref="System.UriFormatException">The URL value specified by <paramref name="strFilePath"/> can't be resolved.</exception>
		internal SoundFile(string strFilePath, bool bUseSysMediaDir = false)
		{
			System.Diagnostics.Debug.Assert(strFilePath != null && strFilePath != "" && (bUseSysMediaDir || System.IO.File.Exists(strFilePath)));
			switch(strFilePath)
			{
				case null:
					throw new System.ArgumentNullException(nameof(strFilePath));
				case "":
					throw new System.ArgumentException("Empty path given when a valid path was expected.", nameof(strFilePath));
			}
			if(!bUseSysMediaDir && !System.IO.File.Exists(strFilePath))
				throw new System.IO.FileNotFoundException("A valid path is required if bUseSysMediaDir is false.", nameof(strFilePath));

			player = new(new SFML.Audio.SoundBuffer(bUseSysMediaDir ? System.IO.Path.Combine(dirSysMedia.FullName, strFilePath) : strFilePath));

			fileSrc = new(strFilePath);
		}

		/// <summary>
		/// Constructs a <see cref="SoundFile"/> for the specified path.
		/// </summary>
		/// <param name="fileSrc">The path of the file you want to play.  This file must already exist.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="fileSrc"/> was <c><b>null</b></c></exception>
		/// <exception cref="System.IO.FileNotFoundException">The path in <paramref name="fileSrc"/> either wasn't a valid path or the file didn't exist.</exception>
		/// <exception cref="System.UriFormatException">The URL value specified by <paramref name="fileSrc"/> can't be resolved.</exception>
		public SoundFile(System.IO.FileInfo fileSrc)
		{
			System.Diagnostics.Debug.Assert(fileSrc != null && fileSrc.Exists);
			System.ArgumentNullException.ThrowIfNull(fileSrc);
			if(!fileSrc.Exists)
				throw new System.IO.FileNotFoundException("You must specify a file that exists.");

			player = new(new SFML.Audio.SoundBuffer(fileSrc.ToString()));

			this.fileSrc = fileSrc;
		}

		/// <summary>
		/// Specialized constructor corresponding to <see cref="SoundFile(System.IO.FileInfo)"/>, but with the option to prepend the Windows media folder on the front.  If
		/// <paramref name="bUseSysMediaDir"/> is <c><b>true</b></c>, the constructor doesn't check to see if the file exists.  Otherwise, it does.
		/// </summary>
		/// <param name="fileSrc">The path of the file you want to play.  This file must already exist.</param>
		/// <param name="bUseSysMediaDir">If <c><b>true</b></c>, the file is assumed to be a predefined file by Windows.  Furthermore, the constructor skips checks to ensure the
		/// file exists as not all predefined sound files exist on all Windows installations.  If <c><b>false</b></c>, the file must exist in the exact path specified.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="fileSrc"/> was <c><b>null</b></c></exception>
		/// <exception cref="System.IO.FileNotFoundException">The path in <paramref name="fileSrc"/> either wasn't a valid path or the file didn't exist.</exception>
		/// <exception cref="System.UriFormatException">The URL value specified by <paramref name="fileSrc"/> can't be resolved.</exception>
		internal SoundFile(System.IO.FileInfo fileSrc, bool bUseSysMediaDir = false)
		{
			System.Diagnostics.Debug.Assert(fileSrc != null && (fileSrc.Exists || bUseSysMediaDir));
			System.ArgumentNullException.ThrowIfNull(fileSrc);
			if(!bUseSysMediaDir && !fileSrc.Exists)
				throw new System.IO.FileNotFoundException("If bUseSysMediaDir is false, the specified file must exist.", nameof(fileSrc));

			player = new(new SFML.Audio.SoundBuffer(bUseSysMediaDir ? System.IO.Path.Combine(dirSysMedia.FullName, fileSrc.ToString()) : fileSrc
				.ToString()));

			this.fileSrc = fileSrc;
		}

		/// <summary>
		/// <see cref="SoundFile"/> wraps <see cref="System.Media.SoundPlayer"/> which can receive a sound file in the form of a <see cref="System.IO.Stream"/>.  So this
		/// constructor provides that option to <see cref="SoundFile"/> callers.
		/// </summary>
		/// <param name="stream">The <see cref="System.IO.Stream"/> instance containing the sound data.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="stream"/> was <c><b>null</b></c></exception>
		/// <exception cref="System.ArgumentException"><paramref name="stream"/> reported it can't be read from.</exception>
		public SoundFile(System.IO.Stream stream)
		{
			System.Diagnostics.Debug.Assert(stream != null && stream.CanRead);
			System.ArgumentNullException.ThrowIfNull(stream);
			if(!stream.CanRead)
				throw new System.ArgumentException("Specified stream is unreadable", nameof(stream));

			player = new(new SFML.Audio.SoundBuffer(stream));

			fileSrc = null;
		}
	#endregion

	#region Delegates
	#endregion

	#region Events
	#endregion

	#region Constants
		/// <summary>
		/// This is where Windows normally stores its system sound files.
		/// </summary>
		public static readonly System.IO.DirectoryInfo dirSysMedia = new(System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment
			.SpecialFolder.Windows), "Media"));
	#endregion

	#region Helper Types
		public class Exception(in Exception.Reasons Reason, in string Desc, in System.Exception? Inner = null) : System.Exception(Desc, Inner)
		{
			public enum Reasons
			{
				playerBusy,
			}
		}
	#endregion

	#region Data Members
		/// <summary>
		/// This is the object we're wrapping.
		/// </summary>
		public readonly SFML.Audio.Sound player;


		public readonly System.IO.FileInfo? fileSrc;
	#endregion

	#region Properties
	#endregion

	#region Methods
		/// <summary>
		/// Plays the sound.  This might be played asynchronously.  It won't loop.  For looped sounds, see <see cref="PlayLooping"/>.  To ensure you get
		/// synchronous sound, use <see cref="PlaySync"/>.
		/// </summary>
		public void Play()
		{
			if(player.Status != SFML.Audio.SoundStatus.Stopped)
			{
				if(fileSrc is null)
					throw new Exception(Exception.Reasons.playerBusy, Rsrcs.strPlayerForSoundBusy);

				throw new Exception(Exception.Reasons.playerBusy, Rsrcs.strPlayerForSoundBusyWithFileName.Fmt(fileSrc.FullName));
			}

			player.Loop = false;
			player.Play();
		}

		/// <summary>
		/// Play the sound so that it loops continuously.  Call <see cref="Stop"/> to stop the playback loop.  Note that looping isn't supported by some
		/// predefined sounds in <see cref="PredefinedSounds"/>.  See that class for more details.
		/// </summary>
		public void PlayLooping()
		{
			if(player.Status != SFML.Audio.SoundStatus.Stopped)
			{
				if(fileSrc is null)
					throw new Exception(Exception.Reasons.playerBusy, Rsrcs.strPlayerForSoundBusy);

				throw new Exception(Exception.Reasons.playerBusy, Rsrcs.strPlayerForSoundBusyWithFileName.Fmt(fileSrc.FullName));
			}

			player.Loop = true;
			player.Play();
		}

		/// <summary>
		/// Same as <see cref="Play"/> in some cases.  However, instances of <see cref="SoundFile"/> will attempt to play back the sound entirely before <see
		/// cref="PlaySync"/> returns.  For instances of <see cref="PredefinedSounds.SysSound"/>, this is the same as a call to <see cref="Play"/> as the sound
		/// files are preloaded by Windows.
		/// </summary>
		/// <remarks>
		///		<para>SFML doesn't have a synchronous option.  We needed it for non-Windows support.  Function may be removed at a later date.  It does nothing.</para>
		/// </remarks>
		[System.Obsolete(@"SFML doesn't have a synchronous option.  We needed it for non-Windows support.  Function may be removed at a later date.  It does " +
			@"nothing.")]
		public void PlaySync()
		{
		}

		/// <summary>
		/// If you started playback with <see cref="PlayLooping"/>, this stops playback.
		/// </summary>
		public void Stop()
				=> player.Stop();
	#endregion

	#region Event Handlers
	#endregion
}