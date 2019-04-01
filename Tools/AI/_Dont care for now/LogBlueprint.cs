using UnityEngine;

namespace Core.AI
{
    public class LogBlueprint : NodeBlueprint
    {
        private string msg;

        public LogBlueprint(string msg)
        {
            this.msg = msg;
        }
        
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
            throw new System.NotImplementedException();
        }
    }
}