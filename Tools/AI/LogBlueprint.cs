using UnityEngine;

namespace Core.AI
{
    public class LogBlueprint : NodeBlueprint
    {
        [Input(ShowBackingValue.Always)]
        public EntityData input;
        
        [SerializeField]
        private string message;

        public override Node CreateNodeInstance(IEntityData data)
        {
            return new Log(message);
        }
    }

    public class Log : Node
    {
        private string _message;
        
        public Log(string msg)
        {
            _message = msg;
        }
        
        public override BehaviorTreeState Tick()
        {
            Debug.Log(_message);
            return BehaviorTreeState.Success;
        }
    }
}