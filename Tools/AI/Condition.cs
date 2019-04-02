namespace Core.AI
{
    public class ConditionBlueprint : NodeBlueprint
    {
        public override Node CreateNodeInstance(IEntityData data)
        {
            return new Condition();
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