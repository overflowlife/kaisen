using System;
using System.Net;
using System.Net.Sockets;
using KaisenLib;

namespace Host
{
    class ListenHelper
    {
        public IPAddress listenAddress { get; private set; }
        public int listenPort { get; private set; }

        /// <summary>
        /// Listen可能なすべてのIPv4アドレス:19690でListen開始します。
        /// </summary>
        public ListenHelper()
        {
            listenAddress = IPAddress.Any;
            listenPort = AppSet.defaultPort;
        }

        /// <summary>
        /// Listen可能なすべてのIPv4アドレス:listenPortでListen開始します。
        /// </summary>
        /// <param name="listenPort"></param>
        public ListenHelper(int listenPort)
        {
            listenAddress = IPAddress.Any;
            this.listenPort = listenPort;
        }

        /// <summary>
        /// listenAddress:listenPortでListen開始します。
        /// </summary>
        /// <param name="listenAddress"></param>
        /// <param name="listenPort"></param>
        public ListenHelper(string listenAddress, int listenPort)
        {
            this.listenAddress = IPAddress.Parse(listenAddress);
            this.listenPort = listenPort;
        }

        /// <summary>
        /// listenAddress:listenPortでListen開始します。
        /// </summary>
        /// <param name="listenAddress"></param>
        /// <param name="listenPort"></param>
        public ListenHelper(IPAddress listenAddress, int listenPort)
        {
            this.listenAddress = listenAddress;
            this.listenPort = listenPort;
        }
    }
}
