---
Module Name: WillPittenger.Goodies.PowerShell.Sounds
Module Guid: 9b641a82-0da7-4407-8218-b3e7a582284b
Download Help Link: {{ Update Download Link }}
Help Version: {{ Please enter version of help manually (X.X.X.X) format }}
Locale: en-US
---

# WillPittenger.Goodies.PowerShell.Sounds Module
## Description
Provides a collection of tools related to playback of sounds in PowerShell.  Use them when you, for instance, want to play a sound to indicate a status like an error or completion of a task.

## WillPittenger.Goodies.PowerShell.Sounds Cmdlets
### [Get-Sound](Get-Sound.md)
Retrieves an object corresponding to either a predefined sound from Windows or a file on your drive.  You can then either pass that to [Start-SoundPlayback](Start-SoundPlayback.md) and [Stop-SoundPlayback](Stop-SoundPlayback.md) or call the methods inside the object.

### 
Starts the playback of a sound.  You can use it the same way you would [Get-Sound](Get-Sound.md) by passing it the same parameters or call [Get-Sound](Get-Sound.md) first and pass that value to [Start-SoundPlayback](Start-SoundPlayback.md).  Both work.  If you turn on -Loop, pass the return value from [Start-SoundPlayback](Start-SoundPlayback.md) to [Stop-SoundPlayback](Stop-SoundPlayback.md) in order to stop it.

### [Stop-SoundPlayback](Stop-SoundPlayback.md)
Stops the playback of a looping sound you started with [Start-SoundPlayback](Start-SoundPlayback.md).

