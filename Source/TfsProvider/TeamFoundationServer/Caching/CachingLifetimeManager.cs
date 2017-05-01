using Microsoft.Practices.Unity;

namespace PSDT.TfsProvider.TeamFoundationServer.Caching {
  public class CachingLifetimeManager : ContainerControlledLifetimeManager {

    public CachingCallHandler GetCachingCallHandler() {
      return (CachingCallHandler) GetValue();
    }
  }
}