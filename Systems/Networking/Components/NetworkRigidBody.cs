using LiteNetLib.Utils;
using UnityEngine;

namespace Core.Systems.Network.Components
{
    [RequireComponent(typeof(NetworkTransform))]
    public class NetworkRigidBody : CoreNetworkBehavior
    {
        protected override void OnSerialize(NetDataWriter writer)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnDeserialize(NetDataReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}