using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using PSDT.TfsProvider.Foundation;

namespace PSDT.TfsProvider
{
  [CmdletProvider("TeamFoundationServer", ProviderCapabilities.Include | ProviderCapabilities.Exclude | ProviderCapabilities.Filter)]
  public class TfsCmdletProvider : NavigationCmdletProvider {

    public TfsCmdletProvider() {
      Logger.RegisterProvider(this);
    }

    protected override PSDriveInfo NewDrive(PSDriveInfo drive) {
      return new TfsDriveInfo(drive, (TfsBuildDriveParameter)DynamicParameters);
    }

    [Pure, ExcludeFromCodeCoverage]
    protected virtual PSDriveInfo GetPsDriveInfo() {
      return PSDriveInfo;
    }

    private Path GetPath(string path) {
      Contract.Requires<InvalidOperationException>(null != GetPsDriveInfo(), "An instance of PSDriveInfo required to build path.");
      Contract.Requires<InvalidOperationException>(GetPsDriveInfo() is TfsDriveInfo, "PSDriveInfo must be an instance of CrmDriveInfo.");

      PathSegment pathSegment = new PathSegment(path);

      NodeContext nodeContext = new NodeContext((TfsDriveInfo)GetPsDriveInfo()) {
        Filter = Filter,
        Force = Force,
        Include = Include ?? new Collection<string>(),
        Exclude = Exclude ?? new Collection<string>(),
        WriteItemObject = WriteItemObject
      };
      nodeContext.SetPath(pathSegment);

      Path crmPath = new Path(nodeContext);
      return crmPath;
    }

    protected override object NewDriveDynamicParameters() {
      return new TfsBuildDriveParameter();
    }

    protected override bool IsValidPath(string path) {
      bool isPathValid = GetPath(path).CurrentNode.PathSegment.Equals(path);
      TraceDebug($"Path '{path}' is valid: {isPathValid}.");
      return isPathValid;
    }

    protected override bool IsItemContainer(string path) {
      bool isContainer = GetPath(path).CurrentNode.IsContainer;
      TraceDebug($"Item '{path}' is container: {isContainer}.");
      return isContainer;
    }

    protected override bool ItemExists(string path) {
      bool itemExists = GetPath(path).CurrentNode.PathSegment.Equals(path);
      TraceDebug($"Item '{path}' exists: {itemExists}.");
      return itemExists;
    }

    protected override string MakePath(string parent, string child) {
      PathSegment parentSegment = new PathSegment(parent);
      PathSegment childSegment = new PathSegment(child);

      PathSegment combinedSegment = parentSegment + childSegment;

      string result = combinedSegment.Path;
      TraceDebug($"MakePath parent: '{parent}' child: '{child}' result: '{result}'");
      return result;
    }

    protected override void GetChildItems(string path, bool recurse) {
      TraceDebug("GetChildItems path: '{0}' recurse: {1}", path, recurse);
      GetPath(path).CurrentNode.GetChildItems();
    }

    protected override void RemoveItem(string path, bool recurse) {
      TraceDebug("RemoveItem: '{0}' recurse: {1}", path, recurse);

      GetPath(path).CurrentNode.RemoveItem(recurse);
    }

    protected override string GetChildName(string path) {
      string childName = GetPath(path).CurrentNode.PathSegment.Segments.LastOrDefault();

      TraceDebug("GetChildName of {0} is {1}", path, childName);

      return childName;
    }

    protected override string GetParentPath(string path, string root) {
      PathSegment pathSegment = GetPath(path).CurrentNode.PathSegment;

      int noOfSegments = pathSegment.Segments.Count();

      string parentPathSegment = pathSegment.GetPathDecendants().Take(noOfSegments - 1).LastOrDefault();
      string result = parentPathSegment ?? root;

      TraceDebug($"GetParentPath: {path} root: {root} result: {result}");

      return result;
    }

    protected override void GetChildNames(string path, ReturnContainers returnContainers) {
      GetPath(path).CurrentNode.GetChildNames(returnContainers);

      TraceDebug("GetChildNames of {0} ({1}).", path, returnContainers);
    }

    protected override bool HasChildItems(string path) {
      bool hasChildItems = GetPath(path).CurrentNode.HasChildItems();
      TraceDebug("{0} {1} child items.", path, hasChildItems ? "has" : "doesn't have");
      return hasChildItems;
    }

    private void TraceDebug(string format, params object[] args) {
      string formattedMessage = string.Format(format, args);
      Logger.WriteDebug(formattedMessage);
    }
  }
}
