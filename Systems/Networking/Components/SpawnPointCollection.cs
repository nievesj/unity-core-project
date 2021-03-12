using Core.Services;
using UnityEngine;

namespace Core.Systems.Network.Components
{
    public class SpawnPointCollection : CoreBehaviour
    {
        private Transform[] Children => GetComponentsInChildren<Transform>();
        public Transform point;

        public Transform GetRandomSpawnTransform()
        {
            // return Children.GetRandomElement();
            return point;
        }
    }
}