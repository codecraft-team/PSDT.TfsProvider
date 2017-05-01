using System.Collections.Generic;
using PSDT.TfsProvider.Foundation;

namespace PSDT.TfsProvider.Nodes {
  public class TfsBuildNode : NodeBase {
    public TfsBuildsNode Parent { get; set; }
    public TfsBuild TfsBuild { get; set; }

    public TfsBuildNode(TfsBuildsNode parent, TfsBuild tfsBuild) : base(parent.NodeContext, tfsBuild, GetPathName(tfsBuild)) {
      Parent = parent;
      TfsBuild = tfsBuild;
      PathSegment = parent.PathSegment + new PathSegment(GetPathName(tfsBuild));
    }

    public override IEnumerable<NodeBase> GetChildNodes() {
      return new NodeBase[0];
    }

    public static string GetPathName(TfsBuild tfsBuild) {
      return tfsBuild.Name;
    }
  }
}