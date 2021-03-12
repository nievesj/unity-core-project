using System;
using System.Collections.Generic;
using Core.Services;
using Core.Systems.Network.Models;
using Core.Tools.Networking;
using UnityEngine;
using UniRx;

namespace Core.Systems.Network
{
    [Serializable]
    public struct PeerPayload
    {
        public string ServerId;
        public Peer Peer;
    }
    
    public class MasterServerApi : CoreBehaviour
    {
        [SerializeField]
        private string apiUrlProd;

        [SerializeField]
        private string apiUrlDev;

        [SerializeField]
        private bool useDevApi = false;

        [SerializeField]
        private string apiKey; //TODO

        [SerializeField]
        private ushort gameServerConnectionPort = 9000;

        private string _localIpAddress;
        private string _publicIpAddress;

        public string LocalIpAddress => _localIpAddress;
        public string PublicIpAddress => _publicIpAddress;
        public ushort GameServerConnectionPort => gameServerConnectionPort;
        public string ActiveApiUrl => useDevApi ? apiUrlDev : apiUrlProd;

        private async void Awake()
        {
            _publicIpAddress = await NetworkUtility.GetPublicIpAsync();
        }

        public IObservable<ServerPayload> GetServers()
        {
            return RestRequest.Request<ServerPayload>(RequestType.Get, $"{ActiveApiUrl}/servers");
        }

        public IObservable<Server> RegisterServer(Server server)
        {
            var json = JsonUtility.ToJson(server);
            return RestRequest.Request<Server>(RequestType.Put, $"{ActiveApiUrl}/register", json);
        }
        
        public IObservable<Server> UnRegisterServer(Server server)
        {
            var json = JsonUtility.ToJson(server);
            return RestRequest.Request<Server>(RequestType.Put, $"{ActiveApiUrl}/unregister", json);
        }
        
        public IObservable<string> AddPeer(PeerPayload payload)
        {
            var json = JsonUtility.ToJson(payload);
            return RestRequest.Request<string>(RequestType.Put, $"{ActiveApiUrl}/addpeer", json);
        }
        
        public IObservable<PeerPayload> RemovePeer(PeerPayload payload)
        {
            var json = JsonUtility.ToJson(payload);
            return RestRequest.Request<PeerPayload>(RequestType.Put, $"{ActiveApiUrl}/removepeer", json);
        }
        
        public IObservable<PeerPayload> KickPeer(PeerPayload payload)
        {
            var json = JsonUtility.ToJson(payload);
            return RestRequest.Request<PeerPayload>(RequestType.Put, $"{ActiveApiUrl}/kickpeer", json);
        }
        
        public IObservable<PeerPayload> BanPeer(PeerPayload payload)
        {
            var json = JsonUtility.ToJson(payload);
            return RestRequest.Request<PeerPayload>(RequestType.Put, $"{ActiveApiUrl}/banpeer", json);
        }
    }
}