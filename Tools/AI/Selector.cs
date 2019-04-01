using UnityEngine;

namespace Core.AI
{
    public class SelectorBlueprint : BranchBlueprint
    {
        public override Node CreateInstance(NodeBlueprint node)
        {
            //also add all children here
            return new Selector();
        }
    }
    
    public class Selector : Branch
    {
        public override BehaviorTreeState Tick()
        {
            var childState = children[activeChildIndex].Tick();
            var ret = BehaviorTreeState.Failure;

            switch (childState)
            {
                case BehaviorTreeState.Success:
                    activeChildIndex = 0;
                    ret = BehaviorTreeState.Success;
                    break;
                case BehaviorTreeState.Failure:
                    activeChildIndex++;
                    if (activeChildIndex == children.Count)
                    {
                        activeChildIndex = 0;
                        ret = BehaviorTreeState.Failure;
                    }
                    else
                        ret = BehaviorTreeState.Continue;

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
    }
}