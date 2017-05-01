using System.Collections.Generic;
using PSDT.TfsProvider.Foundation;

namespace PSDT.TfsProvider.Nodes {
  public class TfsProjectNode : NodeBase {
    public TfsProjectCollectionNode Parent { get; set; }
    public TfsProject TfsProject { get; set; }

    public TfsProjectNode(TfsProjectCollectionNode parent, TfsProject tfsProject) : base(parent.NodeContext, tfsProject, GetPathName(tfsProject)) {
      Parent = parent;
      TfsProject = tfsProject;
      PathSegment = parent.PathSegment + new PathSegment(GetPathName(tfsProject));
    }

    public override IEnumerable<NodeBase> GetChildNodes() {
      IEnumerable<NodeBase> childNodes = new NodeBase[] {
        new TfsBuildsNode(this),
        new TfsBuildDefinitionsNode(this)
      };

      childNodes = Filter(childNodes, node => node.Name);

      return childNodes;
    }

    public static string GetPathName(TfsProject tfsProject) {
      return tfsProject.Name;
    }
  }
}