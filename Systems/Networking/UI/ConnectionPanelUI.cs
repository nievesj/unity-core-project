using Core.Common.Extensions.UnityComponent;
using Core.Services;
using Core.Systems.Network.Managers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Core.Systems.Network
{
    public class ConnectionPanelUI : CoreBehaviour
    {
        [SerializeField]
        private Button hostButton;

        [SerializeField]
        private Button clientButton;

        [Inject]
        private CoreNetworkManager _networkManager;

        private void Awake()
        {
            hostButton.onClick.AddListener(OnHostClicked);
            clientButton.onClick.AddListener(OnClientClicked);

            SetVisible(true);
        }

        private void OnHostClicked()
        {
            // _networkManager.StartHost();

            hostButton.SetActive(false);
            clientButton.SetActive(false);

            SetVisible(false);
        }

        private void OnClientClicked()
        {
            // _networkManager.StartClient();
        }

        private void OnServerClicked()
        {
            // _networkManager.StartServer();
            SetVisible(false);
        }

        private void OnDisconnectClicked()
        {
            // stop host if host mode
            // if (NetworkServer.active && NetworkClient.isConnected)
            // {
            //     _networkManager.StopHost();
            // }
            // // stop client if client-only
            // else if (NetworkClient.isConnected)
            // {
            //     _networkManager.StopClient();
            // }
            // // stop server if server-only
            // else if (NetworkServer.active)
            // {
            //     _networkManager.StopServer();
            // }

            SetVisible(true);
        }

        private void SetVisible(bool isVisible)
        {
            hostButton.SetActive(isVisible);
            clientButton.SetActive(isVisible);
        }
    }
}