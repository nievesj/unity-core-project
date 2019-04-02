namespace Core.AI
{
    public abstract class BlockBlueprint : BranchBlueprint { }

    public abstract class Block : Branch
    {
        public override BehaviorTreeState Tick()
        {
            switch (children[activeChildIndex].Tick())
            {
                case BehaviorTreeState.Continue:
                    return BehaviorTreeState.Continue;
                default:
                    activeChildIndex++;
                    if (activeChildIndex == children.Count)
                    {
                        activeChildIndex = 0;
                        return BehaviorTreeState.Success;
                    }

                    return BehaviorTreeState.Continue;
            }
        }
    }
}