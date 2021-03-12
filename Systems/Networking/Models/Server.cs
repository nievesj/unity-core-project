using System;
using System.Collections.Generic;

namespace Core.Systems.Network.Models
{
    [Serializable]
    public struct ServerPayload
    {
        public List<Server> servers;
    }
    
    [Serializable]
    public class Server
    {
        public string id;
        public string ip;
        public string address;
        public int port;
        public Peer host;//needed?
        public string protocol;
        public int playerCount;
        public int maxplayerCount;
        public string displayName;
        public string serverKey; //encrypted?
        public string gameKey; //encrypted?

        private Dictionary<string, Peer> _peers;
        
        public Server()
        {
            _peers = new Dictionary<string, Peer>();
        }

        public void AddPeer(Peer peer)
        {
            if (!_peers.ContainsKey(peer.id))
                _peers.Add(peer.id, peer);
        }

        public void RemovePeer(Peer peer)
        {
            if (_peers.ContainsKey(peer.id))
                _peers.Remove(peer.id);
        }

        public void KickPeer(Peer peer) { }

        public void BanPeer(Peer peer) { }
    }
}