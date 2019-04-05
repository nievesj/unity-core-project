using UnityEngine;

namespace Core.AI
{
    public class WaitBlueprint : NodeBlueprint
    {
        [Input(ShowBackingValue.Always)]
        public EntityData input;
        
        [SerializeField]
        private float secondsToWait;
        
        public override Node CreateNodeInstance(IEntityData data)
        {
            return new Wait(secondsToWait);
        }
    }
    
    public class Wait : Node
    {
        private float _seconds = 0;
        float future = -1;

        public Wait(float seconds)
        {
            _seconds = seconds;
        }

        public override BehaviorTreeState Tick()
        {
            if (future < 0)
                future = Time.time + _seconds;

            if (Time.time >= future)
            {
                future = -1;
                return BehaviorTreeState.Success;
            }
            else
                return BehaviorTreeState.Continue;
        }

        public override void Reset()
        {
            future = -1;
        }
    }
}