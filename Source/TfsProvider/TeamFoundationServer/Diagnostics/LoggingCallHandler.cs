using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Practices.Unity.InterceptionExtension;
using PSDT.TfsProvider.Base;

namespace PSDT.TfsProvider.TeamFoundationServer.Diagnostics {
  class LoggingCallHandler : ICallHandler {
    public int Order { get; set; } = 0;

    public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext) {
      IMethodReturn result = getNext()(input, getNext);

      try {
        if (typeof(IContainer).FullName == input.MethodBase.DeclaringType?.FullName) {
          LogCallToContainer(input, result);
        }
      }
      catch (Exception e) {
        WriteLog($"{DateTime.Now:hh:MM:ss} Error occured in logging call handler. {e.Message}.");
      }

      return result;
    }

    private void LogCallToContainer(IMethodInvocation input, IMethodReturn result) {
      string message = string.Empty;

      if (result.Exception != null) {
        message = $"threw exception {result.Exception.Message}";
      }
      else {
        switch (input.MethodBase.Name) {
          case nameof(IContainer.GetItemValue):
            message += $"the value at \"{input.Arguments[0]}\" is \"{((IDriveItem)result.ReturnValue).FullName}\"";
            break;
          case nameof(IContainer.GetChildItems):
            message += $"child items are {string.Join(",", ((IEnumerable<IDriveItem>)result.ReturnValue).Select(p => $"\"{p.FullName}\""))}.";
            break;
        }
      }

      Stopwatch sw = (Stopwatch) result.InvocationContext[PerformanceMeasurementHandler.PerformanceKey];
      
      WriteLog($"{DateTime.Now:hh:MM:ss} \"{((IContainer)input.Target).FullName}\" ({sw.Elapsed.TotalSeconds:0.00000}): {message}");
    }

    private void WriteLog(string message) {
      Trace.WriteLine(message);
    }
  }
}