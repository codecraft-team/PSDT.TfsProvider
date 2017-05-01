[CmdletBinding()]
Param (
  [Parameter(Mandatory=$True)]
  [string]$Url,
  [Parameter(Mandatory=$True)]
  [string]$AccessToken
)

$env:PSModulePath = "$env:PSModulePath;{0}" -f [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "\..\..\Binaries"));

Import-Module PSDT.TfsProvider;

Initialize-TfsPSDrive -Url $Url -AccessToken $AccessToken;