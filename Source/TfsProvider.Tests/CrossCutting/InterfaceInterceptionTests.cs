using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSDT.TfsProvider.Tests.CrossCutting.Behaviors;
using PSDT.TfsProvider.Tests.CrossCutting.ProductionCode;

namespace PSDT.TfsProvider.Tests.CrossCutting {
  [TestClass]
  public class InterfaceInterceptionTests {
    public IUnityContainer Container { get; set; }

    [TestInitialize]
    public void TestInitialize() {
      Container = new UnityContainer();
      Container.AddNewExtension<Interception>();

      Dictionary<string, DateTime> dateTimeCache = new Dictionary<string, DateTime> {
        {"c", DateTime.Now}
      };
      Container.RegisterInstance(dateTimeCache);

      Container.RegisterType<ITenantStore, TenantStore>(
        new Interceptor<InterfaceInterceptor>(), 
        new InterceptionBehavior<LoggingInterceptionBehavior>(), 
        new InterceptionBehavior<CachingInterceptionBehavior>());
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
}