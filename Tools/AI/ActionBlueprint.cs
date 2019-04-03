namespace Core.AI
{
    public enum ActionState
    {
        NotStarted,
        Started,
        Completed
    }

    public abstract class ActionBlueprint : NodeBlueprint
    {
        [Input(ShowBackingValue.Always)]
        public EntityData input;
    }

    public abstract class Action : Node
    {
        protected ActionState ActionState = ActionState.NotStarted;

        public override BehaviorTreeState Tick()
        {
            switch (ActionState)
            {
                case ActionState.NotStarted:
                    ActionState = ActionState.Started;
                    StartAction();
                    return BehaviorTreeState.Continue;
                case ActionState.Started:
                    return BehaviorTreeState.Continue;
                case ActionState.Completed:
                    return BehaviorTreeState.Success;
                default:
                    return BehaviorTreeState.Continue;
            }
        }

        protected abstract void StartAction();
    }
}