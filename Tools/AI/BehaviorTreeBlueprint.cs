using System.Collections.Generic;
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
        Transform Self { get; }
        Transform Target { get; }

        void SetTarget(Transform transform);
    }

    [System.Serializable]
    public class EntityData : IEntityData
    {
        public AnimationController AnimationController { get; set; }
        public NavMeshAgent NavMeshAgent { get; set; }
        public Transform Self { get; set; }
        public Transform Target { get; private set; }

        public void SetTarget(Transform transform)
        {
            Target = transform;
        }
    }

    [CreateAssetMenu(menuName = "AI/BehaviorTree", fileName = "New BehaviorTree")]
    public class BehaviorTreeBlueprint : NodeGraph
    {
        public bool isTerminated = false;
        public int activeChild;

        public BehaviorTree CreateBehaviourTree(IEntityData data)
        {
            var blueprintHierarchy = CreateBlueprintHierarchy();
            return CreateTree(data, blueprintHierarchy);
        }

        private List<NodeBlueprint> CreateBlueprintHierarchy()
        {
            //Find Root
            var root = nodes.Find(x => x is RootBlueprint) as RootBlueprint;

            var port = root.GetOutputPort("output");
            return root.GetConnectedNodes(port.GetConnections());
        }

        private BehaviorTree CreateTree(IEntityData data, List<NodeBlueprint> blueprints)
        {
            return new BehaviorTree(data, blueprints);
        }
    }

    public class BehaviorTree
    {
        private Root _root;
        private IEntityData _data;

        public BehaviorTree(IEntityData data, List<NodeBlueprint> blueprints)
        {
            _data = data;
            var nodes = CreateHierarchy(blueprints);
            _root = new Root(nodes);
        }

        public BehaviorTreeState Tick()
        {
            return _root.Tick();
        }

        private List<Node> CreateHierarchy(List<NodeBlueprint> blueprints)
        {
            var nodes = new List<Node>();

            for (var i = 0; i < blueprints.Count; i++)
            {
                if (blueprints[i] is BranchBlueprint)
                {
                    var branch = (BranchBlueprint) blueprints[i];
                    var children = CreateHierarchy(branch.Children);
                    
                    var node = branch.CreateBranchInstance(children);
                    nodes.Add(node);
                }
                else
                {
                    var node = blueprints[i].CreateNodeInstance(_data);
                    nodes.Add(node);
                }
            }

            return nodes;
        }
    }
}