using System;

namespace PSDT.TfsProvider.Tests.CrossCutting.ProductionCode {
  public interface ITenantStore {
    DateTime GetTime(string tenant);
  }
}