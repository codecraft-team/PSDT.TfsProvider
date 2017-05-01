Function Show-TfsContext {
  $host.ui.RawUI.WindowTitle = "PSDT | Team Foundation Server";
  
  function global:prompt {
    Write-Host "[PSDT.Tfs " -NoNewline;
    $drive = Get-Location | % { $_.Drive } | select -First 1;
    $drivePrompt = $drive.Prompt;
    
    If ($drivePrompt -eq $null) {
      Write-Host "Not connected" -NoNewline -ForegroundColor Red;
    }
    Else {
      Write-Host "$drivePrompt" -NoNewline -ForegroundColor Green;
    }

    $host.ui.RawUI.WindowTitle = "PSDT | Team Foundation Server $drivePrompt";

    return "] PS $pwd> ";
  }
}

Function Initialize-TfsPSDrive {
  [CmdletBinding()]
  Param(
    [string]$Url,
    [string]$AccessToken
  )


  #$dataService = New-Object -TypeName "PSDT.TfsProvider.TeamFoundationServer.DataAccess.TfsRestDataService" -ArgumentList @($Url,$AccessToken);
  #New-PSDrive -PSProvider TeamFoundationServer -Name "TFS" -Root "TFS:\" -DataService $dataService -Scope Global;

  New-PSDrive -PSProvider TeamFoundationServer -Name "TFS" -Root "TFS:\" -Url $Url -AccessToken $AccessToken -Scope Global;

  Set-Location TFS:

  Show-TfsContext;
}

Function Clear-TfsPSDriveCache {
  [CmdletBinding()]
  Param (
    [switch]$Builds
  )

  if($Builds) {
    (Get-Location).Drive.ClearBuildsCache();
  }
}