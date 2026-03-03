param
(
	[Parameter(Mandatory)] $ProjectName,
    [Parameter(Mandatory)] $SourceDir,
	[Parameter(Mandatory)] $TargetDir
)

trap
{
	Write-Error $_ -ErrorAction SilentlyContinue;

    Write-Host "Error trapped: $_";

	exit 1;
}

$ErrorActionPreference = "Stop";

$dest = [IO.Path]::Combine($TargetDir, $ProjectName);

write-host "$ProjectName, $TargetDir, $dest";

del $dest -Recurse -Verbose -ErrorAction SilentlyContinue;

mkdir $dest -Force -Verbose;

Write-host "Copying DLLs from $TargetDir to $dest.";
Copy "$TargetDir\*.dll", "$SourceDir\$ProjectName\*.psd1", "$SourceDir\$ProjectName\*.psm" $dest -Force -Verbose;

exit 0;