using Newtonsoft.Json.Linq;

namespace PSDT.TfsProvider.TeamFoundationServer.DataAccess {
  public interface ITfsDataService {
    string Url { get; }
    string GetProjectUrl(TfsProject tfsProject);
    JArray RetrieveProjectCollections();
    JArray RetrieveProjects(TfsProjectCollection projectCollection);
    RetrieveBuildsResponse RetrieveBuilds(RetrieveBuildsRequest request);
    JArray RetrieveBuildDefinitions(TfsProject tfsProject);
    void QueueBuild(TfsBuildDefinition tfsBuildDefinition);
  }
}