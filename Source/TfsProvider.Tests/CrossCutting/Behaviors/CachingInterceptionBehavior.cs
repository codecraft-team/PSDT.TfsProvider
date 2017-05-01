using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace PSDT.TfsProvider.Tests.CrossCutting.Behaviors {

  internal class CachingInterceptionBehavior : IInterceptionBehavior {
    private readonly Dictionary<string, DateTime> _cache;

    public CachingInterceptionBehavior(Dictionary<string, DateTime> cache) {
      _cache = new Dictionary<string, DateTime>();
    }

    public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext) {
      if (input.MethodBase.Name == "GetTime") {
        string tenantName = input.Arguments["tenant"].ToString();
        if (IsInCache(tenantName)) {
          return input.CreateMethodReturn(FetchFromCache(tenantName));
        }
      }

      IMethodReturn result = getNext()(input, getNext);

      if (input.MethodBase.Name == "GetTime") {
        AddToCache(input.Arguments["tenant"].ToString(), (DateTime) result.ReturnValue);
      }
      return result;
    }

    public IEnumerable<Type> GetRequiredInterfaces() {
      return Type.EmptyTypes;
    }

    public bool WillExecute { get { return true; } }

    private bool IsInCache(string key) {
      return _cache.ContainsKey(key);
    }

    private object FetchFromCache(string key) {
      return _cache[key];
    }

    private void AddToCache(string key, DateTime item) {
      _cache.Add(key, item);
    }
  }
}