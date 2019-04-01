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

        [Output]
        public EntityData output;

        public override IEntityData GetInputValue()
        {
            return input;
        }

        public override IEntityData GetOutputValue()
        {
            return output;
        }
    }

    public abstract class Action : Node
    {
        protected ActionState actionState = ActionState.NotStarted;

        public override BehaviorTreeState Tick()
        {
            switch (actionState)
            {
                case ActionState.NotStarted:
                    actionState = ActionState.NotStarted;
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