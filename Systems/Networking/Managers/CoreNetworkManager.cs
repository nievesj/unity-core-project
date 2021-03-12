using System;
using Core.Networking.Clients;
using Core.Systems.Network.Components;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Systems.Network.Managers
{
    [Serializable]
    public struct ConnectionInfo
    {
        public int port;
        public string secretKey;
    }

    public partial class CoreNetworkManager : SystemBehaviour
    {
        [SerializeField]
        private ConnectionInfo connectionInfo;

        [SerializeField]
        private GameObject playerPrefab;
        
        [SerializeField]
        private Cube cubePrefab;

        [Inject]
        private FactoryCoreSystem _factoryCore;

        public ushort PeerId { get; private set; }
        public bool IsHost => Server != null;
        public CoreServer Server { get; private set; }
        public CoreClient Client { get; private set; }
        public MasterServerApi MasterServerApi { get; private set; }

        public int Port => connectionInfo.port;

        private Subject<Unit> _onHostCreated = new Subject<Unit>();
        private Subject<Unit> _onPlayerJoined = new Subject<Unit>();
        private Subject<Unit> _onPlayerLeft = new Subject<Unit>();

        protected override void Awake()
        {
            base.Awake();

            MasterServerApi = GetComponent<MasterServerApi>();
        }
        
        public override void Initialize()
        {
            _onCoreSystemInitialized.OnNext(Unit.Default);
            _onCoreSystemInitialized.OnCompleted();

            DontDestroyOnLoad(gameObject);
        }

        public void CreateHost()
        {
            CreateHost(connectionInfo.port);
        }

        private void OnDestroy()
        {
            Server?.Dispose();
            Client?.Dispose();
        }

        public override void Dispose()
        {
            //why is this not being called????
            Server?.Dispose();
            Client?.Dispose();
        }
    }
}