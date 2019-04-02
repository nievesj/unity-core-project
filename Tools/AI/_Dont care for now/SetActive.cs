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

        public override Node CreateNodeInstance(IEntityData data)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "Set Active : " + gameObject.name + " = " + active;
        }
    }
}