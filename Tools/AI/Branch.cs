using System.Collections.Generic;

namespace Core.AI
{
    public abstract class BranchBlueprint : NodeBlueprint
    {
        protected int activeChildIndex;
        protected List<NodeBlueprint> children = new List<NodeBlueprint>();

        public override IEntityData GetInputValue()
        {
            return null;
        }

        public override IEntityData GetOutputValue()
        {
            return null;
        }
    }

    public abstract class Branch : Node
    {
        protected int activeChildIndex;
        protected List<Node> children = new List<Node>();

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