using UnityEngine;

namespace Core.AI
{
    public interface IDecision
    {
        void EnterDecision(IStateMachineData data);
        bool Decide(IStateMachineData data);
        void ExitDecision(IStateMachineData data);
    }

    public abstract class Decision : ScriptableObject, IDecision
    {
        public abstract void EnterDecision(IStateMachineData data);
        public abstract bool Decide(IStateMachineData data);
        public abstract void ExitDecision(IStateMachineData data);
    }
}