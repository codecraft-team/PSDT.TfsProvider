using PSDT.TfsProvider.TeamFoundationServer.DataAccess;

namespace PSDT.TfsProvider.TeamFoundationServer.Caching {
  internal class RetrieveBuildsCacheKey {
    private readonly RetrieveBuildsRequest _request;

    public RetrieveBuildsCacheKey(RetrieveBuildsRequest request) {
      _request = request;
    }

    public override string ToString() {
      return $"{_request.Project.Name}-{string.Join(",", _request.BuildStatuses)}";
    }
  }
}