using System.Collections.Generic;
using System.Linq;
using PSDT.TfsProvider.Foundation;

namespace PSDT.TfsProvider.Nodes {
  public class TfsBuildsNode : NodeBase {
    public const string NodeName = "Builds";
    public TfsProjectNode Parent { get; set; }
    public string ProjectName => Parent.TfsProject.Name;
    public string ProjectUrl => Parent.TfsProject.Url;

    public TfsBuildsNode(TfsProjectNode parent) : base(parent.NodeContext, NodeName) {
      Parent = parent;
      PathSegment = parent.PathSegment + new PathSegment(Name);
    }

    public override IEnumerable<NodeBase> GetChildNodes() {
      IEnumerable<TfsBuild> tfsBuilds = NodeContext.TfsDrive.TfsDataService.RetrieveBuilds(Parent.TfsProject);
      tfsBuilds = Filter(tfsBuilds, tfsBuild => tfsBuild.Name);

      return tfsBuilds.Select(tfsBuild => new TfsBuildNode(this, tfsBuild));
    }
  }
}