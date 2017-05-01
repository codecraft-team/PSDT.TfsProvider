@{
  RootModule = 'PSDT.TfsProvider.dll'
  ModuleVersion = '1.0.0.1'
  GUID = '0466ACEE-A6F1-4421-9425-B7296DB4E0B9'
  Author = 'Tauri-Code'
  CompanyName = 'Tauri-Code'
  Copyright = '(c) 2017 Tauri-Code. All rights reserved.'
  Description = 'Powershell tools for Team Foundation Builds.'
  ScriptsToProcess = @("PSDT.TfsProvider.ps1")
  FormatsToProcess = @("PSDT.TfsProvider.ps1xml")
  FunctionsToExport = '*'
  CmdletsToExport = '*'
  VariablesToExport = '*'
  AliasesToExport = '*'
}