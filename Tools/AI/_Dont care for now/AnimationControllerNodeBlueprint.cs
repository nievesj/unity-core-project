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

        public override Node CreateNodeInstance(IEntityData data)
        {
            throw new System.NotImplementedException();
        }
    }
}
