using System;
using System.Collections.Generic;
using Core.Common.Extensions.Hashing;
using Core.Common.Extensions.String;
using Core.Networking.Clients;
using Core.Networking.Packets;
using Core.Systems.Network.Components;
using LiteNetLib.Utils;
using UniRx;
using UnityEngine;
using Logger = UnityLogger.Logger;

namespace Core.Systems.Network.Managers
{
    public struct SerializedComponent : INetSerializable
    {
        public ulong Hash;
        public ushort PeerId;
        public ushort NetworkObjectId;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Hash);
            writer.Put(PeerId);
            writer.Put(NetworkObjectId);
        }

        public void Deserialize(NetDataReader reader)
        {
            Hash = reader.GetULong();
            PeerId = reader.GetUShort();
            NetworkObjectId = reader.GetUShort();
        }
    }

    public partial class CoreNetworkManager
    {
        //Holds all INetSerializable references for all networked objects.
        private readonly Dictionary<ushort, Dictionary<ulong, INetSerializable>> _serializableComponents = new Dictionary<ushort, Dictionary<ulong, INetSerializable>>();

        //Cached SerializedComponent
        private SerializedComponent _serializedComponent;

        public void Connect(string address)
        {
            Client = new CoreClient(address, connectionInfo.port, connectionInfo.secretKey);
            Client.RegisterNestedType<ConnectedPeer>();

            Client.RegisterToPacket<NetworkEntityJoinedGamePacket>()
                .Subscribe(OnNetworkEntityJoined)
                .AddTo(this);

            Client.RegisterToPacket<PlayerJoinRequestAcceptedPacket>()
                .Subscribe(OnPlayerJoinRequestAccepted)
                .AddTo(this);

            Client.RegisterToPacket<LoadScenePacket>()
                .Subscribe(OnLoadSceneRequest)
                .AddTo(this);

            Client.RegisterToPacket<InstantiateObjectPacket>()
                .Subscribe(OnInstantiateObjectRequest)
                .AddTo(this);

            Client.RegisterToPacket<DestroyObjectPacket>()
                .Subscribe(OnDestroyObjectRequest)
                .AddTo(this);

            Client.OnReceivedSerializedComponent()
                .Subscribe(OnReceivedSerializedComponent)
                .AddTo(this);

            Client.Start();
        }

        public IObservable<Unit> OnPlayerJoined()
        {
            return _onPlayerJoined;
        }

        public IObservable<Unit> OnPlayerLeft()
        {
            return _onPlayerLeft;
        }

        public void RegisterNetworkObject(CoreNetworkBehavior networkBehavior)
        {
            if (_serializableComponents.ContainsKey(networkBehavior.NetworkObjectId)) return;

            var identity = networkBehavior.GetComponent<NetworkIdentity>();
            GetSerializableComponents(identity);
        }

        public void UnRegisterNetworkObject(CoreNetworkBehavior networkBehavior)
        {
            if (_serializableComponents.ContainsKey(networkBehavior.NetworkObjectId))
                _serializableComponents.Remove(networkBehavior.NetworkObjectId);
        }

        private void CreateEntity(ushort peerId, ushort networkObjectId, bool isLocal = false)
        {
            if (!Client.ConnectedPeers.ContainsKey(peerId))
            {
                var client = Instantiate(playerPrefab);
                var networkIdentity = client.GetComponent<NetworkIdentity>();
                networkIdentity.PeerId = peerId;
                networkIdentity.IsLocalEntity = isLocal;
                networkIdentity.NetworkObjectId = networkObjectId;
                client.name = peerId.ToString();
                Client.ConnectedPeers.Add(peerId, networkIdentity);

                GetSerializableComponents(networkIdentity);
            }
        }

        private void GetSerializableComponents(NetworkIdentity networkIdentity)
        {
            if (_serializableComponents.ContainsKey(networkIdentity.NetworkObjectId)) return;

            var components = networkIdentity.GetComponents<INetSerializable>();
            var dict = new Dictionary<ulong, INetSerializable>();

            foreach (var component in components)
                dict.Add(((Component) component).GetTypeHash(), component);

            _serializableComponents.Add(networkIdentity.NetworkObjectId, dict);
        }

        private void OnReceivedSerializedComponent(ComponentData data)
        {
            _serializedComponent.Deserialize(data.Reader);
            if (_serializableComponents.ContainsKey(_serializedComponent.NetworkObjectId))
            {
                var components = _serializableComponents[_serializedComponent.NetworkObjectId];
                if (components.ContainsKey(_serializedComponent.Hash))
                {
                    components[_serializedComponent.Hash].Deserialize(data.Reader);
                }
            }
        }

        private void OnLoadSceneRequest(LoadScenePacket loadScenePacket) { }
        
        private void OnInstantiateObjectRequest(InstantiateObjectPacket instantiateObjectPacket)
        {
            var instance = _factoryCore.Instantiate(cubePrefab);
            var identity = instance.GetComponent<NetworkIdentity>();
            identity.PeerId = instantiateObjectPacket.OwnerPeerId;
            identity.IsLocalEntity = IsHost;
            identity.NetworkObjectId = instantiateObjectPacket.NetworkObjectId;
            RegisterNetworkObject(instance);

            if (IsHost)
                _serverInstantiatedObjects.Add(identity.NetworkObjectId, identity);

            instance.OnDestroyed()
                .Subscribe(_ =>
                {
                    UnRegisterNetworkObject(instance);

                    if (IsHost)
                        _serverInstantiatedObjects.Remove(identity.NetworkObjectId);
                })
                .AddTo(instance);
        }

        private void OnDestroyObjectRequest(DestroyObjectPacket destroyObjectPacket) { }

        private void OnPlayerJoinRequestAccepted(PlayerJoinRequestAcceptedPacket packet)
        {
            CreateEntity(packet.PeerId, packet.NetworkObjectId, true);
            PeerId = packet.PeerId;
            Client.SyncTick(packet.Tick);
            foreach (var peer in packet.CurrentPeers)
            {
                if (!Client.ConnectedPeers.ContainsKey(peer.PeerId))
                    CreateEntity(peer.PeerId, peer.NetworkObjectId);
            }

            if (IsHost)
            {
                InstantiateNetworkObject(PeerId, PeerId, GetNewNetworkObjectId(), "");
            }

            Logger.Log($"[CLIENT] We joined. Id: {packet.PeerId} Existing players: {packet.CurrentPeers.Length}".ColoredLog(Colors.HotPink));
        }

        private void OnPeerDisconnected(ushort peerId)
        {
            Client.RemovePeer(peerId);

            if (_serializableComponents.ContainsKey(peerId))
                _serializableComponents.Remove(peerId);

            _onPlayerLeft.OnNext(Unit.Default);
            //Remove all network objects.
        }

        private void OnNetworkEntityJoined(NetworkEntityJoinedGamePacket packet)
        {
            CreateEntity(packet.PeerId, packet.NetworkObjectId);

            //Send network objects
            if (IsHost)
            {
                foreach (var key in _serverInstantiatedObjects.Keys)
                {
                    InstantiateNetworkObject(packet.PeerId, _serverInstantiatedObjects[key].PeerId, _serverInstantiatedObjects[key].NetworkObjectId, "");
                }
            }

            _onPlayerJoined.OnNext(Unit.Default);
            Logger.Log($"[CLIENT] Player Joined!! {packet.PeerId}".ColoredLog(Colors.HotPink));
        }
    }
}