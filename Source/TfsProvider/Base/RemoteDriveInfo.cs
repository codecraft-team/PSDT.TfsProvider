using System.Management.Automation;

namespace PSDT.TfsProvider.Base {
  public class RemoteDriveInfo : PSDriveInfo {
    public string Prompt { get; private set; }

    public RemoteDriveInfo(PSDriveInfo driveInfo) : base(driveInfo) {
    }

    public RemoteDriveInfo(string name, ProviderInfo provider, string root, string description, PSCredential credential) : base(name, provider, root, description, credential) {
    }

    public RemoteDriveInfo(string name, ProviderInfo provider, string root, string description, PSCredential credential, string displayRoot) : base(name, provider, root, description, credential, displayRoot) {
    }

    public RemoteDriveInfo(string name, ProviderInfo provider, string root, string description, PSCredential credential, bool persist) : base(name, provider, root, description, credential, persist) {
    }

    public void SetPrompt(string prompt) {
      Prompt = prompt;
    }
  }
}