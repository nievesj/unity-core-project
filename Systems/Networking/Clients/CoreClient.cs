using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Core.Common.Extensions.String;
using Core.Common.Extensions.UnityComponent;
using Core.Networking.Packets;
using Core.Systems.Network;
using Core.Systems.Network.Components;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityLogger;

namespace Core.Networking.Clients
{
    public class CoreClient : UdpClient
    {
        public NetPeer Peer { get; set; }
        public NetPeer Server { get; private set; }
        private string _address;

        private Dictionary<ushort, NetworkIdentity> _clientNetworkEntities = new Dictionary<ushort, NetworkIdentity>();
        public Dictionary<ushort, NetworkIdentity> ConnectedPeers => _clientNetworkEntities;

        public CoreClient(string address, int port, string key, int eventPoolingFrequencyMilliseconds = 20) : base(port, key, eventPoolingFrequencyMilliseconds)
        {
            _address = address;
        }

        public override void Start()
        {
            NetManager = new NetManager(this);
            NetManager.DisconnectTimeout = 5000;

            NetManager.Start();
            Peer = NetManager.Connect(_address, Port, Key);

            base.Start();
        }

        public void SyncTick(uint serverTick)
        {
            Tick = serverTick;
        }

        public void RemovePeer(ushort peerId)
        {
            if (_clientNetworkEntities.ContainsKey(peerId))
            {
                var peer = _clientNetworkEntities[peerId];
                _clientNetworkEntities.Remove(peerId);
                peer.gameObject.Destroy();
            }
        }

        public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod, PacketType packetType = PacketType.Serialized) where T : class, new()
        {
            if (Server != null)
            {
                NetDataWriter.Reset();
                NetDataWriter.Put((byte) packetType);
                NetPacketProcessor.Write(NetDataWriter, packet);

                Server.Send(NetDataWriter, deliveryMethod);
                // Logger.Log($"[CLIENT] SendPacket sending {NetDataWriter.Length} bytes for {packet}".ColoredLog(Colors.Yellow));
            }
        }

        public void SendSerializedPacket<T>(T packet, DeliveryMethod deliveryMethod, PacketType packetType = PacketType.Serialized) where T : struct, INetSerializable
        {
            if (Server != null)
            {
                NetDataWriter.Reset();
                NetDataWriter.Put((byte) packetType);
                packet.Serialize(NetDataWriter);

                Server.Send(NetDataWriter, deliveryMethod);
                Logger.Log($"[CLIENT] SendSerializedPacket sending {NetDataWriter.Length} bytes for {packet}".ColoredLog(Colors.Yellow));
            }
        }

        public void SendRaw(byte[] data, DeliveryMethod deliveryMethod)
        {
            if (Server != null)
            {
                Server.Send(data, deliveryMethod);
            }
        }

        public virtual void SendSerializedComponent(ISerializableComponent comp, DeliveryMethod deliveryMethod)
        {
            NetDataWriter.Reset();
            NetDataWriter.Put((byte) PacketType.SerializedComponent);
            comp.Serialize(NetDataWriter);

            SendRaw(NetDataWriter.Data, deliveryMethod);
        }

        public override void OnPeerConnected(NetPeer server)
        {
            Server = server;
            Logger.Log($"[CLIENT] We connected to server: {server.EndPoint}".ColoredLog(Colors.HotPink));

            //Connected, send join request 
            SendPacket(new PlayerJoinRequestPacket {PeerId = (ushort) server.Id, Tick = Tick}, DeliveryMethod.ReliableOrdered);
        }

        public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Logger.Log("[CLIENT] We disconnected because " + disconnectInfo.Reason);
        }

        public override void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Logger.Log("[CLIENT] We received error " + socketError);
        }

        public override void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            // if (messageType == UnconnectedMessageType.BasicMessage && NetManager.ConnectedPeersCount == 0 && reader.GetInt() == 1)
            // {
            //     Logger.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
            //     NetManager.Connect(remoteEndPoint, Key);
            // }
        }

        public override void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            // Logger.Log($"Client OnNetworkLatencyUpdate {peer.Id} {latency} ");
        }

        public override void OnConnectionRequest(ConnectionRequest request)
        {
            request.Reject();
        }
    }
}