using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Reflection;

namespace PSDT.TfsProvider.Base {
  [CmdletProvider("VirtualDrive", ProviderCapabilities.Filter)]
  public abstract class VirtualDriveProvider : NavigationCmdletProvider {
    private readonly string pathSeparator = "\\";
    private IContainer _driveContent;

    protected IContainer DriveContent {
      get {
        if (null == _driveContent) {
          InitializeDriveContent();
        }
        return _driveContent;
      }
    }
    
    protected virtual IContainer GetDriveContent() {
      return new VirtualDriveSample();
    }

    protected void InitializeDriveContent() {
      _driveContent = GetDriveContent();
    }

    protected override PSDriveInfo NewDrive(PSDriveInfo drive) {
      return new RemoteDriveInfo(drive);
    }

    #region Item Methods

    protected override void GetItem(string path) {
      if (PathIsDrive(path)) {
        TraceDebug($"GetItem.path: \"{path}\" WriteItemObject(PSDriveInfo, path, true)");
        WriteItemObject(PSDriveInfo, path, true);
        return;
      }

      IDriveItem item = GetItemValue(path);
      if (item == null) {
        WriteError(new ErrorRecord(new ArgumentException($"The item at the path \"{path}\" does not exist. Get-Item failed."), "GetItem", ErrorCategory.InvalidArgument, path));
        return;
      }

      bool isContainer = item is IContainer;
      TraceDebug($"GetItem.path: \"{path}\" WriteItemObject(item, path, true) Item.path: {item.FullName}");
      WriteItemObject(item, item.FullName, isContainer);
    }

    protected IDriveItem GetItemValue(string path) {
      if (string.IsNullOrEmpty(path) || (null != PSDriveInfo && string.Equals(PSDriveInfo.Root, path, StringComparison.OrdinalIgnoreCase))) {
        return DriveContent;
      }

      path = EnsureDriveIsRooted(path);
      path = path.TrimEnd('\\');

      IDriveItem item = DriveContent.GetItemValue(path);
      if (null == item) {
        TraceDebug($"GetItemValue: path \"{path}\" not found.");
      }

      return item;
    }

    protected override void InvokeDefaultAction(string path) {
      IDriveItem item = GetItemValue(path);
      item.InvokeDefaultAction(DynamicParameters);
    }

    protected override object InvokeDefaultActionDynamicParameters(string path) {
      IDriveItem item = GetItemValue(path);
      return item.InvokeDefaultActionDynamicParameters();
    }

    protected override bool ItemExists(string path) {
      if (PathIsDrive(path)) {
        TraceDebug($"ItemExists.path: \"{path}\" Path is drive. return true;");
        return true;
      }

      IDriveItem item = GetItemValue(path);
      bool itemExists = item != null;
      TraceDebug($"ItemExists.path: \"{path}\" return {itemExists};");
      return itemExists;
    }

    protected override bool IsValidPath(string path) {
      if (string.IsNullOrEmpty(path)) {
        WriteDebug($"IsValidPath.path: {path} is empty returning false;");
        return false;
      }

      string modifiedPath = path;
      modifiedPath = EnsureDriveIsRooted(modifiedPath);
      int num = modifiedPath.IndexOf(':');
      int length = modifiedPath.IndexOf(':', num + 1);
      if (length > 0) modifiedPath = modifiedPath.Substring(0, length);

      bool isAbsolutePath = modifiedPath.Contains(":");
      if (!isAbsolutePath) {
        WriteDebug($"IsValidPath.path: {path}, modified path: {modifiedPath} is not an absolute path, returning false;");
        return false;
      }

      IDriveItem info = GetItemValue(modifiedPath);
      if (info == null) {
        WriteDebug($"IsValidPath.path: {path} not found IDriveItem.");
      }

      return true;
    }

    #endregion Item Overloads

    #region Container Overloads

    protected override void GetChildItems(string path, bool recurse) {
      IDriveItem item = GetItemValue(path);
      if (!(item is IContainer)) {
        TraceDebug($"GetChildItems.path: \"{path}\" GetChildItems.recurse: \"{recurse}\" return without doing anything, item is not a container.");
        return;
      }

      foreach (IDriveItem child in GetChildItems(item)) {
        bool isContainer = child is IContainer;

        TraceDebug($"GetChildItems.path: \"{path}\" GetChildItems.recurse: \"{recurse}\" WriteItemObject(child.Name, child.FullName, isContainer); child.Name: {child.Name} child.FullName: {child.FullName} isContainer: {isContainer}");
        WriteItemObject(child, child.FullName, isContainer);

        if (isContainer && recurse) {
          GetChildItems(child.FullName, recurse);
        }
      }
    }

    protected override object GetChildItemsDynamicParameters(string path, bool recurse) {
      IContainer item = GetItemValue(path) as IContainer;
      return item?.GetChildItemsDynamicParameters();
    }

    private IEnumerable<IDriveItem> GetChildItems(IDriveItem item, ReturnContainers returnContainers = ReturnContainers.ReturnMatchingContainers) {
      IEnumerable<IDriveItem> childItems = ((IContainer) item).GetChildItems(DynamicParameters);
      bool useFilter = !string.IsNullOrEmpty(Filter);
      if (useFilter) {
        WildcardPattern pattern = new WildcardPattern(Filter, WildcardOptions.IgnoreCase);
        bool returnAllContainers = returnContainers == ReturnContainers.ReturnAllContainers;
        return childItems.Where(i => (returnAllContainers && i is Container) || pattern.IsMatch(i.Name));
      }
      return childItems;
    }

    protected override void GetChildNames(string path, ReturnContainers returnContainers) {
      IDriveItem item = GetItemValue(path);
      if (item is IContainerItem) {
        TraceDebug($"GetChildNames.path: \"{path}\" GetChildItems.returnContainers: \"{returnContainers}\" return without doing anything, item is file info;");
        return;
      }

      foreach (IDriveItem child in GetChildItems(item, returnContainers)) {
        bool isContainer = child is IContainer;

        TraceDebug($"GetChildNames.path: \"{path}\" GetChildItems.returnContainers: \"{returnContainers}\"  WriteItemObject(child.Name, child.FullName, isContainer) child.Name: {child.Name} child.FullName: {child.FullName} isContainer: {isContainer};");
        WriteItemObject(child.Name, child.FullName, isContainer);
      }
    }

    protected override bool HasChildItems(string path) {
      if (PathIsDrive(path)) {
        TraceDebug($"HasChildItems.path: \"{path}\" Path is drive. return true;");
        return true;
      }

      IDriveItem item = GetItemValue(path);
      bool hasChildren = item is IContainer;
      TraceDebug($"HasChildItems.path: \"{path}\" return {hasChildren};");
      return hasChildren;
    }

    protected override void NewItem(string path, string type, object newItemValue) {
      TraceDebug($"NewItem.path: \"{path}\" type: \"{type}\" newItemValue: \"{newItemValue}\"");
    }

    protected override void CopyItem(string path, string copyPath, bool recurse) {
      TraceDebug($"CopyItem.path: \"{path}\" copyPath: \"{copyPath}\" recurse: \"{recurse}\"");
    }

    protected override void RemoveItem(string path, bool recurse) {
      TraceDebug($"RemoveItem.path: \"{path}\" recurse: \"{recurse}\"");

      if (recurse) {
        WriteError(new ErrorRecord(new NotSupportedException("Remove-Item -Recurse switch not supported."), "RemoveItemRecurseNotSupported", ErrorCategory.NotImplemented, path));
        return;
      }

      IDriveItem item = GetItemValue(path);
      if (null == item) {
        string message = $"The item at the path \"{path}\" not found, Remove-Item failed.";
        WriteError(new ErrorRecord(new ItemNotFoundException(message), "RemoveItemItemNotFound", ErrorCategory.InvalidOperation, path));
        return;
      }

      // ReSharper disable once ConditionIsAlwaysTrueOrFalse
      item.RemoveItem(recurse);
    }

    #endregion Container Overloads

    #region Navigation

    protected override bool IsItemContainer(string path) {
      IDriveItem item = GetItemValue(path);
      return item is IContainer;
    }

    protected override string GetChildName(string path) {
      if (PathIsDrive(path)) {
        TraceDebug($"GetChildName.path: {path} path is drive so returning path, which was received.");
        return string.Empty;
      }

      path = MakeAbsolutePath(path);

      IDriveItem item = GetItemValue(path);
      TraceDebug($"GetChildName.path: {path} is not the drive, so returning: {item.Name}");
      return item.Name;
    }

    private string MakeAbsolutePath(string path) {
      if (path.StartsWith(PSDriveInfo.Root)) {
        return path;
      }

      if (string.IsNullOrEmpty(PSDriveInfo.CurrentLocation)) {
        return $"{PSDriveInfo.Root}{path}";
      }

      string absolutePath = string.Join("\\", new [] {
        PSDriveInfo.Root,
        PSDriveInfo.CurrentLocation.Replace(path, string.Empty),
        path
      }.Where(p => !string.IsNullOrEmpty(p)).Select(p => p.TrimEnd('\\')));

      return absolutePath;
    }

    protected override string GetParentPath(string path, string root) {
      if (string.IsNullOrEmpty(path)) {
        return string.Empty;
      }

      string result = base.GetParentPath(path, root);
      result = EnsureDriveIsRooted(result);
      TraceDebug($"GetParentPath.path: {path} root: {root} returning: {result}");
      return result;
    }

    private static string EnsureDriveIsRooted(string path) {
      string str = path;
      int num = path.IndexOf(':');
      if (num != -1 && num + 1 == path.Length) str = path + "\\";
      return str;
    }

    protected override string MakePath(string parent, string child) {
      string basePath = parent;
      if (!string.IsNullOrEmpty(child)) {
        child = child.TrimEnd('\\');
      }

      string result = base.MakePath(basePath, child);
      TraceDebug($"MakePath.parent: {parent} child: {child} result: {result}");
      return result;
    }

    protected override string NormalizeRelativePath(string path, string basepath) {
      RelativePathBuilder pathBuilder = new RelativePathBuilder(() => PSDriveInfo.Root);
      pathBuilder.SetBasePath(basepath);
      pathBuilder.SetPath(path);
      return pathBuilder.ToString();
    }

    protected override void MoveItem(string path, string destination) {
      TraceDebug($"MoveItem.path: \"{path}\" destination: \"{destination}\"");
    }

    #endregion Navigation

    private void TraceDebug(string format, params object[] args) {
      string formattedMessage = string.Format(format, args);
      WriteDebug(formattedMessage);
    }

    private bool PathIsDrive(string path) {
      string root = PSDriveInfo == null ? SessionState.Drive.Current.Root : PSDriveInfo.Root;

      string pathWithoutRoot = !string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(root) ? path.Replace(root, "") : path;
      bool isPathEmpty = string.IsNullOrEmpty(pathWithoutRoot);

      string replace = !string.IsNullOrEmpty(path) ? path.Replace(root + pathSeparator, "") : path;
      bool isRootPathEmpty = string.IsNullOrEmpty(replace);

      if (isPathEmpty || isRootPathEmpty) {
        return true;
      }

      return false;
    }
  }
}