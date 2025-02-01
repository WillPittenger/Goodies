---
external help file: WillPittenger.Goodies.PowerShell.JSON.dll-Help.xml
Module Name: WillPittenger.Goodies.PowerShell.JSON
online version:
schema: 2.0.0
---

# ConvertFrom-JSON

## SYNOPSIS
Converts a JSON string into a native .NET object.  Unlike the ConvertFrom-JSON command that comes with PowerShell, this version ensures all fields are sorted.

## SYNTAX

```
ConvertFrom-JSON [-Input] <String> [-MaxDepth <int>] [-AsPowerShellObj] [-AdditionalFieldsMaker <script>] [<CommonParameters>]
```

## DESCRIPTION
By default, this ConvertFrom-JSON implementation outputs as a series of .NET collections.  If you prefer it output PSObjects, specify `-AsPowerShellObj`.  You can use `-AdditionalFieldMaker` to add fields to the structure.  Unlike the standard implementation, pipeline input isn't supported.  The input should be a single string.  The `-MaxDepth` field is passed to the parser.  The parser treats JSON's `undefined` as `$null`.

## EXAMPLES

### Example 1
```powershell
PS C:\> ConvertFrom-JSON (Get-Content ~\MyFile.json)
```

Standard way to convert a JSON file.

### Example 2
```powershell
function _MyAdditionalFieldMaker($_)
{
	if($_ -is [WillPittenger.Goodies.JSON.Obj] -and $_.HasField('SomeDateStr'))
	{
		$_.Add('_SomeDateVal', [DateTime]$_['SomeDateStr'])
	}
}
PS C:\> ConvertFrom-JSON (Get-Content ~\MyFile.json) -MaxDepth 5 -AdditionalFieldMaker _MyAdditonalFieldMaker -AsPowerShellObj
```

Same as above, but now limits the depth to 5, adds a new field, and requests a PowerShell object.  It also checks for a field named "SomeDateStr".  If it finds it, it adds a new one with the same value, but now as a .NET DateTime object.

## PARAMETERS

### -Input
Pass any JSON string.

```yaml
Type: System.String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -MaxDepth
Passed to the parser as a limit to how deep parsing can go.  The default is infinite depth

```yaml
Type: int
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: [int]::MaxValue
Accept pipeline input: False
Accept wildcard characters: False
```

### -AsPowerShellObj
Causes the output to be a `PSObject` instead of a `WillPittenger.Goodies.JSON.ObjBase`.

```yaml
Type: System.Management.Automation.SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: none
Accept pipeline input: False
Accept wildcard characters: False
```

### -AdditionalFieldMaker
Lets you add additional fields.  Use `$_` to access the existing object.  This will be an instance of `WillPittenger.Goodies.JSON.ObjBase`.  Pass a script that adds additional entries to the tables.  You can’t modify the existing entries.  If you also use `-AsPowerShellObj`, the new fields will become PowerShell Note Properties.  The main use for this parameter?  Suppose the JSON has a field that you know contains a date, but it's in a string.  By passing a script block, you can add an entry in a .NET type.  Any .NET type is supported.  Values can be `$null`.  You can add both values to arrays and fields to objects.  See the documentation for `WillPittenger.Goodies.JSON.Array.Add`, `WillPittenger.Goodies.JSON.Array.Insert`, and `WillPittenger.Goodiea.JSON.Obj.Add` for more information.  You have full access to the entire object.

```yaml
Type: System.Management.Automation.SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: $null
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS
A JSON string.

## OUTPUTS

Either a WillPittenger.Goodies.JSON.ObjBase or a PSObject.