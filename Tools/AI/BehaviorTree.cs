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

        public Tree CreateBehaviourTree(IEntityData data)
        {
            var blueprintHierarchy = CreateBlueprintHierarchy();
            return CreateTree(data, blueprintHierarchy);
        }

        private IEnumerable<NodeBlueprint> CreateBlueprintHierarchy()
        {
            //Find Root
            var root = nodes.Find(x => x is RootBlueprint) as RootBlueprint;

            var port = root.GetOutputPort("output");
            return root.GetConnectedNodes(port.GetConnections());
        }

        private Tree CreateTree(IEntityData data, IEnumerable<NodeBlueprint> blueprints)
        {
            return new Tree(data, blueprints);
        }
    }

    public class Tree
    {
        private Root _root;
        private IEntityData _data;

        public Tree(IEntityData data, IEnumerable<NodeBlueprint> blueprints)
        {
            _data = data;
            var nodes = CreateHierarchy(blueprints.ToList());
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
                    var branch = blueprints[i] as BranchBlueprint;
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