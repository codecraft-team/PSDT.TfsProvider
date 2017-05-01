using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace PSDT.TfsProvider.Tests.CrossCutting.Behaviors {
  internal class LoggingInterceptionBehavior : IInterceptionBehavior {
    public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext) {
      WriteLog($"Invoking method {input.MethodBase} at {DateTime.Now.ToLongTimeString()}");

      IMethodReturn result = getNext()(input, getNext);

      WriteLog($"Method {input.MethodBase} {(result.Exception != null ? $"threw exception {result.Exception.Message}" : $"returns {result.ReturnValue}")} at {DateTime.Now.ToLongTimeString()}");

      return result;
    }

    public IEnumerable<Type> GetRequiredInterfaces() {
      return Type.EmptyTypes;
    }

    public bool WillExecute { get { return true; } }

    private void WriteLog(string message) {
      Trace.WriteLine(message);
    }
  }
}