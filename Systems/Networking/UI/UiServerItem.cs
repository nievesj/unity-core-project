using System;
using Core.Services;
using Core.Systems.Network.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Systems.Network
{
    public class UiServerItem : CoreBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI displayName;

        [SerializeField]
        private TextMeshProUGUI address;

        [SerializeField]
        private TextMeshProUGUI maxPlayers;

        [SerializeField]
        private TextMeshProUGUI currentPlayers;

        [SerializeField]
        private Button joinButton;

        private Server _server;

        public event Action<Server> OnServerSelectedEvent;

        private void Awake()
        {
            joinButton.onClick.AddListener(OnJoinClicked);
        }

        public void SetItem(Server server)
        {
            _server = server;

            displayName.text = server.displayName;
            address.text = server.address;
            maxPlayers.text = server.maxplayerCount.ToString();
            currentPlayers.text = server.playerCount.ToString();
        }

        private void OnJoinClicked()
        {
            OnServerSelectedEvent?.Invoke(_server);
        }
    }
}