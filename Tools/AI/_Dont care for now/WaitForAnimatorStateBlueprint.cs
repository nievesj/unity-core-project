using UnityEngine;

namespace Core.AI
{
    public class WaitForAnimatorStateBlueprint : NodeBlueprint
    {
        private Animator animator;
        private int id;
        private int layer;
        private string stateName;

        public WaitForAnimatorStateBlueprint(Animator animator, string name, int layer = 0)
        {
            this.id = Animator.StringToHash(name);
            if (!animator.HasState(layer, this.id))
            {
                Debug.LogError("The animator does not have state: " + name);
            }

            this.animator = animator;
            this.layer = layer;
            this.stateName = name;
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
            return "Wait For State : " + stateName;
        }
    }
}