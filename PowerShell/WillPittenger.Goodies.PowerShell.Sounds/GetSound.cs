namespace WillPittenger.Goodies.PowerShell.Sounds;

/// <summary>
/// Looks up a sound.  This can be either from a file name by passing <c>-Path</c> or one of the predefined sounds from Windows whose ID values are declared in
/// the <see cref="Goodies.Sounds.PredefinedSounds.SoundIDs"/> <c><b>enum</b></c> type.  For the later, pass <c>-Predefined</c>.
/// </summary>
/// <remarks>
/// All the functionality of this cmdlet is also provided in <see cref="StartSoundPlayback"/>.  Use <see cref="GetSound"/> if you want to store the sound
/// provided.  The return value will be an <see cref="Goodies.Sounds.ISound"/> instance.  Once you have an instance, you can either pass it to <see
/// cref="StartSoundPlayback"/> or call <see cref="Goodies.Sounds.ISound.Play"/>.
/// </remarks>
/// <seealso cref="Goodies.Sounds.ISound"/>
/// <seealso cref="Goodies.Sounds.ISound.Play"/>
/// <seealso cref="Goodies.Sounds.ISound.PlayLooping"/>
/// <seealso cref="Goodies.Sounds.ISound.PlaySync"/>
/// <seealso cref="StartSoundPlayback"/>
/// <seealso cref="StopSoundPlayback"/>
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Get, "Sound", DefaultParameterSetName = strPredefinedParamSetName)]
[System.Management.Automation.OutputType(typeof(Goodies.Sounds.ISound))]
public class GetSound : System.Management.Automation.PSCmdlet
{
	/// <summary>
	/// Constructs an instance.  You'll never need to call this yourself, even from C#.
	/// </summary>
	public GetSound()
	{
		Path = new("c:/");
		Predefined = Goodies.Sounds.PredefinedSounds.SoundIDs.beep;
	}

	/// <summary>
	/// Just a string used to avoid magic values.
	/// </summary>
	internal const string strPathParamSetName = "Path";

	/// <summary>
	/// Just a string used to avoid magic values.
	/// </summary>
	internal const string strPredefinedParamSetName = "Predefined";

	/// <summary>
	/// Specifies the path to a sound file.  The file must exist.
	/// </summary>
	[System.Management.Automation.Parameter(Position = 0, Mandatory = true, ParameterSetName = strPathParamSetName, ValueFromPipeline = true, HelpMessage =
		"Specifies the path to a sound file.  The file must exist.")]
	[System.Management.Automation.Alias("InputObject")]
	[System.Management.Automation.ValidateNotNullOrEmpty]
	public System.IO.FileInfo Path
	{
		get;
		set;
	}

	/// <summary>
	/// Specifies a predefined sound from Windows.  Use a value from the <see cref="Goodies.Sounds.PredefinedSounds.SoundIDs"/> <c><b>enum</b></c> type.
	/// </summary>
	[System.Management.Automation.Parameter(Position = 0, Mandatory = true, ParameterSetName = strPredefinedParamSetName, HelpMessage = "Specifies a predefined " +
		"sound from Windows.  Use a value from the WillPittenger.PowerShell.Goodies.Sounds.PredefinedSounds.SoundIDs enum type.")]
	public Goodies.Sounds.PredefinedSounds.SoundIDs Predefined
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
			case strPathParamSetName:
				WriteObject(new Goodies.Sounds.SoundFile(Path));

				break;

			case strPredefinedParamSetName:
				WriteObject(Goodies.Sounds.PredefinedSounds.AllPredefinedSounds[Predefined]);

				break;
		}
	}
}