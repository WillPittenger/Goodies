param
(
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

$dest = [IO.Path]::Combine($TargetDir, $ProjectName);

Write-host "Copying DLLs from $TargetDir to $dest.";
Copy "$TargetDir\*.dll" $dest -Force -Verbose;

exit 0;