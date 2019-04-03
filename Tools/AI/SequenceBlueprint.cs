using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.AI
{
    public class SequenceBlueprint : BranchBlueprint
    {
        [Input(ShowBackingValue.Always)]
        public EntityData input;
        
        public override Node CreateNodeInstance(IEntityData data)
        {
            return null;
        }

        public override Branch CreateBranchInstance(List<Node> nodes)
        {
           return new Sequence(nodes);
        }
    }

    public class Sequence : Branch
    {
        public Sequence(List<Node> nodes)
        {
            children = nodes;
        }
        
        public override BehaviorTreeState Tick()
        {
            var childState = children[activeChildIndex].Tick();
            var ret = BehaviorTreeState.Failure;

            switch (childState)
            {
                case BehaviorTreeState.Success:
                    activeChildIndex++;
                    if (activeChildIndex == children.Count)
                    {
                        activeChildIndex = 0;
                        ret = BehaviorTreeState.Success;
                    }
                    else
                        ret = BehaviorTreeState.Continue;

                    break;
                case BehaviorTreeState.Failure:
                    activeChildIndex = 0;
                    ret = BehaviorTreeState.Failure;
                    break;
                case BehaviorTreeState.Continue:
                    ret = BehaviorTreeState.Continue;
                    break;
                case BehaviorTreeState.Abort:
                    activeChildIndex = 0;
                    ret = BehaviorTreeState.Abort;
                    break;
            }

            return ret;
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}