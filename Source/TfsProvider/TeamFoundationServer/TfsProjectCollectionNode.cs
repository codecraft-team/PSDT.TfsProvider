using System.Collections.Generic;
using System.Linq;
using PSDT.TfsProvider.Foundation;

namespace PSDT.TfsProvider.Nodes {
  public class TfsProjectCollectionNode : NodeBase {
    public TfsNode Parent { get; set; }
    public TfsProjectCollection TfsProjectCollection { get; set; }

    public TfsProjectCollectionNode(TfsNode parent, TfsProjectCollection tfsProjectCollection) : base(parent.NodeContext, tfsProjectCollection, GetPathName(tfsProjectCollection)) {
      Parent = parent;
      TfsProjectCollection = tfsProjectCollection;
      PathSegment = parent.PathSegment + new PathSegment(GetPathName(tfsProjectCollection));
    }

    public override IEnumerable<NodeBase> GetChildNodes() {
      IEnumerable<TfsProject> projectCollections = NodeContext.TfsDrive.TfsDataService.RetrieveProjects(TfsProjectCollection);

      projectCollections = Filter(projectCollections, projectCollection => projectCollection.Name);

      return projectCollections.Select(projectCollection => new TfsProjectNode(this, projectCollection));
    }

    public static string GetPathName(TfsProjectCollection tfsProjectCollection) {
      return tfsProjectCollection.Name;
    }
  }
}