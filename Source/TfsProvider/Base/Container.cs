using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace PSDT.TfsProvider.Base {
  public abstract class Container : DriveItem, IContainer {
    protected Container(string name, Container parent) : base(name, parent) {
    }

    protected Container(string name) : base(name, null) {
    }

    public IDriveItem GetItemValue(string path) {
      List<IDriveItem> childItems = GetChildItems(null).ToList();

      foreach (IDriveItem childItem in childItems) {
        if (IsMatch(path, childItem)) {
          return childItem;
        }

        bool pathIsInCurrentBranch = path.StartsWith(childItem.FullName, StringComparison.OrdinalIgnoreCase);
        if (pathIsInCurrentBranch) {
          IDriveItem item = (childItem as IContainer)?.GetItemValue(path);
          if (item != null) {
            return item;
          }
        }
      }

      return null;
    }

    public virtual IEnumerable<IDriveItem> GetChildItems(object dynamicParameters) {
      //If ReturnAllContainers then do not consider -Filter, otherwise only return matching containers.
      //Think about it as you would use -File and -Directory parameter on Get-ChildItem.

      //Filter is applied on the result of RetrieveChildItems by the provider.
      //Include and Exclude is appled on the query, that is only included data is retrieved, and/or excluded data is not retrieved.
      throw new NotSupportedException($"The container {GetType().FullName} under the path \"{FullName}\" does not support child items.");
    }

    public virtual object GetChildItemsDynamicParameters() {
      return null;
    }
    
    private bool IsMatch(string path, IDriveItem driveItem) {
      string fullName = driveItem.FullName;

      if (path.Contains("*")) {
        WildcardPattern pattern = new WildcardPattern(path, WildcardOptions.IgnoreCase);
        return pattern.IsMatch(fullName);
      }

      return string.Equals(fullName, path, StringComparison.OrdinalIgnoreCase);
    }
  }
}