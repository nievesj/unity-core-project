using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Core.Common.Extensions.String;
using Core.Systems.Network;
using LiteNetLib;
using LiteNetLib.Utils;
using UniRx;
using UnityEngine;
using Logger = UnityLogger.Logger;

namespace Core.Networking.Clients
{
    /// <summary>
    /// This is used on CoreServer to hold all connected entities and the NetPeer
    /// Struct is created before any GameObject is instantiated.
    /// </summary>
    public struct NetworkEntity
    {
        public NetPeer Peer { get; private set; }
        public ushort PeerId => (ushort) Peer.Id;
        public ushort NetworkObjectId { get; private set; }

        public NetworkEntity(NetPeer peer, ushort networkObjectId)
        {
            Peer = peer;
            NetworkObjectId = networkObjectId;
        }
    }

    public class CoreServer : UdpClient, INetLogger
    {
        public Dictionary<ushort, NetworkEntity> ServerNetworkEntities = new Dictionary<ushort, NetworkEntity>();

        public CompositeDisposable _subscriptions;
        private Subject<ushort> _onPeerDisconnected = new Subject<ushort>();

        public uint ServerTick => Tick;

        public CoreServer(int port, string key, int eventPoolingFrequencyMilliseconds = 20) : base(port, key, eventPoolingFrequencyMilliseconds)
        {
            _subscriptions = new CompositeDisposable();
        }

        public override void Start()
        {
            NetManager = new NetManager(this);
            NetManager.DisconnectTimeout = 5000;
            NetManager.Start(Port);

            base.Start();
        }

        public void SendPacket<T>(NetPeer peer, T packet, DeliveryMethod deliveryMethod, PacketType packetType = PacketType.Serialized) where T : class, new()
        {
            NetDataWriter.Reset();
            NetDataWriter.Put((byte) packetType);
            NetPacketProcessor.Write(NetDataWriter, packet);

            // Logger.Log($"[SERVER] SendPacket sending {peer.Id} {NetDataWriter.Length} bytes for {packet}".ColoredLog(Colors.Yellow));
            peer.Send(NetDataWriter, deliveryMethod);
        }

        public void SendPacket<T>(ushort peerId, T packet, DeliveryMethod deliveryMethod, PacketType packetType = PacketType.Serialized) where T : class, new()
        {
            if (!ServerNetworkEntities.ContainsKey(peerId)) return;

            var peer = ServerNetworkEntities[peerId];

            SendPacket<T>(peer.Peer, packet, deliveryMethod, packetType);
        }

        public void SendSerializedPacket<T>(NetPeer peer, T packet, DeliveryMethod deliveryMethod, PacketType packetType = PacketType.Serialized) where T : struct, INetSerializable
        {
            NetDataWriter.Reset();
            NetDataWriter.Put((byte) packetType);
            packet.Serialize(NetDataWriter);

            peer.Send(NetDataWriter, deliveryMethod);
            // Logger.Log($"[SERVER] SendSerializedPacket sending {NetDataWriter.Length} bytes for {packet}".ColoredLog(Colors.Yellow));
        }

        public void SendPacketToEveryone<T>(T packet, DeliveryMethod deliveryMethod, PacketType packetType = PacketType.Serialized) where T : class, new()
        {
            NetDataWriter.Reset();
            NetDataWriter.Put((byte) packetType);
            NetPacketProcessor.Write(NetDataWriter, packet);

            NetManager?.SendToAll(NetDataWriter, deliveryMethod);
            // Logger.Log($"[SERVER] SendPacketToEveryone sending {NetDataWriter.Length} bytes for {packet}".ColoredLog(Colors.Yellow));
        }

        public void SendRaw(NetPeer peer, byte[] data, DeliveryMethod deliveryMethod)
        {
            peer.Send(data, deliveryMethod);
            // Logger.Log($"[CLIENT] SendSerializedPacket sending {data.Length} bytes".ColoredLog(Colors.Yellow));
        }

        public virtual void SendSerializedComponent(ISerializableComponent comp, DeliveryMethod deliveryMethod)
        {
            NetDataWriter.Reset();
            NetDataWriter.Put((byte) PacketType.SerializedComponent);
            comp.Serialize(NetDataWriter);

            foreach (var key in ServerNetworkEntities.Keys)
                SendRaw(ServerNetworkEntities[key].Peer, NetDataWriter.Data, deliveryMethod);
        }

        public void SendUnconnectedMessage(NetDataWriter resp, IPEndPoint remoteEndPoint)
        {
            Debug.Log($"SendUnconnectedMessage ");
            NetManager.SendUnconnectedMessage(resp, remoteEndPoint);
        }

        public void WriteNet(NetLogLevel level, string str, params object[] args)
        {
            Debug.LogFormat(str, args);
        }

        public IObservable<ushort> OnPeerDisconnected()
        {
            return _onPeerDisconnected;
        }

        public override void OnPeerConnected(NetPeer peer)
        {
            Logger.Log($"[SERVER] Client connected: {peer.Id}".ColoredLog(Colors.Yellow));
        }

        public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (!ServerNetworkEntities.ContainsKey((ushort) peer.Id)) return;

            var entity = ServerNetworkEntities[(ushort) peer.Id];
            ServerNetworkEntities.Remove(entity.PeerId);
            _onPeerDisconnected.OnNext(entity.PeerId);
            Logger.Log($"[SERVER] Peer {entity.PeerId} disconnected because {disconnectInfo.Reason}".ColoredLog(Colors.Yellow));
        }

        public override void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Logger.Log($"[SERVER] received error {socketError}".ColoredLog(Colors.Yellow));
        }

        public override void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.Broadcast)
            {
                Logger.Log("[SERVER] Received discovery request. Send discovery response".ColoredLog(Colors.Yellow));
                var resp = new NetDataWriter();
                resp.Put(1);
                SendUnconnectedMessage(resp, remoteEndPoint);
            }
        }

        public override void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            // Debug.Log($"OnNetworkLatencyUpdate {data.Peer.Id} {data.Latency} ");
        }

        public override void OnConnectionRequest(ConnectionRequest request)
        {
            request.Accept();
        }
        
        public override void Dispose()
        {
            _subscriptions.Dispose();
            base.Dispose();
        }
    }
}