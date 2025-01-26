param(
	[Parameter()] $ProjectName,
	[Parameter()] $ConfigurationName,
	[Parameter()] $TargetDir
)

trap
{
	Write-Error $_ -ErrorAction SilentlyContinue;

	exit 1;
}

$ErrorActionPreference = "Stop";

Copy "$TargetDir/*.dll" "$TargetDir/Org.WillPittenger.PowerShell.Tools.Sounds" -Force -Verbose;

exit 0;