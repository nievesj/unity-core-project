namespace Core.AI
{
    public class ConditionBlueprint : NodeBlueprint
    {
        public override Node CreateNodeInstance(NodeBlueprint node)
        {
            return new Condition();
        }
        
        public override IEntityData GetOutputValue()
        {
            return null;
        }
        
        public override IEntityData GetInputValue()
        {
            return null;
        }
    }

    public class Condition : Node
    {
        public System.Func<bool> fn;

        // public Condition(System.Func<bool> fn)
        // {
        //     this.fn = fn;
        // }

        public override BehaviorTreeState Tick()
        {
            return fn() ? BehaviorTreeState.Success : BehaviorTreeState.Failure;
        }
    }
}