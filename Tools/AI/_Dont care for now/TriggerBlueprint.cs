using UnityEngine;

namespace Core.AI
{
    public class Trigger : Node
    {
        private Animator animator;
        private int id;
        private string triggerName;
        private bool set = true;

        public override BehaviorTreeState Tick()
        {
            if (set)
                animator.SetTrigger(id);
            else
                animator.ResetTrigger(id);

            return BehaviorTreeState.Success;
        }

        public override void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class TriggerBlueprint : NodeBlueprint
    {
        private Animator animator;
        private int id;
        private string triggerName;
        private bool set = true;

        //if set == false, it reset the trigger istead of setting it.
        public TriggerBlueprint(Animator animator, string name, bool set = true)
        {
            this.id = Animator.StringToHash(name);
            this.animator = animator;
            this.triggerName = name;
            this.set = set;
        }

        public override Node CreateNodeInstance(IEntityData data)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "Trigger : " + triggerName;
        }
    }
}