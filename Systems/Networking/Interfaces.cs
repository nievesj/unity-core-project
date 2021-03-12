using LiteNetLib;
using LiteNetLib.Utils;

namespace Core.Systems.Network
{
    public interface INetworkIdentity
    {
        ushort PeerId { get; }
        ushort NetworkObjectId { get; }
        bool IsLocalEntity { get; }
        bool IsHost { get; }
    }

    public interface ISerializableComponent : INetworkIdentity, INetSerializable
    {
        uint NetworkTick { get;}
    }
}