using System.Collections.Generic;
using PSDT.TfsProvider.Foundation;

namespace PSDT.TfsProvider.Nodes {
  public class TfsBuildDefinitionNode : NodeBase {
    public TfsBuildDefinitionsNode Parent { get; set; }
    public TfsBuildDefinition TfsBuildDefinition { get; set; }

    public TfsBuildDefinitionNode(TfsBuildDefinitionsNode parent, TfsBuildDefinition tfsBuildDefinition) : base(parent.NodeContext, tfsBuildDefinition, GetPathName(tfsBuildDefinition)) {
      Parent = parent;
      TfsBuildDefinition = tfsBuildDefinition;
      PathSegment = parent.PathSegment + new PathSegment(GetPathName(tfsBuildDefinition));
    }

    public override IEnumerable<NodeBase> GetChildNodes() {
      return new NodeBase[0];
    }

    public static string GetPathName(TfsBuildDefinition tfsBuildDefinition) {
      return tfsBuildDefinition.Name;
    }
  }
}