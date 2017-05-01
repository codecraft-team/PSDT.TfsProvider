using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PSDT.TfsProvider.Base;
using PSDT.TfsProvider.TeamFoundationServer.DataAccess;

namespace PSDT.TfsProvider.TeamFoundationServer {
  public class TfsDrive : Container {
    public ITfsDataService DataService { get; }
    
    public TfsDrive(TfsDriveInfo driveInfo) : base(driveInfo.Root) {
      DataService = driveInfo.TfsDataService;
    }

    public override IEnumerable<IDriveItem> GetChildItems(object dynamicParameters) {
      IEnumerable<IDriveItem> projectCollections = DataService.RetrieveProjectCollections().ToList().Select(token => new TfsProjectCollection(token.Value<string>("name"), this) {
        Id = token.Value<string>("id"),
        Links = new Links(token)
      });
      return projectCollections;
    }

    public override void InvokeDefaultAction(object dynamicParameters) {
      Process.Start(DataService.Url);
    }
  }
}