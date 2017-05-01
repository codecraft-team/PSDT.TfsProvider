using System.Diagnostics;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace PSDT.TfsProvider.TeamFoundationServer.Diagnostics {
  class PerformanceMeasurementHandler : ICallHandler {
    public const string PerformanceKey = "Stopwatch";
    public int Order { get; set; } = 1;

    public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext) {
      Stopwatch sw = Stopwatch.StartNew();
      input.InvocationContext.Add(PerformanceKey, sw);

      IMethodReturn methodReturn = getNext()(input, getNext);

      sw.Stop();
      return methodReturn;
    }
  }
}