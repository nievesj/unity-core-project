using System.Linq;
using Core.Animation;
using UnityEngine;
using UnityEngine.AI;
using XNode;

namespace Core.AI
{
    public interface IEntityData
    {
        AnimationController AnimationController { get; }
        NavMeshAgent NavMeshAgent { get; }
        Transform Target { get; }

        void SetTarget(Transform transform);
    }

    [System.Serializable]
    public class EntityData : IEntityData
    {
        public AnimationController AnimationController { get; set; }
        public NavMeshAgent NavMeshAgent { get; set; }
        public Transform Target { get; private set; }

        public void SetTarget(Transform transform)
        {
            Target = transform;
        }
    }

    [CreateAssetMenu(menuName = "AI/BehaviorTree", fileName = "New BehaviorTree")]
    public class BehaviorTree : NodeGraph
    {
        public bool isTerminated = false;
        public int activeChild;

        public void CreateBehaviourTreeInstance(EntityData entity)
        {
            //Find the first node without any inputs. This is the starting node.
            var root = nodes.Find(x => x is RootBlueprint) as RootBlueprint;
            
            var port = root.GetOutputPort("output");
            var children = root.GetConnectedNodes(port.GetConnections());
            
            var pon = root.CreateNodeInstance(root);
        }
    }
}