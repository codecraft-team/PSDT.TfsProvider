using System;

namespace PSDT.TfsProvider.Tests.CrossCutting.ProductionCode {
  public class TenantStore : ITenantStore {

    public DateTime GetTime(string tenant) {
      return DateTime.UtcNow;
    }
  }
}