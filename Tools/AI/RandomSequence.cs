using System.Collections.Generic;
using UnityEngine;

namespace Core.AI
{
    public class RandomSequenceBlueprint : BlockBlueprint
    {
        public override Node CreateNodeInstance(NodeBlueprint node)
        {
            return new RandomSequence();
        }

        public override Branch CreateBranchInstance(List<BranchBlueprint> nodes)
        {
            throw new System.NotImplementedException();
        }
    }

    public class RandomSequence : Block
    {
        private int[] m_Weight = null;
        private int[] m_AddedWeight = null;

        public override BehaviorTreeState Tick()
        {
            if (activeChildIndex == -1)
                PickNewChild();

            var result = children[activeChildIndex].Tick();

            switch (result)
            {
                case BehaviorTreeState.Continue:
                    return BehaviorTreeState.Continue;
                default:
                    PickNewChild();
                    return result;
            }
        }

        private void PickNewChild()
        {
            var choice = Random.Range(0, m_AddedWeight[m_AddedWeight.Length - 1]);

            for (var i = 0; i < m_AddedWeight.Length; ++i)
            {
                if (choice - m_AddedWeight[i] <= 0)
                {
                    activeChildIndex = i;
                    break;
                }
            }
        }
    }
}