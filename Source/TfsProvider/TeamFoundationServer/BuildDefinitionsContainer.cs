using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PSDT.TfsProvider.Base;

namespace PSDT.TfsProvider.TeamFoundationServer {
  public class BuildDefinitionsContainer : Container {
    private const string ContainerName = "BuildDefinitions";
    public TfsProject Project { get; set; }

    public BuildDefinitionsContainer(TfsProject parent) : base(ContainerName, parent) {
      Project = parent;
    }

    public override IEnumerable<IDriveItem> GetChildItems(object dynamicParameters) {
      IEnumerable<IDriveItem> buildDefintiions = Project.ProjectCollection.Tfs.DataService.RetrieveBuildDefinitions(Project).ToList().Select(token => new TfsBuildDefinition(token.Value<string>("name"), this) {
        Id = token.Value<string>("id"),
        Links = new Links(token),
        AuthoredBy = new TfsUser(token.SelectToken("authoredBy")),
        CreatedDate = token.Value<DateTime>("createdDate"),
        Queue = new TfsQueue(token.SelectToken("queue")),
        Revision = token.Value<string>("revision")
      });

      return buildDefintiions;
    }

    public override void InvokeDefaultAction(object dynamicParameters) {
      Process.Start($"{Project.Links.Web}/_build");
    }
  }
}