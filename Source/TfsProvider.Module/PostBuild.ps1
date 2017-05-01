﻿param (
  [string]$TargetDir,
  [string]$TargetName,
  [string]$ProjectDir
)

Write-Host "`nRemoving Module binaries..`n";

@("$TargetDir\$targetName.dll", "$TargetDir\$targetName.pdb") | Remove-Item -Force -Verbose;

Write-Host "`nModule binaries removed.`n";

If (Get-Module -ListAvailable -Name PSScriptAnalyzer) {
  $hasError = $false;

  Try {
    $script = "$($ProjectDir)PSDT.TfsProvider.ps1";
    Write-Host "Analyzing script: $($script)";
    $report = Invoke-ScriptAnalyzer -Severity Error -Path $script;
    $report | Format-Table;
    $hasError = $report.Count -gt 0;
  }
  Catch {
    $errorMessage = $_.Exception.Message;
    Write-Host "Failed to analyze scripts. $($errorMessage)";
  }

  If ($hasError) {
    Write-Host "The PSScriptAnalyzer found one or more errors, i.e. quality gate not passed.";
    $Host.SetShouldExit(1);
  }
} 
Else {
  Write-Host "Please install PSSCriptAnalyzer in order to verify script quality.";
  $Host.SetShouldExit(1);
}