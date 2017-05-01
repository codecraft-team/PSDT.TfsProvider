using System;
using System.Diagnostics.Contracts;

namespace PSDT.TfsProvider.TeamFoundationServer.DataAccess {
  public class RetrieveBuildsRequest {
    public TfsProject Project { get; }
    public BuildStatus[] BuildStatuses { get; }

    public RetrieveBuildsRequest(TfsProject project, BuildStatus[] buildStatuses) {
      Contract.Requires<ArgumentNullException>(null != project, nameof(project));

      Project = project;
      BuildStatuses = buildStatuses;
    }
  }
}