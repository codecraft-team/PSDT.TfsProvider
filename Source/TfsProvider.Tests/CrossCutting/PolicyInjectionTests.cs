using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSDT.TfsProvider.Tests.CrossCutting.ProductionCode;

namespace PSDT.TfsProvider.Tests.CrossCutting {
  [TestClass]
  public class PolicyInjectionTests {

    public IUnityContainer Container { get; set; }

    [TestInitialize]
    public void TestInitialize() {
      Container = new UnityContainer();
      Container.AddNewExtension<Interception>();

      Container.RegisterType<ITenantStore, TenantStore>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
      Container.RegisterType<ISurveyStore, SurveyStore>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

      InjectionProperty first = new InjectionProperty("Order", 1);
      InjectionProperty second = new InjectionProperty("Order", 2);

      PolicyDefinition loggingPolicy = Container.Configure<Interception>().AddPolicy("logging");
      loggingPolicy.AddMatchingRule<AssemblyMatchingRule>(new InjectionConstructor(new InjectionParameter(GetType().Assembly.FullName)));
      loggingPolicy.AddCallHandler<LoggingCallHandler>(new ContainerControlledLifetimeManager(), new InjectionConstructor(), first);

      PolicyDefinition cachingPolicy = Container.Configure<Interception>().AddPolicy("caching");
      cachingPolicy.AddMatchingRule<AssemblyMatchingRule>(new InjectionConstructor(new InjectionParameter(GetType().Assembly.FullName)));
      cachingPolicy.AddCallHandler<CachingCallHandler>(new ContainerControlledLifetimeManager(), new InjectionConstructor(), second);
    }

    [TestMethod]
    public void TestLoggingInterception() {
      ITenantStore tenantStore = Container.Resolve<ITenantStore>();
      DateTime serverTime = tenantStore.GetTime("a");

      Assert.AreNotEqual(DateTime.MinValue, serverTime);
    }

    [TestMethod]
    public void TestCachingInterception() {
      ITenantStore tenantStore = Container.Resolve<ITenantStore>();
      DateTime serverTime1 = tenantStore.GetTime("a");
      DateTime serverTime2 = tenantStore.GetTime("a");
      DateTime serverTime3 = tenantStore.GetTime("b");

      Assert.AreEqual(serverTime1, serverTime2);
      Assert.AreNotEqual(serverTime1, serverTime3);
    }
  }

  class LoggingCallHandler : ICallHandler {
    public int Order { get; set; }

    public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext) {
      IMethodReturn result = getNext()(input, getNext);

      WriteLog($"Method {input.MethodBase} {(result.Exception != null ? $"threw exception {result.Exception.Message}" : $"returns {result.ReturnValue}")} at {DateTime.Now.ToLongTimeString()}");
      WriteLog($"{DateTime.Now:hh:MM:ss}: {input.MethodBase}");

      return result;
    }

    private void WriteLog(string message) {
      Trace.WriteLine(message);
    }
  }

  class CachingCallHandler : ICallHandler {
    private readonly Dictionary<string, DateTime> _cache;

    public CachingCallHandler() {
      _cache = new Dictionary<string, DateTime>();
    }

    public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext) {
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

    public int Order { get; set; }

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