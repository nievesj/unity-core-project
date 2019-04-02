using System.Collections.Generic;

namespace Core.AI
{
    public class ConditionalBranchBlueprint : BlockBlueprint
    {
        public override Node CreateNodeInstance(NodeBlueprint node)
        {
            return new ConditionalBranch();
        }

        public override Branch CreateBranchInstance(List<BranchBlueprint> nodes)
        {
            throw new System.NotImplementedException();
        }
    }

    public class ConditionalBranch : Block
    {
        public System.Func<bool> fn;
        private bool tested = false;

        // public ConditionalBranch(System.Func<bool> fn)
        // {
        //     this.fn = fn;
        // }

        public override BehaviorTreeState Tick()
        {
            if (!tested)
            {
                tested = fn();
            }

            if (tested)
            {
                var result = base.Tick();
                if (result == BehaviorTreeState.Continue)
                    return BehaviorTreeState.Continue;
                else
                {
                    tested = false;
                    return result;
                }
            }
            else
            {
                return BehaviorTreeState.Failure;
            }
        }
    }
}