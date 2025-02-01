---
external help file: WillPittenger.Goodies.PowerShell.Sounds.dll-Help.xml
Module Name: WillPittenger.Goodies.PowerShell.Sounds
online version:
schema: 2.0.0
---

# Start-SoundPlayback

## SYNOPSIS
Starts playing any sound.  You can identify the sound via the pipeline from Get-Sound, a predefined sound ID, or a file name.

## SYNTAX

### Path (Default)
```
Start-SoundPlayback [-Path] <FileInfo> [-Loop] [-Sync]
 [<CommonParameters>]
```

### Pipeline
```
Start-SoundPlayback [-SoundToPlay] <ISound> [-Loop] [-Sync]
 [<CommonParameters>]
```

### Predefined
```
Start-SoundPlayback [-Predefined] {asterisk | beep | exclamation | hand | question | alarm01 | alarm02 | alarm03 | alarm04 | alarm05 | alarm06 | alarm07 | alarm08 | alarm09 | alarm10 | chimes | chord | ding | flourish | notify | oneStop | recycle | ring01 | ring02 | ring03 | ring04 | ring05 | ring06 | ring07 | ring08 | ring09 | ring10 | ringOut | speechDisambig | speechMisrecog | speechOff | speechOn | speechSleep | tada | town | windowsBkg | windowsBalloon | windowsBatteryCritical | windowsBatteryLow | windowsCriticalStop | default_ | windowsDing | error | windowsExclamation | windowsFeedDiscovered | windowsFg | windowsHardwareFail | windowsHardwareInsert | windowsHardwareRemove | windowsInfoBar | windowsLogOff | windowsLogOn | windowsMenuCmd | windowsMsgNudge | windowsMinimize | windowsNavStart | windowsNotifyCal | windowsNotifyEmail | windowsNotifyMessaging | windowsNotifySysGeneric | windowsNotifyUnspecified | windowsPopUpBlocked | windowsPrintComplete | windowsProximityConnection | windowsProximityNotification | windowsRecycle | windowsRestore | windowsRingIn | windowsRingOut | windowsShutDown | windowsStartUp | windowsUnlock | windowsUAC} [-Loop] [-Sync] [<CommonParameters>]
```

## DESCRIPTION
Starts playback of the specified sound.  If you pipe multiple sounds in, they might be played at the same time.  Not all sounds can be played in a loop.  If so, the -Loop parameter will be ignored.

## EXAMPLES

### Example 1
```powershell
PS C:\> Start-SoundPlayback tada
```

Starts playing the predefined sound "tada".

### Example 2
```powershell
PS C:\> Get-Sound tada | Start-SoundPlayback
```

Same as above, but piped to Start-SoundBack.

### Example 3
```powershell
PS C:\> Start-SoundPlayback ~\MySound.wav -Loop
```

Plays a sound from the Windows profile directory continuously.

## PARAMETERS

### -Loop
Causes the sound to play endlessly if the playback mechanism supports it.  Use Stop-SoundPlayback to stop playback of the sound.  Overrides -Sync.

```yaml
Type: System.Management.Automation.SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Path
Specifies the path to a sound file.  The file must exist.

```yaml
Type: System.IO.FileInfo
Parameter Sets: Path
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Predefined
Specifies a predefined sound from Windows.  Use a value from the Goodies.Sounds.PredefinedSounds.SoundIDs enum type.

```yaml
Type: WillPittenger.Goodies.Sounds.PredefinedSounds+SoundIDs
Parameter Sets: Predefined
Aliases:
Accepted values: asterisk, beep, exclamation, hand, question, alarm01, alarm02, alarm03, alarm04, alarm05, alarm06, alarm07, alarm08, alarm09, alarm10, chimes, chord, ding, flourish, notify, oneStop, recycle, ring01, ring02, ring03, ring04, ring05, ring06, ring07, ring08, ring09, ring10, ringOut, speechDisambig, speechMisrecog, speechOff, speechOn, speechSleep, tada, town, windowsBkg, windowsBalloon, windowsBatteryCritical, windowsBatteryLow, windowsCriticalStop, default_, windowsDing, error, windowsExclamation, windowsFeedDiscovered, windowsFg, windowsHardwareFail, windowsHardwareInsert, windowsHardwareRemove, windowsInfoBar, windowsLogOff, windowsLogOn, windowsMenuCmd, windowsMsgNudge, windowsMinimize, windowsNavStart, windowsNotifyCal, windowsNotifyEmail, windowsNotifyMessaging, windowsNotifySysGeneric, windowsNotifyUnspecified, windowsPopUpBlocked, windowsPrintComplete, windowsProximityConnection, windowsProximityNotification, windowsRecycle, windowsRestore, windowsRingIn, windowsRingOut, windowsShutDown, windowsStartUp, windowsUnlock, windowsUAC

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SoundToPlay
Use to pipe a value from Get-Sound.

```yaml
Type: WillPittenger.Goodies.Sounds.ISound
Parameter Sets: Pipeline
Aliases: InputObject

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Sync
Prevents Start-SoundPlayback from returning until the sound has played if the playback mechanism supports it.  Has no effect if -Loop was specified.

```yaml
Type: System.Management.Automation.SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
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
[Stop-SoundPlayback](Stop-SoundPlayback.md)