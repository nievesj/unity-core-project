using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;

namespace Core.Tools.Networking
{
    public static class NetworkUtility
    {
        public enum AddressType
        {
            IPv4,
            IPv6
        }

        public static string GetIP(AddressType addressType)
        {
            var ret = "";
            var IPs = GetAllIPs(addressType, false);
            if (IPs.Count > 0)
            {
                ret = IPs[IPs.Count - 1];
            }

            return ret;
        }

        public static List<string> GetAllIPs(AddressType addressType, bool includeDetails)
        {
            //Return null if AddressType is Ipv6 but Os does not support it
            if (addressType == AddressType.IPv6 && !Socket.OSSupportsIPv6)
            {
                return null;
            }

            var output = new List<string>();

            foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS
                var _type1 = NetworkInterfaceType.Wireless80211;
                var _type2 = NetworkInterfaceType.Ethernet;

                var isCandidate = (item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2);

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            // as of MacOS (10.13) and iOS (12.1), OperationalStatus seems to be always "Unknown".
            isCandidate = isCandidate && item.OperationalStatus == OperationalStatus.Up;
#endif

                if (isCandidate)
#endif
                {
                    foreach (var ip in item.GetIPProperties().UnicastAddresses)
                    {
                        //IPv4
                        if (addressType == AddressType.IPv4)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                var s = ip.Address.ToString();
                                if (includeDetails)
                                    s += "  " + item.Description.PadLeft(6) + item.NetworkInterfaceType.ToString().PadLeft(10);

                                output.Add(s);
                            }
                        }

                        //IPv6
                        else if (addressType == AddressType.IPv6)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                                output.Add(ip.Address.ToString());
                        }
                    }
                }
            }

            return output;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }

        public static async UniTask<string> GetPublicIpAsync()
        {
            var externalIp = await new WebClient().DownloadStringTaskAsync("http://icanhazip.com");
            var cleaned = externalIp.Replace("\n", "").Replace("\r", "");

            return cleaned;
        }
    }
}