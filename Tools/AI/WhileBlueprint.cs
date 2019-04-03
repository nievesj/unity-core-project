using System.Collections.Generic;

namespace Core.AI
{
    public class WhileBlueprint : BlockBlueprint
    {
        public override Node CreateNodeInstance(IEntityData data)
        {
            return new While();
        }

        public override Branch CreateBranchInstance(List<Node> nodes)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class While : Block
    {
        public System.Func<bool> fn;

        public override BehaviorTreeState Tick()
        {
            if (fn())
                base.Tick();
            else
            {
                //if we exit the loop
                ResetChildren();
                return BehaviorTreeState.Failure;
            }

            return BehaviorTreeState.Continue;
        }

        public override void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}