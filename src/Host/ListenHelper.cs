using System;
using System.Net;
using System.Net.Sockets;
using KaisenLib;

namespace Host
{
    class ListenHelper
    {
        private IPAddress listenAddress;
        private int listenPort;
        public TcpListener Listener { get; private set; }

        /// <summary>
        /// Listen可能なすべてのIPv4アドレス:19690でListen開始します。
        /// </summary>
        public ListenHelper()
        {
            listenAddress = IPAddress.Any;
            listenPort = AppSet.defaultPort;
            StartListen();
        }

        /// <summary>
        /// Listen可能なすべてのIPv4アドレス:listenPortでListen開始します。
        /// </summary>
        /// <param name="listenPort"></param>
        public ListenHelper(int listenPort)
        {
            listenAddress = IPAddress.Any;
            this.listenPort = listenPort;
            StartListen();
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
            StartListen();
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
            StartListen();
        }

        private void StartListen()
        {
            try
            {
                Listener = new TcpListener(listenAddress, listenPort);
                Listener.Start();
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
