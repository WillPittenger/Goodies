namespace WillPittenger.Goodies.PowerShell.Sounds;

/// <summary>
/// Starts playback of a sound.  This can be either one you obtained from <see cref="GetSound"/> or you can pass the same information <see cref="GetSound"/>
/// takes directly to <see cref="StartSoundPlayback"/>.
/// </summary>
/// <see cref="GetSound"/>
/// <see cref="Goodies.Sounds.ISound"/>
/// <see cref="StopSoundPlayback"/>
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsLifecycle.Start, "SoundPlayback", DefaultParameterSetName = strPathParamSetName)]
public class StartSoundPlayback : System.Management.Automation.PSCmdlet
{
	/// <summary>
	/// Constructs an instance.  You'll never need to call this yourself, even from C#.
	/// </summary>
	public StartSoundPlayback()
	{
		SoundToPlay = Goodies.Sounds.PredefinedSounds.AllPredefinedSounds[Goodies.Sounds.PredefinedSounds.SoundIDs.beep];
		Path = new("c:/");
		Predefined = Goodies.Sounds.PredefinedSounds.SoundIDs.beep;
	}

	/// <summary>
	/// Just a string used to avoid magic values.
	/// </summary>
	internal const string strPiplineParamSetName = "Pipeline";

	/// <summary>
	/// Just a string used to avoid magic values.
	/// </summary>
	internal const string strPathParamSetName = "Path";

	/// <summary>
	/// Just a string used to avoid magic values.
	/// </summary>
	internal const string strPredefinedParamSetName = "Predefined";

	/// <summary>
	/// Use to pipe a value from <see cref="GetSound"/>
	/// </summary>
	[System.Management.Automation.Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ParameterSetName = strPiplineParamSetName, HelpMessage =
		"Use to pipe a value from Get-Sound.")]
	[System.Management.Automation.Alias("InputObject")]
	[System.Management.Automation.ValidateNotNull]
	public Goodies.Sounds.ISound SoundToPlay
	{
		get;
		set;
	}

	/// <summary>
	/// Specifies the path to a sound file.  The file must exist.
	/// </summary>
	[System.Management.Automation.Parameter(Position = 0, Mandatory = true, ParameterSetName = strPathParamSetName, HelpMessage = "Specifies the path to a " +
		"sound file.  The file must exist.")]
	[System.Management.Automation.ValidateNotNullOrEmpty]
	public System.IO.FileInfo Path
	{
		get;
		set;
	}

	/// <summary>
	/// Specifies a predefined sound from Windows.  Use a value from the <see cref="Goodies.Sounds.PredefinedSounds.SoundIDs"/> <c><b>enum</b></c> type.
	/// </summary>
	[System.Management.Automation.Parameter(Position = 0, Mandatory = true, ParameterSetName = strPredefinedParamSetName, HelpMessage = "Specifies a predefined"
		+ " sound from Windows.  Use a value from the Goodies.Sounds.PredefinedSounds.SoundIDs enum type.")]
	public Goodies.Sounds.PredefinedSounds.SoundIDs Predefined
	{
		get;
		set;
	}

	/// <summary>
	/// Causes the sound to play endlessly if the playback mechanism supports it.  Use <see cref="StopSoundPlayback"/> to stop playback.  Overrides <see
	/// cref="Sync"/>.
	/// </summary>
	[System.Management.Automation.Parameter(HelpMessage = "Causes the sound to play endlessly if the playback mechanism supports it.  Use Stop-SoundPlayback to"
		+ " stop playback of the sound.  Overrides -Sync.")]
	public System.Management.Automation.SwitchParameter Loop
	{
		get;
		set;
	}

	/// <summary>
	/// Prevents <see cref="StartSoundPlayback"/> from returning until the sound has played if the playback mechanism supports it.  Has no effect if <see
	/// cref="Loop"/> is on.
	/// </summary>
	[System.Management.Automation.Parameter(HelpMessage = "Prevents Start-SoundPlayback from returning until the sound has played if the playback mechanism " +
		"supports it.  Has no effect if -Loop was specified.")]
	public System.Management.Automation.SwitchParameter Sync
	{
		get;
		set;
	}

	/// <summary>
	/// This is what runs the command.  See <see cref="System.Management.Automation.Cmdlet.ProcessRecord"/> for more information.
	/// </summary>
	protected override void ProcessRecord()
	{
		base.ProcessRecord();

		switch(ParameterSetName)
		{
			case strPredefinedParamSetName:
				SoundToPlay = Goodies.Sounds.PredefinedSounds.AllPredefinedSounds[Predefined];

				break;

			case strPathParamSetName:
				SoundToPlay = new Goodies.Sounds.SoundFile(Path);

				break;
		}

		if(Loop.IsPresent)
			SoundToPlay.PlayLooping();
		else if(Sync.IsPresent)
			SoundToPlay.PlaySync();
		else
			SoundToPlay.Play();
	}
}