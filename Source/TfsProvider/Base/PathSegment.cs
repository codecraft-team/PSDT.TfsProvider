using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace PSDT.TfsProvider.Foundation {
  public class PathSegment {
    public const string RootPath = "";

    private readonly string _pathSegment;

    private string _drive;

    private string _path;

    public string FullPathSegment => GetFullPath();

    /// <summary>
    /// Gets the path segments without psdrive.
    /// </summary>
    public IEnumerable<string> Segments => SplitPathSegments();

    public string Drive => _drive;

    public string Path => _path;

    public PathSegment(string pathSegment) {
      Contract.Requires<ArgumentNullException>(null != pathSegment);

      _pathSegment = pathSegment;
      Logger.WriteVerbose($"PathSegment: {pathSegment}");

      ParsePath();
    }

    public void NormalizePath() {
      _path = _path.Replace('/', '\\');
      _path = !string.IsNullOrEmpty(_path) && !_path.StartsWith(@"\") ? @"\" + _path : _path;
    }

    private void ParsePath() {
      if (ContainsCrmDrive()) {
        SplitDriveAndPath();
      }
      else {
        _path = _pathSegment;
      }

      NormalizePath();
    }

    public IEnumerable<string> GetPathDecendants() {
      StringBuilder pathBuilder = new StringBuilder();

      foreach (string segment in Segments) {
        yield return pathBuilder.AppendFormat("\\{0}", segment).ToString();
      }
    }

    public bool Equals(string path) {
      return string.Equals(path.TrimStart('\\'), _path.TrimStart('\\'), StringComparison.OrdinalIgnoreCase);
    }

    private PathSegment Append(PathSegment pathSegment) {
      return new PathSegment($@"{_path}{pathSegment.Path}");
    }

    private bool ContainsCrmDrive() {
      return _pathSegment.Contains(":");
    }

    private void SplitDriveAndPath() {
      string[] segments = _pathSegment.Split(new[] { @":" }, StringSplitOptions.RemoveEmptyEntries);

      _drive = segments.First();
      _path = segments.Last();
    }

    private string[] SplitPathSegments() {
      return _path.Split(new[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
    }

    private string GetFullPath() {
      return string.IsNullOrEmpty(_drive) ? _path : $"{_drive}:{_path}";
    }

    public override string ToString() {
      return FullPathSegment;
    }

    public static PathSegment operator +(PathSegment first, PathSegment second) {
      return first.Append(second);
    }
  }

}
