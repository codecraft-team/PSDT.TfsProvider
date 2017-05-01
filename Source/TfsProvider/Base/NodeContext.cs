using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace PSDT.TfsProvider.Foundation {
  public class NodeContext {
    public TfsDriveInfo TfsDrive { get; private set; }
    public PathSegment PathSegment { get; private set; }

    public WriteItemObjectDelegate WriteItemObject { get; set; }
    public string Filter { get; set; }
    public bool Force { get; set; }
    public Collection<string> Include { get; set; }
    public Collection<string> Exclude { get; set; }

    public NodeContext(TfsDriveInfo driveInfo) {
      Contract.Requires<ArgumentNullException>(null != driveInfo, "driveInfo");

      TfsDrive = driveInfo;
    }

    public void SetPath(PathSegment pathSegment) {
      PathSegment = pathSegment;
    }
  }
}