using System;
using System.Collections.Generic;
using System.Linq;
using PSDT.TfsProvider.Foundation;

namespace PSDT.TfsProvider.Nodes {
  public class TfsBuildDefinitionsNode : NodeBase {
    public const string NodeName = "BuildDefinitions";
    public TfsProjectNode Parent { get; set; }
    public string ProjectName => Parent.TfsProject.Name;
    public string ProjectUrl => Parent.TfsProject.Url;

    public TfsBuildDefinitionsNode(TfsProjectNode parent) : base(parent.NodeContext, NodeName) {
      Parent = parent;
      PathSegment = parent.PathSegment + new PathSegment(Name);
    }

    public override IEnumerable<NodeBase> GetChildNodes() {
      if (NodeContext.Include.Count > 1) {
        throw new InvalidOperationException($"The current path supports zero or one include parameter, but {NodeContext.Include.Count} received.");
      }

      IEnumerable<TfsBuildDefinition> tfsBuilds = NodeContext.TfsDrive.TfsDataService.RetrieveBuildDefinitions(Parent.TfsProject, NodeContext.Include.FirstOrDefault());
      tfsBuilds = Filter(tfsBuilds, tfsBuild => tfsBuild.Name);

      return tfsBuilds.Select(tfsBuild => new TfsBuildDefinitionNode(this, tfsBuild));
    }
  }
}