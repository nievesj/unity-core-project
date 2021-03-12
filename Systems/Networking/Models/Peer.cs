using System;

namespace Core.Systems.Network.Models
{
    [Serializable]
    public class Peer
    {
        public string id;
        public string ip;
        public int port;
        public string name;
        public string gamekey; //encrypted?
    }
}