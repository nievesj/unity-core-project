using LiteNetLib.Utils;

namespace Core.Networking.Packets
{
    public struct ConnectedPeer : INetSerializable
    {
        public ushort PeerId;
        public ushort NetworkObjectId;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PeerId);
            writer.Put(NetworkObjectId);
        }

        public void Deserialize(NetDataReader reader)
        {
            PeerId = reader.GetUShort();
            NetworkObjectId = reader.GetUShort();
        }
    }
}