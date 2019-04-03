using System.Collections;
using System.Collections.Generic;
using System.Resources;
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
        //TODO: This is a hack to force the sequence order, need to make an editor to hide it and set it when a connection is made.
        public int sequenceOrder;
        public abstract Node CreateNodeInstance(IEntityData data);
    }

    public abstract class Node
    {
        protected EntityData EntityData;

        public abstract BehaviorTreeState Tick();
        public abstract void Reset();
    }
}