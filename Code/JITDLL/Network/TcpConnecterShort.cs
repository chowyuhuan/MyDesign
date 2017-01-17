using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Net;

namespace Network
{
    /// <summary>
    /// Tcp短连接 
    /// </summary>
    public class TcpConnecterShort : Connecter
    {
        Socket _socket = null;
        NetworkStream _stream = null;

        public TcpConnecterShort(NetworkThread networkThread)
            : base(networkThread)
        {
        }

        public override void Close()
        {
            if (_socket != null)
            {
                _socket.Close();

                _socket = null;
                _stream = null;
            }
        }

        public override void Connect()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

#if NETWORK_LOG
            _networkThread.AddLog("[网络] Connect Host " + _url.Host + " Port " + _url.Port);
#endif

            System.Threading.ManualResetEvent mre = new System.Threading.ManualResetEvent(false);
            IAsyncResult result = _socket.BeginConnect(_url.Host, _url.Port, (ac) => { mre.Set(); }, null);
            bool active = mre.WaitOne(_timeout);
            if (active)
            {
                _socket.EndConnect(result);

                _stream = new NetworkStream(_socket);
            }
            else
            {
                throw new Exception("Connection timed out!");
            }
        }

        public override void Send(byte[] contents)
        {
            System.Threading.ManualResetEvent mre = new System.Threading.ManualResetEvent(false);
            IAsyncResult result = _stream.BeginWrite(contents, 0, contents.Length, (ac) => { mre.Set(); }, null);
            bool active = mre.WaitOne(_timeout);
            if (active)
            {
                _stream.EndWrite(result);
            }
            else
            {
                throw new Exception("Send timed out!");
            }
        }

        public override void Receive()
        {
            NetworkRspData networkRspData = null;
            networkRspData = ReadProtocolData(_stream, _timeout);
            networkRspData.request = _request;

            _networkThread.PutResponseData(networkRspData);
        }
    }
}
