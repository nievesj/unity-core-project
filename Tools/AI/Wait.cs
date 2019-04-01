using UnityEngine;

namespace Core.AI
{
    public class WaitBlueprint : NodeBlueprint
    { 
        public override IEntityData GetInputValue()
        {
            return null;
        }
        
        public override IEntityData GetOutputValue()
        {
            return null;
        }

        public override Node CreateInstance(NodeBlueprint node)
        {
            return new Wait();
        }
    }
    
    public class Wait : Node
    {
        public float seconds = 0;
        float future = -1;
        // public Wait(float seconds)
        // {
        //     this.seconds = seconds;
        // }

        public override BehaviorTreeState Tick()
        {
            if (future < 0)
                future = Time.time + seconds;

            if (Time.time >= future)
            {
                future = -1;
                return BehaviorTreeState.Success;
            }
            else
                return BehaviorTreeState.Continue;
        }
    }
}