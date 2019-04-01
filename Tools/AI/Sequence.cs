namespace Core.AI
{
    public class SequenceBlueprint : BranchBlueprint
    {
        public override Node CreateInstance(NodeBlueprint node)
        {
            //also add all children here
            return new Sequence();
        }
    }

    public class Sequence : Branch
    {
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
    }
}