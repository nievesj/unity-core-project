using System.Collections.Generic;
using XNode;

namespace Core.AI
{
    public class NodeCollection
    {
        public List<NodeBlueprint> nodeBlueprints;
        public List<Node> nodes;

        public NodeCollection()
        {
            nodeBlueprints = new List<NodeBlueprint>();
            nodes = new List<Node>();
        }
    }

    public abstract class BranchBlueprint : NodeBlueprint
    {
        [Output(connectionType = ConnectionType.Multiple)]
        public EntityData output;

        public List<NodeBlueprint> Children { get; protected set; }

        public override Node CreateNodeInstance(IEntityData data)
        {
            return null;
        }

        public abstract Branch CreateBranchInstance(List<Node> nodes);

        public virtual List<BranchBlueprint> GetChildren()
        {
            var port = GetOutputPort("output");
            var children = GetConnectedNodes(port.GetConnections());

            return null;
        }

        public virtual List<NodeBlueprint> GetConnectedNodes(List<NodePort> connections)
        {
            var nodes = new List<NodeBlueprint>();
            for (var i = 0; i < connections.Count; i++)
            {
                var node = connections[i].node;

                if (node is BranchBlueprint)
                {
                    var branch = node as BranchBlueprint;
                    var port = branch.GetOutputPort("output");
                    branch.Children = GetConnectedNodes(port.GetConnections());
                    nodes.Add(branch);
                }
                else
                    nodes.Add(node as NodeBlueprint);
            }

            return nodes;
        }
    }

    public abstract class Branch : Node
    {
        protected int activeChildIndex;
        protected List<Node> children;

        public int ActiveChild()
        {
            return activeChildIndex;
        }

        public virtual void ResetChildren()
        {
            activeChildIndex = 0;
            for (var i = 0; i < children.Count; i++)
            {
                var b = children[i] as Branch;
                if (b != null)
                {
                    b.ResetChildren();
                }
            }
        }
    }
}