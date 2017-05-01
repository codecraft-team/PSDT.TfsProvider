using System.Collections.Generic;
using System.Diagnostics;
using PSDT.TfsProvider.Base;

namespace PSDT.TfsProvider.TeamFoundationServer {
  public class TfsProject : Container {
    public TfsProjectCollection ProjectCollection { get; }
    public string Id { get; internal set; }
    public Links Links { get; internal set; }
    public Team DefaultTeam { get; internal set; }

    public TfsProject(string name, TfsProjectCollection parent) : base(name, parent) {
      ProjectCollection = parent;
    }

    public override IEnumerable<IDriveItem> GetChildItems(object dynamicParameters) {
      return new IDriveItem[] {
        new BuildsContainer(this), 
        new BuildDefinitionsContainer(this),  
      };
    }

    public override void InvokeDefaultAction(object dynamicParameters) {
      Process.Start(Links.Web);
    }
  }
}