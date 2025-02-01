---
external help file: WillPittenger.Goodies.PowerShell.Sounds.dll-Help.xml
Module Name: WillPittenger.Goodies.PowerShell.Sounds
online version:
schema: 2.0.0
---

# Stop-SoundPlayback

## SYNOPSIS
Stops the playback of a sound that you started playback of with Start-Playback.

## SYNTAX

```
Stop-SoundPlayback [-SoundToStop] <ISound> [<CommonParameters>]
```

## DESCRIPTION
In order to use Stop-SoundPlayback, save the return value or values (if you piped in many sounds) from Start-SoundPlayback.  Then pass that value to Stop-SoundPlayback.

## EXAMPLES

### Example 1
```powershell
PS C:\> $sound = Start-SoundPlayback ~\MySound.wav -Loop
PS C:\> Stop-SoundPlayback $sound
PS c:\> delete variable:sound
```

This code starts playback of a sound from your Windows profile directory.  It stores the sound in $sound and the stops it with Stop-Playback.  (This sample then deletes $sound to avoid cluttering up the variable space.)

## PARAMETERS

### -SoundToStop
Use to pipe a value from Get-Sound.

```yaml
Type: WillPittenger.Goodies.Sounds.ISound
Parameter Sets: (All)
Aliases: InputObject

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```


### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### WillPittenger.Goodies.Sounds.ISound

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
[Get-Sound](Get-Sound.md)
[Start-SoundPlayback](Start-SoundPlayback.md)