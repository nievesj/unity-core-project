using Core.Services;
using Core.Systems.Network.Managers;
using UnityEngine;
using Zenject;

namespace Core.Systems.Network.Components
{
    [DisallowMultipleComponent]
    public class NetworkIdentity : CoreBehaviour, INetworkIdentity
    {
        public ushort PeerId { get; set; }
        public ushort NetworkObjectId { get; set; }
        public bool IsHost => CoreNetworkManager.IsHost;
        public bool IsLocalEntity { get; set; }


        [Inject]
        protected CoreNetworkManager CoreNetworkManager;
    }
}