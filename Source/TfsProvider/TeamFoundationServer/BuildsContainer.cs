using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PSDT.TfsProvider.Base;
using PSDT.TfsProvider.TeamFoundationServer.DataAccess;

namespace PSDT.TfsProvider.TeamFoundationServer {
  public class BuildsContainer : Container {
    private const string ContainerName = "Builds";
    public TfsProject Project { get; set; }

    public BuildsContainer(TfsProject parent) : base(ContainerName, parent) {
      Project = parent;
    }

    public override IEnumerable<IDriveItem> GetChildItems(object dynamicParameters) {
      GetChildDynamicParameters parameter = dynamicParameters as GetChildDynamicParameters;
      RetrieveBuildsResponse retrieveBuildsResponse = Project.ProjectCollection.Tfs.DataService.RetrieveBuilds(new RetrieveBuildsRequest(Project, parameter.Status));

      IEnumerable<IDriveItem> builds = retrieveBuildsResponse.Builds.Select(token => new TfsBuild(token.Value<string>("buildNumber"), this) {
        Id = token.Value<string>("id"),
        Result = token.Value<string>("result"),
        CompletedOn = token.Value<DateTime>("completedOn"),
        RequestedFor = token.Value<string>("requestedOn"),
        Status = token.Value<string>("status"),
        Links = new Links(token)
      });

      return builds;
    }

    public override object GetChildItemsDynamicParameters() {
      return new GetChildDynamicParameters();
    }

    public override void InvokeDefaultAction(object dynamicParameters) {
      Process.Start($"{Project.Links.Web}/_build");
    }
  }
}