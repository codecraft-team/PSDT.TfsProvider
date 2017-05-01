using System.Management.Automation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using PSDT.TfsProvider.Base;
using PSDT.TfsProvider.TeamFoundationServer.Caching;
using PSDT.TfsProvider.TeamFoundationServer.DataAccess;
using PSDT.TfsProvider.TeamFoundationServer.Diagnostics;

namespace PSDT.TfsProvider.TeamFoundationServer {
  public class TfsDriveInfo : PSDriveInfo {
    public string Prompt { get; private set; }
    public ITfsDataService TfsDataService { get; private set; }
    public UnityContainer Container { get; set; }

    public TfsDriveInfo(PSDriveInfo driveInfo, TfsDriveParameter dynamicParameters) : base(driveInfo) {
      Setup(dynamicParameters);
    }

    private void Setup(TfsDriveParameter dynamicParameters) {
      Container = new UnityContainer();
      Container.AddNewExtension<Interception>();

      //Container.RegisterType<IContainer, TfsDrive>(new TransientLifetimeManager(), new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
      Container.RegisterType<IContainer, TfsDrive>(new TransientLifetimeManager());
      Container.RegisterType<CachingLifetimeManager>(new ContainerControlledLifetimeManager());

      if (null != dynamicParameters.DataService) {
        Container.RegisterInstance(dynamicParameters.DataService);
      }
      else {
        Container.RegisterType<ITfsDataService, TfsRestDataService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(
          new InjectionParameter<string>(dynamicParameters.Url),
          new InjectionParameter<string>(dynamicParameters.AccessToken)
        ), new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
      }

      PolicyDefinition loggingPolicy = Container.Configure<Interception>().AddPolicy("logging");
      loggingPolicy.AddMatchingRule<AssemblyMatchingRule>(new InjectionConstructor(new InjectionParameter(GetType().Assembly.FullName)));
      loggingPolicy.AddCallHandler<LoggingCallHandler>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
      loggingPolicy.AddCallHandler<PerformanceMeasurementHandler>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
      loggingPolicy.AddCallHandler<CachingCallHandler>(Container.Resolve<CachingLifetimeManager>(), new InjectionConstructor());

      TfsDataService = Container.Resolve<ITfsDataService>();
    }

    public void ClearBuildsCache() {
      CachingLifetimeManager cachingLifetimeManager = Container.Resolve<CachingLifetimeManager>();
      CachingCallHandler cache = cachingLifetimeManager.GetCachingCallHandler();
      cache.ClearBuildsCache();
    }

    public TfsDriveInfo(string name, ProviderInfo provider, string root, string description, PSCredential credential) : base(name, provider, root, description, credential) {
    }

    public TfsDriveInfo(string name, ProviderInfo provider, string root, string description, PSCredential credential, string displayRoot) : base(name, provider, root, description, credential, displayRoot) {
    }

    public TfsDriveInfo(string name, ProviderInfo provider, string root, string description, PSCredential credential, bool persist) : base(name, provider, root, description, credential, persist) {
    }

    public void SetPrompt(string prompt) {
      Prompt = prompt;
    }
  }

}