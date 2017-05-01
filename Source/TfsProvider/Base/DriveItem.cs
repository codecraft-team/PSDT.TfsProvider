using System;

namespace PSDT.TfsProvider.Base {
  public abstract class DriveItem : IDriveItem {
    public const string PathSerparator = "\\";
    public IContainer Parent { get; }
    public string Name { get; protected set; }

    public string FullName => GetFullName();

    protected DriveItem(string name, IContainer parent) {
      Name = name ?? string.Empty;
      Parent = parent;
    }

    public virtual void RemoveItem(bool recurse) {
      throw new NotSupportedException($"The item {GetType().FullName} under the path \"{FullName}\" does not support Remove-Item.");
    }

    public virtual void InvokeDefaultAction(object dynamicParameters) {
      throw new NotSupportedException($"The item {GetType().FullName} under the path \"{FullName}\" does not support Invoke-Item.");
    }

    public virtual object InvokeDefaultActionDynamicParameters() {
      return null;
    }

    private string GetFullName() {
      char[] pathSeparator = PathSerparator.ToCharArray();
      return $"{Parent?.FullName.TrimEnd(pathSeparator)}{(Parent == null ? "" : PathSerparator)}{Name.TrimEnd(pathSeparator)}";
    }
  }
}