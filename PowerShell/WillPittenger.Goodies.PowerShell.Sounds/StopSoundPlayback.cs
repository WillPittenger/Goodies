namespace WillPittenger.Goodies.PowerShell.Sounds;

/// <summary>
/// If you called <see cref="StartSoundPlayback"/> with <see cref="StartSoundPlayback.Loop"/> set to true, that sound might play endlessly if the playback 
/// mechanism supports it.  Call <see cref="StopSoundPlayback"/> with the same <see cref="Goodies.Sounds.ISound"/> instance to end playback or call <see
/// cref="Goodies.Sounds.ISound.Stop"/>.
/// </summary>
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsLifecycle.Stop, "Sound")]
public class StopSoundPlayback : System.Management.Automation.PSCmdlet
{
	/// <summary>
	/// Constructs an instance.  You'll never need to call this yourself, even from C#.
	/// </summary>
	public StopSoundPlayback()
		=> SoundToStop = Goodies.Sounds.PredefinedSounds.AllPredefinedSounds[Goodies.Sounds.PredefinedSounds.SoundIDs.beep];

	/// <summary>
	/// Use to pipe a value from <see cref="GetSound"/>
	/// </summary>
	[System.Management.Automation.Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, HelpMessage = "Use to pipe a value from Get-Sound.")]
	[System.Management.Automation.Alias("InputObject")]
	[System.Management.Automation.ValidateNotNull]
	public Goodies.Sounds.ISound SoundToStop
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

		SoundToStop.Stop();
	}
}