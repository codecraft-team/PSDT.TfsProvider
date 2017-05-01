using System.Collections.Generic;
using System.Linq;
using PSDT.TfsProvider.Foundation;

namespace PSDT.TfsProvider.Nodes {
  public class TfsNode : NodeBase {
    public TfsNode(NodeContext nodeContext) : base(nodeContext, string.Empty) {
      PathSegment = new PathSegment(string.Empty);
    }

    public override IEnumerable<NodeBase> GetChildNodes() {
      IEnumerable<TfsProjectCollection> projectCollections = RetrieveTfsProjectCollections();

      return projectCollections.Select(projectCollection => new TfsProjectCollectionNode(this, projectCollection));
    }

    private IEnumerable<TfsProjectCollection> RetrieveTfsProjectCollections() {
      IEnumerable<TfsProjectCollection> projectCollections = NodeContext.TfsDrive.TfsDataService.RetrieveProjectCollections();

      projectCollections = Filter(projectCollections, projectCollection => projectCollection.Name);

      return projectCollections;
    }
  }
}