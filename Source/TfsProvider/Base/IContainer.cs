using System.Collections.Generic;

namespace PSDT.TfsProvider.Base {
  public interface IContainer : IDriveItem {
    IEnumerable<IDriveItem> GetChildItems(object dynamicParameters);
    IDriveItem GetItemValue(string path);
    object GetChildItemsDynamicParameters();
  }
}