using System.Collections.Generic;
using UnityEngine;

namespace Core.AI
{
    [System.Serializable]
    public struct LoopOptions
    {
        public bool loopForever;
        public int numberOfTimesToLoop;
    }
    
    public class LoopBlueprint : BlockBlueprint
    {
        [Input(ShowBackingValue.Always)]
        public EntityData input;
        
        [SerializeField]
        private LoopOptions loopOptions;
        
        public override Node CreateNodeInstance(IEntityData data)
        {
            throw new System.NotImplementedException();
        }

        public override Branch CreateBranchInstance(List<Node> nodes)
        {
            return new Loop(nodes, loopOptions);
        }
    }
    
    public class Loop : Block
    {
        private LoopOptions _options;
        private int currentCount = 0;

        public Loop(List<Node> nodes, LoopOptions options)
        {
            children = nodes;
            _options = options;
        }

        // public override BehaviorTreeState Tick()
        // {
        //     if ((_options.numberOfTimesToLoop > 0 && currentCount < _options.numberOfTimesToLoop) || _options.loopForever)
        //     {
        //         var result = base.Tick();
        //         switch (result)
        //         {
        //             case BehaviorTreeState.Continue:
        //                 return BehaviorTreeState.Continue;
        //             default:
        //                 currentCount++;
        //                 if (currentCount == _options.numberOfTimesToLoop)
        //                 {
        //                     currentCount = 0;
        //                     return BehaviorTreeState.Success;
        //                 }
        //
        //                 return BehaviorTreeState.Continue;
        //         }
        //     }
        //
        //     return BehaviorTreeState.Success;
        // }
        
        public override void Reset()
        {
            currentCount = 0;
            activeChildIndex = 0;
        }
        
        public override BehaviorTreeState Tick()
        {
            var result = BehaviorTreeState.Continue;
            
            if ((_options.numberOfTimesToLoop > 0 && currentCount < _options.numberOfTimesToLoop) || _options.loopForever)
            {
                switch (children[activeChildIndex].Tick())
                {
                    case BehaviorTreeState.Continue:
                        result = BehaviorTreeState.Continue;
                        break;
                    case BehaviorTreeState.Success:
                        if (_options.loopForever || currentCount < _options.numberOfTimesToLoop)
                        {
                            currentCount++;
                            children[activeChildIndex].Reset();
                            result = BehaviorTreeState.Continue;
                        }
                        else
                        {
                            result = BehaviorTreeState.Success;
                        }
                        break;
                    default:
                        activeChildIndex++;
                        if (activeChildIndex == children.Count)
                        {
                            activeChildIndex = 0;
                            Debug.LogError("paap");

                            result = _options.loopForever ? BehaviorTreeState.Continue : BehaviorTreeState.Success;
                        }

                        result = BehaviorTreeState.Continue;
                        break;
                }
            }

            return result;

        }
    }
}