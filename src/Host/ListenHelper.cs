using System;
using System.Net;
using System.Net.Sockets;
using KaisenLib;

namespace Host
{
    class ListenHelper
    {
        public IPAddress Address { get; private set; }
        public int Port { get; private set; }

        /// <summary>
        /// Listen可能なすべてのIPv4アドレス:19690を設定します。
        /// </summary>
        public ListenHelper()
        {
            Address = IPAddress.Any;
            Port = AppSet.defaultPort;
        }

        /// <summary>
        /// Listen可能なすべてのIPv4アドレス:listenPortを設定します。
        /// </summary>
        /// <param name="listenPort"></param>
        public ListenHelper(int listenPort)
        {
            Address = IPAddress.Any;
            this.Port = listenPort;
        }

        /// <summary>
        /// listenAddress:listenPortを設定します。
        /// </summary>
        /// <param name="listenAddress"></param>
        /// <param name="listenPort"></param>
        public ListenHelper(string listenAddress, int listenPort)
        {
            this.Address = IPAddress.Parse(listenAddress);
            this.Port = listenPort;
        }

        /// <summary>
        /// listenAddress:listenPortを設定します。
        /// </summary>
        /// <param name="listenAddress"></param>
        /// <param name="listenPort"></param>
        public ListenHelper(IPAddress listenAddress, int listenPort)
        {
            this.Address = listenAddress;
            this.Port = listenPort;
        }
    }
}
