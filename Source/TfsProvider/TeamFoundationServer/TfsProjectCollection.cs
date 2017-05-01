using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PSDT.TfsProvider.Base;

namespace PSDT.TfsProvider.TeamFoundationServer {
  public class TfsProjectCollection : Container {
    public TfsDrive Tfs { get; set; }

    public string Id { get; internal set; }
    public Links Links { get; internal set; }

    public TfsProjectCollection(string name, TfsDrive parent) : base(name, parent) {
      Tfs = parent;
    }

    public override IEnumerable<IDriveItem> GetChildItems(object dynamicParameters) {
      IEnumerable<IDriveItem> projects = Tfs.DataService.RetrieveProjects(this).ToList().Select(token => new TfsProject(token.Value<string>("name"), this) {
        Id = token.Value<string>("id"),
        Links = new Links(token),
        DefaultTeam = new Team(token.SelectToken("defaultTeam"))
      });
      return projects;
    }

    public override void InvokeDefaultAction(object dynamicParameters) {
      Process.Start(Links.Web);
    }
  }
}