namespace Core.Networking.Packets
{
    public interface IPacket
    {
        ushort PeerId { get; }
        uint Tick { get; }
    }

    public abstract class Packet : IPacket
    {
        public ushort PeerId { get; set; }
        public uint Tick { get; set; }
    }

    public class PlayerJoinRequestPacket : Packet { }

    public class PlayerJoinRequestAcceptedPacket : Packet
    {
        public ushort NetworkObjectId { get; set; }

        public ConnectedPeer[] CurrentPeers { get; set; }
    }

    public class NetworkEntityJoinedGamePacket : Packet
    {
        public ushort NetworkObjectId { get; set; }
    }

    public class LoadScenePacket : Packet
    {
        public string SceneName { get; set; }
    }

    public class InstantiateObjectPacket : Packet
    {
        public string PrefabName { get; set; }
        public ushort NetworkObjectId { get; set; }
        public ushort OwnerPeerId { get; set; }
    }

    public class DestroyObjectPacket : Packet
    {
        public ushort NetworkObjectId { get; set; }
    }
}