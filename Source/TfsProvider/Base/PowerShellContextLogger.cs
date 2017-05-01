using System;
using System.Management.Automation;
using System.Management.Automation.Provider;

namespace PSDT.TfsProvider.Base { 
  public class PowerShellContextLogger {
    private NavigationCmdletProvider _provider;
    private DateTime? _previousDebugMessageTime;

    public void RegisterProvider(NavigationCmdletProvider provider) {
      _provider = provider;
    }

    public void WriteDebug(string message) {
      message = AddTimestamp(message);
      Write(provider => provider?.WriteDebug(message));
    }

    private string AddTimestamp(string message) {
      DateTime currentMessageTime = DateTime.Now;
      TimeSpan elapsed = _previousDebugMessageTime.HasValue ? currentMessageTime.Subtract(_previousDebugMessageTime.Value) : TimeSpan.Zero;
      _previousDebugMessageTime = currentMessageTime;

      return $"{DateTime.Now:HH:mm:ss.fff} +{elapsed.TotalSeconds:0.000}: {message}";
    }

    public void WriteWarning(string message) {
      Write(provider => provider?.WriteWarning(message));
    }

    private void Write(Action<NavigationCmdletProvider> writeAction) {
      writeAction(_provider);
    }

    public void WriteError(Exception exception, ErrorCategory errorCategory, object targetObject) {
      Write(provider => provider?.WriteError(new ErrorRecord(exception, exception.HResult.ToString(), errorCategory, targetObject)));
    }

    public void WriteVerbose(string message) {
      Write(provider => provider?.WriteVerbose(message));
    }
  }
}