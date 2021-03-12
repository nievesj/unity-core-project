using Core.Common.Extensions.Hashing;
using Core.Services;
using Core.Systems.Network.Components;
using Core.Systems.Network.Managers;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using Zenject;

namespace Core.Systems.Network
{
    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(NetworkTransform))]
    public abstract class CoreNetworkBehavior : CoreBehaviour, ISerializableComponent
    {
        public ushort PeerId => NetworkIdentity.PeerId;
        public ushort NetworkObjectId => NetworkIdentity.NetworkObjectId;
        public bool IsLocalEntity => NetworkIdentity.IsLocalEntity;
        public bool IsHost => NetworkIdentity.IsHost;
        public uint NetworkTick { get; protected set; }

        protected NetworkIdentity NetworkIdentity;
        private SerializedComponent _serializedComponent;

        [Inject]
        protected CoreNetworkManager CoreNetworkManager;

        protected virtual void Awake()
        {
            NetworkIdentity = GetComponent<NetworkIdentity>();
        }

        protected virtual void Serialize(bool isClient = true, DeliveryMethod deliveryMethod = DeliveryMethod.Unreliable)
        {
            if (isClient)
            {
                CoreNetworkManager.Client.SendSerializedComponent(this, deliveryMethod);
            }
            else if (IsHost)
            {
                CoreNetworkManager.Server.SendSerializedComponent(this, deliveryMethod);
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            _serializedComponent.Hash = this.GetTypeHash();
            _serializedComponent.PeerId = PeerId;
            _serializedComponent.NetworkObjectId = NetworkObjectId;
            _serializedComponent.Serialize(writer);

            OnSerialize(writer);
        }

        public void Deserialize(NetDataReader reader)
        {
            OnDeserialize(reader);
        }

        protected abstract void OnSerialize(NetDataWriter writer);
        protected abstract void OnDeserialize(NetDataReader reader);
    }
}