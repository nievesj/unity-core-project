using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using XNode;
using Zenject;

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
        public abstract IEntityData GetInputValue();
        public abstract IEntityData GetOutputValue();

        public abstract Node CreateInstance(NodeBlueprint node);
    }
    
    public abstract class Node
    {
        protected EntityData EntityData;
        
        public abstract BehaviorTreeState Tick();
    }
}
