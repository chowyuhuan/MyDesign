using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Net;

namespace Network
{
    public class TcpConnecterPersistent : Connecter
    {
        Socket _socket = null;
        NetworkStream _stream = null;

        byte[] _buffer;
        int _totalBytes;
        int _currentBytes;

        ProtocolHeader _header;
        NetworkRspData _networkRspData;

        public TcpConnecterPersistent(NetworkThread networkThread)
            : base(networkThread)
        {
        }

        public override void SetUrl(string url)
        {
            base.SetUrl(url);

            if (_socket != null)
            {
                _socket.Close();

                _socket = null;
                _stream = null;
            }
        }

        public override void Close()
        {
            //Debug.Log("Close");
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
            _networkThread.AddLog("[网络] TcpPersistent connect to " + _url.Host + ":" + _url.Port);
#endif

            System.Threading.ManualResetEvent mre = new System.Threading.ManualResetEvent(false);
            IAsyncResult result = _socket.BeginConnect(_url.Host, _url.Port, (ac) => { mre.Set(); }, null);
            bool active = mre.WaitOne(_timeout);
            if (active)
            {
                _socket.EndConnect(result);

                _stream = new NetworkStream(_socket);

                PrepareBuffer(ProtocolHeader.DataSize);
                BeginReceive(HeaderReceived);
            }
            else
            {
                throw new Exception("Connection timed out!");
            }
        }

        void PrepareBuffer(uint size)
        {
            _buffer = new byte[size];
            _totalBytes = _buffer.Length;
            _currentBytes = 0;
        }

        void BeginReceive(System.Action callback)
        {
            _stream.BeginRead(_buffer, _currentBytes, _totalBytes - _currentBytes, BeginReadCallback, callback);
        }

        void BeginReadCallback(IAsyncResult ar)
        {
            if (_socket != null && _stream != null)
            {
                try
                {
                    int readBytes = _stream.EndRead(ar);

                    if (readBytes <= 0)
                    {
                        throw new Exception("Stream closed unexpectedly when read!");
                    }

                    _currentBytes += readBytes;

                    if (_currentBytes < _totalBytes)
                    {
                        BeginReceive(ar.AsyncState as System.Action);
                    }
                    else
                    {
                        System.Action callback = ar.AsyncState as System.Action;
                        if (callback != null)
                        {
                            callback();
                        }
                    }
                }
                catch (Exception e)
                {
#if UNITY_EDITOR && !NETWORK_LOG
                    Debug.LogException(e);
#endif
#if NETWORK_LOG
                    _networkThread.AddLog(e);
#endif
                    Close();
                }
            }
        }

        void HeaderReceived()
        {
            _header.FromBytes(_buffer);

            _networkRspData = new NetworkRspData();

            _networkRspData.responseCommand = _header.mCommand;
            _networkRspData.result = _header.mRetCode;
            _networkRspData.sequence = _header.mSeq;

            PrepareBuffer(_header.mLen - ProtocolHeader.DataSize);
            BeginReceive(BodyReceived);
        }

        void BodyReceived()
        {
            _networkRspData.responseData = _buffer;

            _networkThread.PutResponseData(_networkRspData);

            PrepareBuffer(ProtocolHeader.DataSize);
            BeginReceive(HeaderReceived);
        }

        public override void ProcessProtocol(SendReqData reqData)
        {
            if (_socket == null || !_socket.Connected)
            {
                Connect();
            }

            _request = reqData.request;

            OnProcessEnter(reqData);

            try
            {
                Send(reqData.requestData);
                PostSend(reqData);

                OnProcessExit(reqData);
            }
            catch (Exception e)
            {
#if UNITY_EDITOR && !NETWORK_LOG
                Debug.LogException(e);
#endif
#if NETWORK_LOG
                _networkThread.AddLog(e);
#endif

                OnProcessFailed(reqData);

                Close();
            }
            finally
            {
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

        protected override void PostSend(SendReqData reqData)
        {
            base.PostSend(reqData);
            _networkThread.PutConfirmTimeoutData(reqData);
        }
    }
}
