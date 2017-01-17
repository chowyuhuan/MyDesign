using UnityEngine;
using System.Collections;
using System.IO;
using System;

namespace Network
{
    public class UdpConnecterEnet : Connecter
    {
        ENet.Host _host;
        ENet.Peer _peer;

        ENet.Event _event;

        public UdpConnecterEnet(NetworkThread networkThread)
            : base(networkThread)
        {
        }

        public override void SetUrl(string url)
        {
            base.SetUrl(url);

            if (_peer.State == ENet.PeerState.Connected)
            {
                _peer.Disconnect(0);
            }
        }

        public override void Close()
        {
            if (_peer.State == ENet.PeerState.Connected)
            {
                _peer.Disconnect(0);
            }

            if (_host.IsInitialized)
            {
                _host.Dispose();
            }
        }

        public override void Disconnect()
        {
            if (_peer.State == ENet.PeerState.Connected)
            {
                _peer.Disconnect(0);
            }
        }

        public override void Connect()
        {
            _host = new ENet.Host();
            _host.Initialize(null, 1);

#if NETWORK_LOG
            _networkThread.AddLog("[网络] enet connect to "+_url.Host + ":" + _url.Port);
#endif
            _peer = _host.Connect(_url.Host, _url.Port, 0, 0);

            if (_host.Service(_timeout, out _event))
            {
                if (_event.Type == ENet.EventType.Connect)
                {
#if NETWORK_LOG
                    _networkThread.AddLog("[网络] enet connected ");
#endif
                }
            }

            if (!(_peer.State == ENet.PeerState.Connected))
            {
                throw new Exception("Connect timed out!");
            }
        }

        public override bool NeedTick()
        {
            return true;
        }

        public override void Tick()
        {
            if (_host.IsInitialized && _peer.State == ENet.PeerState.Connected)
            {
                //Debug.Log("Service");

                if (_host.Service(0, out _event))
                {
                    do
                    {
                        switch (_event.Type)
                        {
                            case ENet.EventType.Disconnect:
                                break;

                            case ENet.EventType.Receive:
                                Receive();
                                _event.Packet.Dispose();
                                break;

                            default:
                                break;
                        }
                    }
                    while (_host.CheckEvents(out _event));
                }
            }
        }

        public override void ProcessProtocol(SendReqData reqData)
        {
            _request = reqData.request;

            OnProcessEnter(reqData);

            try
            {
                if (!(_peer.State == ENet.PeerState.Connected))
                {
                    Connect();
                }

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
            }
            finally
            {
            }
        }

        public override void Send(byte[] contents)
        {
            _peer.Send(0, contents, ENet.PacketFlags.Reliable);
        }

        public override void Receive()
        {
            byte[] data = _event.Packet.GetBytes();
            MemoryStream stream = new MemoryStream(data);

            NetworkRspData networkRspData = null;
            networkRspData = ReadProtocolData(stream, _timeout);
            networkRspData.request = _request;

            _networkThread.PutResponseData(networkRspData);
        }

        protected override void PostSend(SendReqData reqData)
        {
            base.PostSend(reqData);
            _networkThread.PutConfirmTimeoutData(reqData);
        }
    }
}
