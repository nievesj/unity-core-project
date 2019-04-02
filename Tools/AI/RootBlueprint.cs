using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XNode;

namespace Core.AI
{
    public class RootBlueprint : BlockBlueprint
    {
        public override Node CreateNodeInstance(NodeBlueprint node)
        {
            return new Root();
        }

        public override Branch CreateBranchInstance(List<BranchBlueprint> nodes)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class Root : Block
    {        
        public bool IsTerminated { get; set;}

        public void Init(IEntityData data)
        {
            // output = data as EntityData;
            // children = Children();
                
            //since this is the root the activeChildIndex children has to be 1
            if (children.Count > 0)
                activeChildIndex = 1;
            else
                IsTerminated = true; 
        }

        public override BehaviorTreeState Tick()
        {
            if (IsTerminated) return BehaviorTreeState.Abort;
            while (true)
            {
                switch (children[activeChildIndex].Tick())
                {
                    case BehaviorTreeState.Continue:
                        return BehaviorTreeState.Continue;
                    case BehaviorTreeState.Abort:
                        IsTerminated = true;
                        return BehaviorTreeState.Abort;
                    default:
                        activeChildIndex++;
                        if (activeChildIndex == children.Count)
                        {
                            activeChildIndex = 0;
                            return BehaviorTreeState.Success;
                        }
                        continue;
                }
            }
        }
    }
}