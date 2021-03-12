using System;
using System.Collections.Generic;
using Core.Services;
using Core.Systems.Network.Models;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Systems.Network
{
    public class ServerListPanel : CoreBehaviour
    {
        [SerializeField]
        private UiServerItem listServerItemPrefab;

        [SerializeField]
        private RectTransform serverListScrollViewContent;

        [SerializeField]
        private Button hostButton;

        [SerializeField]
        private RectTransform serverListRect;

        [SerializeField]
        private RectTransform noServerRect;

        public event Action<Server> OnJoinRequestedEvent;
        public event Action<Unit> OnHostRequestedEvent;

        private void Awake()
        {
            hostButton.onClick.AddListener(OnHostClicked);
        }

        public void SetServers(List<Server> servers)
        {
            foreach (var server in servers)
            {
                var item = Instantiate(listServerItemPrefab, serverListScrollViewContent);
                item.SetItem(server);

                Observable.FromEvent<Server>(
                        h => item.OnServerSelectedEvent += h,
                        h => item.OnServerSelectedEvent -= h)
                    .Subscribe(OnServerSelectedClicked)
                    .AddTo(this);
            }
        }
        
        private void OnServerSelectedClicked(Server server)
        {
            Debug.Log($"Selected {server.displayName} {server.ip}");
            OnJoinRequestedEvent?.Invoke(server);
        }

        private void OnHostClicked()
        {
            OnHostRequestedEvent?.Invoke(Unit.Default);
        }
    }
}