using System;
using System.Collections.Generic;
using Core.Common.Extensions.String;
using Core.Networking.Clients;
using Core.Networking.Packets;
using Core.Systems.Network.Components;
using LiteNetLib;
using UniRx;
using UnityLogger;

namespace Core.Systems.Network.Managers
{
    public partial class CoreNetworkManager
    {
        private ushort _networkObjectIdentifierCounter;
        private Dictionary<ushort, NetworkIdentity> _serverInstantiatedObjects = new Dictionary<ushort, NetworkIdentity>();
        
        public void CreateHost(int port)
        {
            Server = new CoreServer(port, connectionInfo.secretKey);
            Server.RegisterNestedType<ConnectedPeer>();

            Server.OnPeerDisconnected()
                .Subscribe(OnPeerDisconnected)
                .AddTo(this);

            Server.OnReceivedSerializedComponent()
                .Subscribe(OnReceivedSerializedComponent)
                .AddTo(this);

            Server.RegisterToPacket<PlayerJoinRequestPacket, NetPeer>()
                .Subscribe(OnPlayerJoinRequestPacketReceived)
                .AddTo(this);

            Server.Start();

            _onHostCreated.OnNext(Unit.Default);
        }

        public IObservable<Unit> OnHostCreated()
        {
            return _onHostCreated;
        }

        private ushort GetNewNetworkObjectId()
        {
            _networkObjectIdentifierCounter++;
            return _networkObjectIdentifierCounter;
        }

        private void OnPlayerJoinRequestPacketReceived(CompositeData<PlayerJoinRequestPacket, NetPeer> data)
        {
            //Accept request. TODO: Check connection limits
            //Since this is the host, instantiate objects connected.
            Logger.Log($"[SERVER] OnPlayerJoinRequestPacketReceived {data.UserData.Id}".ColoredLog(Colors.Yellow));

            var networkObjectId = GetNewNetworkObjectId();
            var currentPeers = new List<ConnectedPeer>();
            foreach (var value in Server.ServerNetworkEntities.Values)
            {
                if (value.PeerId != data.UserData.Id)
                    currentPeers.Add(new ConnectedPeer {PeerId = value.PeerId, NetworkObjectId = value.NetworkObjectId});
            }

            var networkEntity = new NetworkEntity(data.UserData, networkObjectId);

            Server.ServerNetworkEntities.Add((ushort) data.UserData.Id, networkEntity);
            Server.SendPacket(data.UserData, new PlayerJoinRequestAcceptedPacket
            {
                PeerId = (ushort) data.UserData.Id,
                Tick = Server.ServerTick,
                NetworkObjectId = networkObjectId,
                CurrentPeers = currentPeers.ToArray()
            }, DeliveryMethod.ReliableOrdered);
            
            
            // Tell everyone that a new client just connected. TODO: improve this
            Server.SendPacketToEveryone(new NetworkEntityJoinedGamePacket
            {
                PeerId = (ushort) data.UserData.Id,
                NetworkObjectId = networkObjectId,
                Tick = Server.ServerTick
            }, DeliveryMethod.ReliableOrdered);
        }

        private void InstantiateNetworkObject(ushort peerId, ushort ownerPeerId, ushort networkObjectId, string prefabName)
        {
            Server.SendPacket(peerId, new InstantiateObjectPacket
            {
                NetworkObjectId = networkObjectId,
                OwnerPeerId = ownerPeerId,
                PeerId = peerId,
                PrefabName = prefabName
            }, DeliveryMethod.ReliableOrdered);
        }
    }
}