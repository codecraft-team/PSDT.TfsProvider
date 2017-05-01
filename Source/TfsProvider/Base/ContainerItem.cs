namespace PSDT.TfsProvider.Base {
  public abstract class ContainerItem : DriveItem {

    protected ContainerItem(string name, IContainer parent) : base(name, parent) {
    }
  }
}