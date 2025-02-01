---
Module Name: WillPittenger.Goodies.PowerShell.JSON
Module Guid: 9a4b7d3e-f9b2-4500-923f-236932b3e86d
Download Help Link: {{ Update Download Link }}
Help Version: {{ Please enter version of help manually (X.X.X.X) format }}
Locale: en-US
---

# WillPittenger.Goodies.PowerShell.JSON Module
## Description
Designed to replace some stock PowerShell JSON CmdLets with better versions.  The main CmdLet replaced for now is ConvertFrom-JSON.

## WillPittenger.Goodies.PowerShell.JSON Cmdlets
### [ConvertFrom-JSON](ConvertFrom-JSON.md)
Like the stock ConvertFrom-JSON, this version converts from JSON into structures PowerShell can work with.  The back end, provided by WillPittenger.Goodies.JSON, keeps the fields sorted.  ConvertFrom-JSON can then either just return that or convert it into PowerShell PSObject instances.  Take your pick.  You can also add fields as needed.  See the documentation for details.