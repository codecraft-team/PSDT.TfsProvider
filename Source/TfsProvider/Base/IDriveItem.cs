namespace PSDT.TfsProvider.Base {
  public interface IDriveItem {
    string FullName { get; }
    string Name { get; }
    IContainer Parent { get; }

    void RemoveItem(bool recurse);
    void InvokeDefaultAction(object dynamicParameters);
    object InvokeDefaultActionDynamicParameters();
  }
}