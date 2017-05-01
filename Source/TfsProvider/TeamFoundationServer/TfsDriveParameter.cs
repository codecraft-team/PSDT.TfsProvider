using System.Management.Automation;
using PSDT.TfsProvider.TeamFoundationServer.DataAccess;

namespace PSDT.TfsProvider.TeamFoundationServer {
  public class TfsDriveParameter {
    [Parameter(Mandatory = false), AllowEmptyCollection]
    public ITfsDataService DataService { get; set; }

    [Parameter(Mandatory = false)]
    public string Url { get; set; }

    [Parameter(Mandatory = false)]
    public string AccessToken { get; set; }
  }
}