using System;
using System.Diagnostics;
using System.Linq;

namespace PSDT.TfsProvider.Base {
  public class RelativePathBuilder {
    private readonly string _pathSeparator;
    private readonly Func<string> _getDriveRoot;
    private string _basePath;
    private string _path;

    public RelativePathBuilder(Func<string> getDriveRoot) : this(getDriveRoot, "\\") {
    }

    public RelativePathBuilder(Func<string> getDriveRoot, string pathSeparator) {
      _getDriveRoot = getDriveRoot;
      _pathSeparator = pathSeparator;
    }

    public void SetPath(string path) {
      _path = path;
    }

    public void SetBasePath(string basePath) {
      _basePath = basePath;
    }

    public override string ToString() {
      return GetRelativePath(_path, _basePath);
    }

    protected string GetRelativePath(string path, string basepath) {
      string sourcePath = EnsureCorrectPathSeparator(basepath);
      sourcePath = RemoveDriveFromPath(sourcePath);
      string targetPath = EnsureCorrectPathSeparator(path);
      targetPath = RemoveDriveFromPath(targetPath);

      //If navigating from "TFS:\" to "TFS:\Source"
      bool fromEmptyToTarget = string.IsNullOrEmpty(sourcePath);
      if (fromEmptyToTarget) {
        return targetPath;
      }

      bool targetIsChildPath = targetPath.Contains(sourcePath);
      if (targetIsChildPath) {
        //If navigating from "TFS:\Source" to "TFS:\Source\SubFolder1" then return ".\SubFolder1"
        //If navigating from "TFS:\Source" to "TFS:\Source" then return ".\"
        string result = targetPath.Substring(sourcePath.Length);
        return $"{result.Trim(_pathSeparator.ToCharArray())}";
      }

      //If navigating from "TFS:\Source\SubFolder1" to "TFS:\Temp" then return "..\..\Temp"
      //If navigating from "TFS:\Source\SubFolder1" to "TFS:\Source" then return "..\"
      bool sourceIsChildPath = sourcePath.Contains(targetPath);
      if (sourceIsChildPath) {
        string childPath = sourcePath.Substring(targetPath.Length);
        string pathToParent = GetRelativePathToRoot(childPath);
        return $"{pathToParent}{_pathSeparator}";
      }
      string pathToRoot = GetRelativePathToRoot(sourcePath);
      return $"{pathToRoot}{_pathSeparator}{targetPath}";
    }

    private string GetRelativePathToRoot(string path) {
      int pathDepth = path.Split(_pathSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length;
      return string.Join(_pathSeparator, Enumerable.Range(0, pathDepth).Select(i => ".."));
    }

    private string EnsureCorrectPathSeparator(string path) {
      return path.Replace("/", _pathSeparator);
    }

    private string RemoveDriveFromPath(string path) {
      string result = path;
      string root = _getDriveRoot();

      if (result == null) {
        result = String.Empty;
      }

      if (result.Contains(root)) {
        result = result.Substring(result.IndexOf(root, StringComparison.OrdinalIgnoreCase) + root.Length);
      }

      return result;
    }
  }
}