using System.Management.Automation;
using PSDT.TfsProvider.TeamFoundationServer.DataAccess;

namespace PSDT.TfsProvider.Base {
  internal sealed class GetChildDynamicParameters {
    [Alias("a")]
    [Parameter]
    public SwitchParameter All { get; set; }

    [Alias("s")]
    [AllowEmptyCollection]
    [Parameter, ValidateSet("inProgress","completed","failed")]
    public BuildStatus[] Status { get; set; }
  }
}