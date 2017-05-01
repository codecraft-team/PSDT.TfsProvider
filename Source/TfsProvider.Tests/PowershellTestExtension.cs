using System;
using System.Configuration;
using System.IO;
using System.Management.Automation;
using PSDT.TfsProvider.Base;
using PSDT.TfsProvider.TeamFoundationServer.DataAccess;

namespace PSDT.TfsProvider.Tests {
  public static class PowerShellTestExtensions {
    public static readonly string AbsoluteModulePath;

    static PowerShellTestExtensions () {
      // ReSharper disable once AssignNullToNotNullAttribute
      string relativeModulePath = Path.Combine(Path.GetDirectoryName(typeof(VirtualDriveProvider).Assembly.Location), "..\\");
      AbsoluteModulePath = new DirectoryInfo(relativeModulePath).FullName;
    }

    public static PowerShell NewDrive(this PowerShell powerShell, string driveName, ITfsDataService tfsDataService) {
      powerShell.AddScript($@"
        param (
          [{typeof(ITfsDataService).FullName}]$tfsDataService
        )
        $env:PSModulePath = ""$env:PSModulePath;{AbsoluteModulePath}"";
        Import-Module PSDT.TfsProvider;
        $driveInfo = New-PSDrive -PSProvider TeamFoundationServer -Name {driveName} -Root {driveName}:\ -DataService $tfsDataService;
        Set-Location {driveName}:;");

      powerShell.AddParameter("tfsDataService", new PSObject(tfsDataService));

      powerShell.Streams.Error.ForwardToConsole();

      return powerShell;
    }

    public static PowerShell NewDrive(this PowerShell powerShell) {
      
      powerShell.AddScript($@"
        $env:PSModulePath = ""$env:PSModulePath;{AbsoluteModulePath}"";
        Import-Module PSDT.TfsProvider;
        New-PSDrive -PSProvider VirtualDrive -Name TFS -Root TFS:\ -Scope Global;
        Set-Location TFS:;
      ");

      powerShell.Streams.Error.ForwardToConsole();

      return powerShell;
    }

    public static void ForwardToConsole(this PSDataCollection<ErrorRecord> errors) {
      errors.DataAdded += (sender, args) => {
        ErrorRecord error = errors[args.Index];
        Console.WriteLine($"{error.Exception.Message} {error.ScriptStackTrace}");
      };
    }
  }
}