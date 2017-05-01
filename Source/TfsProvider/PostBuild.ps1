param (
  [string]$Config,
  [string]$TargetDir
)

Function Remove-Artifact {
  param (
    [string]$Name
  )

  Remove-Item -Force -Path ("{0}{1}" -f $TargetDir,$Name) -Recurse;
}

If($Config -eq "Release") {
  Remove-Artifact "PSDT.TfsProvider.pdb";
  Remove-Artifact "CodeContracts";
  Remove-Artifact "PSDT.TfsProvider.dll.config";
}