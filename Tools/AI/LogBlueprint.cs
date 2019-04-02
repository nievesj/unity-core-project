using UnityEngine;

namespace Core.AI
{
    public class LogBlueprint : NodeBlueprint
    {
        [Input(ShowBackingValue.Always)]
        public EntityData input;
        
        [SerializeField]
        private string message;
        
        public override IEntityData GetInputValue()
        {
            return null;
        }
        
        public override IEntityData GetOutputValue()
        {
            return null;
        }

        public override Node CreateNodeInstance(NodeBlueprint node)
        {
            return new Log(message);
        }
    }

    public class Log : Node
    {
        private string _message;
        
        public Log(string msg)
        {
            msg = _message;
        }
        
        public override BehaviorTreeState Tick()
        {
            Debug.Log(_message);
            return BehaviorTreeState.Success;
        }
    }
}