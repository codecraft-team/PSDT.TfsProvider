using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Practices.Unity.InterceptionExtension;
using Newtonsoft.Json.Linq;
using PSDT.TfsProvider.TeamFoundationServer.DataAccess;

namespace PSDT.TfsProvider.TeamFoundationServer.Caching {
  public class CachingCallHandler : ICallHandler {
    private JArray _projectCollectionsCache;
    private readonly ConcurrentDictionary<string, JArray> _projectsCache = new ConcurrentDictionary<string, JArray>();
    private readonly ConcurrentDictionary<string, RetrieveBuildsResponse> _buildsCache = new ConcurrentDictionary<string, RetrieveBuildsResponse>();
    private readonly ConcurrentDictionary<string, JArray> _buildDefinitionssCache = new ConcurrentDictionary<string, JArray>();

    public int Order { get; set; } = 2;

    public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext) {
      ITfsDataService tfsDataService = input.Target as ITfsDataService;
      if (null == tfsDataService) {
        return getNext()(input, getNext);
      }

      switch (input.MethodBase.Name) {
        case nameof(ITfsDataService.RetrieveProjectCollections):
          return RetrieveProjectCollections(input, getNext);
        case nameof(ITfsDataService.RetrieveProjects):
          return RetrieveProjects(input, getNext);
        case nameof(ITfsDataService.RetrieveBuilds):
          return RetrieveBuilds(input, getNext);
        case nameof(ITfsDataService.RetrieveBuildDefinitions):
          return RetrieveBuildDefinitions(input, getNext);
        default:
          return getNext()(input, getNext);
      }
    }

    public void ClearBuildsCache() {
      _buildsCache.Clear();
    }

    private IMethodReturn RetrieveProjectCollections(IMethodInvocation input, GetNextHandlerDelegate getNext) {
      if (null == _projectCollectionsCache) {
        _projectCollectionsCache = (JArray) getNext()(input, getNext).ReturnValue;
      }

      return input.CreateMethodReturn(_projectCollectionsCache);
    }

    private IMethodReturn RetrieveProjects(IMethodInvocation input, GetNextHandlerDelegate getNext) {
      string key = ((TfsProjectCollection) input.Arguments[0]).Name;

      if (!_projectsCache.ContainsKey(key)) {
        JArray projects = (JArray) getNext()(input, getNext).ReturnValue;
        _projectsCache.TryAdd(key, projects);
      }

      JArray result;
      _projectsCache.TryGetValue(key, out result);

      return input.CreateMethodReturn(result);
    }

    private IMethodReturn RetrieveBuilds(IMethodInvocation input, GetNextHandlerDelegate getNext) {
      RetrieveBuildsCacheKey cacheKey = new RetrieveBuildsCacheKey((RetrieveBuildsRequest)input.Arguments[0]);
      string key = cacheKey.ToString();

      if (!_buildsCache.ContainsKey(key)) {
        RetrieveBuildsResponse retrieveBuildsResponse = (RetrieveBuildsResponse) getNext()(input, getNext).ReturnValue;
        _buildsCache.TryAdd(key, retrieveBuildsResponse);
      }

      RetrieveBuildsResponse cachedResponse;
      _buildsCache.TryGetValue(key, out cachedResponse);

      return input.CreateMethodReturn(cachedResponse);
    }

    private IMethodReturn RetrieveBuildDefinitions(IMethodInvocation input, GetNextHandlerDelegate getNext) {
      string key = ((TfsProject) input.Arguments[0]).Name;

      if (!_buildDefinitionssCache.ContainsKey(key)) {
        JArray buildDefinitions = (JArray) getNext()(input, getNext).ReturnValue;
        _buildDefinitionssCache.TryAdd(key, buildDefinitions);
      }

      JArray result;
      _buildDefinitionssCache.TryGetValue(key, out result);

      return input.CreateMethodReturn(result);
    }
  }
}