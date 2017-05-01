using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PSDT.TfsProvider.TeamFoundationServer.DataAccess {
  public class TfsRestDataService : ITfsDataService {
    private static readonly HttpClient HttpClient = new HttpClient();
    public bool IsOnline { get; }
    public string Url { get; }

    public TfsRestDataService(string url, string accessToken) {
      Contract.Requires<ArgumentNullException>(null != url, nameof(url));
      Contract.Requires<ArgumentNullException>(null != accessToken, nameof(accessToken));

      Url = url;
      IsOnline = url.EndsWith(".visualstudio.com");

      string authorizationHeaderValue = $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($":{accessToken}"))}";
      HttpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeaderValue);
    }

    public string GetProjectUrl(TfsProject tfsProject) {
      return IsOnline ? $"{Url}/{tfsProject.Name}" : $"{Url}/{tfsProject.ProjectCollection.Name}/{tfsProject.Name}";
    }

    public virtual JArray RetrieveProjectCollections() {
      string url = $"{Url}/_apis/projectcollections";
      JArray collectionHeaders = ParseCollectionResponse(url);
      return RetrieveDetails(collectionHeaders);
    }

    public virtual JArray RetrieveProjects(TfsProjectCollection projectCollection) {
      string url = IsOnline ? $"{Url}/_apis/projects" : $"{Url}/{projectCollection.Name}/_apis/projects";
      JArray collectionHeaders = ParseCollectionResponse(url);
      return RetrieveDetails(collectionHeaders);
    }

    public virtual RetrieveBuildsResponse RetrieveBuilds(RetrieveBuildsRequest request) {
      string url = $"{GetProjectUrl(request.Project)}/_apis/build/builds?maxBuildsPerDefinition=1";
      if (request.BuildStatuses.Any()) {
        url += $"{url}&statusFilter={string.Join(",", request.BuildStatuses)}";
      }

      return new RetrieveBuildsResponse {
        Builds = ParseCollectionResponse(url)
      };
    }

    public virtual JArray RetrieveBuildDefinitions(TfsProject tfsProject) {
      string url = $"{GetProjectUrl(tfsProject)}/_apis/build/definitions";
      return ParseCollectionResponse(url);
    }

    public void QueueBuild(TfsBuildDefinition tfsBuildDefinition) {
      TfsProject project = tfsBuildDefinition.BuildDefinitionsContainer.Project;
      string url = $"{GetProjectUrl(project)}/_apis/build/builds?api-version=2.0";

      //REMARK: invalidate cache, show result?
      HttpResponseMessage response = HttpClient.PostAsync(url, new StringContent($"{{\"definition\":{{\"id\": {tfsBuildDefinition.Id}}}}}", Encoding.UTF8, "application/json")).Result;

      QueueBuildResponse queueBuildResponse = new QueueBuildResponse(tfsBuildDefinition.Queue.Name, response);
      queueBuildResponse.Validate();
    }

    private JArray ParseCollectionResponse(string url) {
      string responseContent = GetResponseContent(url);
      JObject content = JObject.Parse(responseContent);
      return (JArray)content["value"];
    }

    private string GetResponseContent(string url) {
      HttpResponseMessage response = HttpClient.GetAsync(url).Result;
      return response.Content.ReadAsStringAsync().Result;
    }

    private JArray RetrieveDetails(JArray collectionHeaders) {
      JArray projectCollections = new JArray();
      foreach (JToken collectionHeader in collectionHeaders) {
        string collectionDetail = GetResponseContent(collectionHeader.Value<string>("url"));
        JToken collectionDetailToken = JToken.Parse(collectionDetail);
        projectCollections.Add(collectionDetailToken);
      }
      return projectCollections;
    }
  }
}