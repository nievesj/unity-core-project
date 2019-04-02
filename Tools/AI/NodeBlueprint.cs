using System.Collections;
using System.Collections.Generic;
using XNode;

namespace Core.AI
{
    public enum BehaviorTreeState
    {
        Failure,
        Success,
        Continue,
        Abort
    }

    public abstract class NodeBlueprint : XNode.Node
    {
        public abstract Node CreateNodeInstance(IEntityData data);
    }

    public abstract class Node
    {
        protected EntityData EntityData;

        public abstract BehaviorTreeState Tick();
    }
}