using System.Management.Automation;
using System.Management.Automation.Provider;
using Microsoft.Practices.Unity;
using PSDT.TfsProvider.Base;

namespace PSDT.TfsProvider.TeamFoundationServer {
  [CmdletProvider("TeamFoundationServer", ProviderCapabilities.Include | ProviderCapabilities.Exclude | ProviderCapabilities.Filter)]
  public class TfsProvider : VirtualDriveProvider {

    protected override PSDriveInfo NewDrive(PSDriveInfo drive) {
      return new TfsDriveInfo(drive, (TfsDriveParameter) DynamicParameters);
    }

    protected override object NewDriveDynamicParameters() {
      return new TfsDriveParameter();
    }

    protected override IContainer GetDriveContent() {
      IContainer container = ((TfsDriveInfo) PSDriveInfo).Container.Resolve<IContainer>(new ParameterOverrides {
        {"driveInfo", (TfsDriveInfo) PSDriveInfo}
      });

      return container;
    }
  }
}