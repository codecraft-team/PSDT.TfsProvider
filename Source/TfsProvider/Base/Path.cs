using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using PSDT.TfsProvider.Nodes;

namespace PSDT.TfsProvider.Foundation {
  public class Path {
    private readonly Dictionary<string, NodeBase> _nodes;
    private readonly NodeContext _nodeContext;

    public NodeBase CurrentNode { get; private set; }

    public Path(NodeContext nodeContext) {
      Contract.Requires<ArgumentNullException>(null != nodeContext, "nodeContext");

      _nodes = new Dictionary<string, NodeBase>(new NodeKeyEqualityComparer());
      _nodeContext = nodeContext;

      CreateNodes();
    }

    private NodeBase GetNode(string path) {
      if (!NodesContainsKey(path)) {
        return new NotSupportedNode(path);
      }

      return _nodes[path];
    }

    private void CreateNodes() {
      Logger.WriteDebug("Path: building up nodes structure...");
      CurrentNode = CreateRootNode();

      IEnumerable<string> paths = _nodeContext.PathSegment.GetPathDecendants();

      foreach (string currentPath in paths) {
        Logger.WriteDebug($"Path: building up node for '${currentPath}'");

        CurrentNode.GetChildNodes().ToList().ForEach(EnsureNodeExists);

        if (NodesContainsKey(currentPath)) {
          CurrentNode = _nodes[currentPath];
        }
      }

      SetPrompt();

      Logger.WriteDebug("Path: building up nodes structure finished.");
    }

    private void SetPrompt() {
      SetDeploymentUrl();
    }

    private void SetDeploymentUrl() {
      NodeBase deploymentNode = _nodes.Values.FirstOrDefault(node => node is TfsNode);

      if (null == deploymentNode) {
        return;
      }

      string prompt = _nodeContext.TfsDrive.TfsDataService.Url;
      
      _nodeContext.TfsDrive.SetPrompt(prompt);
    }

    private NodeBase CreateRootNode() {
      EnsureNodeExists(new TfsNode(_nodeContext));
      return GetNode(PathSegment.RootPath);
    }

    private void EnsureNodeExists(NodeBase node) {
      string key = node.PathSegment.Path;

      if (!NodesContainsKey(key)) {
        _nodes.Add(key, node);
      }
      else {
        Logger.WriteVerbose($"The node for the path {key} already exists.");
      }
    }

    private bool NodesContainsKey(string key) {
      return _nodes.ContainsKey(key);
    }
  }
}