using Core.AI;
using UnityEngine;

namespace RTS.AI
{
    [CreateAssetMenu(menuName = "AI/Decision/HasTargetDecision")]
    public class HasTargetDecision : Decision
    {
        public override bool Decide(IStateMachineData data)
        {
            return false;
        }

        public override void EnterDecision(IStateMachineData data) { }
        public override void ExitDecision(IStateMachineData data) { }
    }
}