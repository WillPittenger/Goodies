---
external help file: WillPittenger.Goodies.PowerShell.Sounds.dll-Help.xml
Module Name: WillPittenger.Goodies.PowerShell.Sounds
online version:
schema: 2.0.0
---

# Get-Sound

## SYNOPSIS
Retrieves a sound from the file system.  The file can be either a predefined sound from the operating system or a sound file on your computer.  The supported file types depend on which operating system you have.  Windows has added built-in support for more formats over the years.

## SYNTAX

### Predefined (Default)
```
Get-Sound [-Predefined] {asterisk | beep | exclamation | hand | question | alarm01 | alarm02 | alarm03 | alarm04 | alarm05 | alarm06 | alarm07 | alarm08 | alarm09 | alarm10 | chimes | chord | ding | flourish | notify | oneStop | recycle | ring01 | ring02 | ring03 | ring04 | ring05 | ring06 | ring07 | ring08 | ring09 | ring10 | ringOut | speechDisambig | speechMisrecog | speechOff | speechOn | speechSleep | tada | town | windowsBkg | windowsBalloon | windowsBatteryCritical | windowsBatteryLow | windowsCriticalStop | default_ | windowsDing | error | windowsExclamation | windowsFeedDiscovered | windowsFg | windowsHardwareFail | windowsHardwareInsert | windowsHardwareRemove | windowsInfoBar | windowsLogOff | windowsLogOn | windowsMenuCmd | windowsMsgNudge | windowsMinimize | windowsNavStart | windowsNotifyCal | windowsNotifyEmail | windowsNotifyMessaging | windowsNotifySysGeneric | windowsNotifyUnspecified | windowsPopUpBlocked | windowsPrintComplete | windowsProximityConnection | windowsProximityNotification | windowsRecycle | windowsRestore | windowsRingIn | windowsRingOut | windowsShutDown | windowsStartUp | windowsUnlock | windowsUAC} [<CommonParameters>]
```

### Path
```
Get-Sound [-Path] <FileInfo>  [<CommonParameters>]
```

## DESCRIPTION
Get-Sound retrieves a structure that defines a sound.  Those structures are defined by WillPittenger.Goodies.Sounds, our backend.  The predefined sounds have a different class behind them than the file sounds do.  The basic type returned does allow looping of sounds.  However, some sounds can't be looped.  In that case, any attempt to loop them will be ignored.

You can either pass a sound to Start-SoundPlayback or call the Play/PlayLooping members.  They do the same thing.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-Sound tada
```

Retrieves the predefined system sound "tada" from Windows.  Predefined sounds use a different API to trigger the playback.

### Example 2
```powershell
PS C:\> Get-Sound ~\MySound.wav
```

This call returns a sound for a wav file in your profile.

## PARAMETERS

### -Path
Specifies the path to a sound file.  The file must exist.

```yaml
Type: System.IO.FileInfo
Parameter Sets: Path
Aliases: InputObject

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Predefined
Specifies a predefined sound from Windows.  Use a value from the WillPittenger.PowerShell.Goodies.Sounds.PredefinedSounds.SoundIDs enum type.

```yaml
Type: WillPittenger.Goodies.Sounds.PredefinedSounds+SoundIDs
Parameter Sets: Predefined
Aliases:
Accepted values: asterisk, beep, exclamation, hand, question, alarm01, alarm02, alarm03, alarm04, alarm05, alarm06, alarm07, alarm08, alarm09, alarm10, chimes, chord, ding, flourish, notify, oneStop, recycle, ring01, ring02, ring03, ring04, ring05, ring06, ring07, ring08, ring09, ring10, ringOut, speechDisambig, speechMisrecog, speechOff, speechOn, speechSleep, tada, town, windowsBkg, windowsBalloon, windowsBatteryCritical, windowsBatteryLow, windowsCriticalStop, default_, windowsDing, error, windowsExclamation, windowsFeedDiscovered, windowsFg, windowsHardwareFail, windowsHardwareInsert, windowsHardwareRemove, windowsInfoBar, windowsLogOff, windowsLogOn, windowsMenuCmd, windowsMsgNudge, windowsMinimize, windowsNavStart, windowsNotifyCal, windowsNotifyEmail, windowsNotifyMessaging, windowsNotifySysGeneric, windowsNotifyUnspecified, windowsPopUpBlocked, windowsPrintComplete, windowsProximityConnection, windowsProximityNotification, windowsRecycle, windowsRestore, windowsRingIn, windowsRingOut, windowsShutDown, windowsStartUp, windowsUnlock, windowsUAC

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

Either a file or a predefined system sound ID.

## OUTPUTS
An WillPittenger.Goodies.Sounds.ISound instance.

## NOTES
Unfortunately, operating systems other than Windows can't be supported as they don't have the sound API used.

## RELATED LINKS
[Start-SoundPlayback](Start-SoundPlayback.md)
[Stop-SoundPlayback](Stop-SoundPlayback.md)