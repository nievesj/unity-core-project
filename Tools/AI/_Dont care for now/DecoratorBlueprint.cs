using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.AI
{
    public abstract class Decorator : Node
    {
        protected Node child;
        public Decorator Do(Node child)
        {
            this.child = child;
            return this;
        }
    }
    
    public abstract class DecoratorBlueprint : NodeBlueprint
    {
        protected NodeBlueprint child;
        
        public DecoratorBlueprint Do(NodeBlueprint child)
        {
            this.child = child;
            return this;
        }
    }
}
