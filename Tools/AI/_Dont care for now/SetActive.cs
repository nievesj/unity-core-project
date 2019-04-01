using UnityEngine;

namespace Core.AI
{
    public class SetActiveBlueprint : NodeBlueprint
    {
        private GameObject gameObject;
        private bool active;

        public SetActiveBlueprint(GameObject gameObject, bool active)
        {
            this.gameObject = gameObject;
            this.active = active;
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

        public override string ToString()
        {
            return "Set Active : " + gameObject.name + " = " + active;
        }
    }
}