using System;
using Core.Services;
using Core.Systems.Network.Managers;
using Core.Systems.Network.Models;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Systems.Network
{
    public class MasterListServerUI : CoreBehaviour
    {
        [SerializeField]
        private ServerListPanel serverListPanel;

        private Server _serverData;

        [Inject]
        private CoreNetworkManager _networkManager;
        
        private void Start()
        {
            _networkManager.MasterServerApi.GetServers()
                .Subscribe(payload => { serverListPanel.SetServers(payload.servers); });

            Observable.FromEvent<Unit>(
                    h => serverListPanel.OnHostRequestedEvent += h,
                    h => serverListPanel.OnHostRequestedEvent -= h)
                .Subscribe(_ =>
                {
                    _networkManager.CreateHost();
                    OnServerCreated();
                    gameObject.SetActive(false);
                })
                .AddTo(this);

            Observable.FromEvent<Server>(
                    h => serverListPanel.OnJoinRequestedEvent += h,
                    h => serverListPanel.OnJoinRequestedEvent -= h)
                .Subscribe(server =>
                {
                    _networkManager.Connect(server.ip);
                    gameObject.SetActive(false);
                })
                .AddTo(this);
        }

        private async void OnServerCreated()
        {
            var protocol = "udp";
            _serverData = new Server();

            _serverData.id = Guid.NewGuid().ToString();
            _serverData.displayName = $"Awesome Game {(UnityEngine.Random.value * 1000).ToString("0")}";
            _serverData.protocol = protocol;
            _serverData.port = _networkManager.Port;
            _serverData.maxplayerCount = 4;
            _serverData.playerCount = 0;

            _networkManager.MasterServerApi.RegisterServer(_serverData).Subscribe(val => { _serverData.id = val.id; });

            await UniTask.Delay(TimeSpan.FromSeconds(.1f));

            _networkManager.Connect("localhost");
        }

        protected override void OnDestroy()
        {
            if(_serverData!= null)
             _networkManager.MasterServerApi.UnRegisterServer(_serverData).Subscribe(val => { _serverData.id = val.id; });
            base.OnDestroy();
        }
    }
}