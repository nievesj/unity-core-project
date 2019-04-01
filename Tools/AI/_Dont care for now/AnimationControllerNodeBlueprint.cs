using System.Collections;
using System.Collections.Generic;
using Core.Animation;
using UnityEngine;

namespace Core.AI
{
    public class AnimationControllerNodeBlueprint : NodeBlueprint
    {
        [SerializeField][Input]
        private EntityData entity;
        
        [SerializeField][Output]
        private AnimationController animationController;
        
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
