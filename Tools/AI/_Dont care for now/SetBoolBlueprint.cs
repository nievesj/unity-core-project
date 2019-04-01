using UnityEngine;

namespace Core.AI
{
    public class SetBoolBlueprint : NodeBlueprint
    {
        private Animator animator;
        private int id;
        private bool value;
        private string triggerName;

        public SetBoolBlueprint(Animator animator, string name, bool value)
        {
            this.id = Animator.StringToHash(name);
            this.animator = animator;
            this.value = value;
            this.triggerName = name;
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
            return "SetBool : " + triggerName + " = " + value.ToString();
        }
    }
}