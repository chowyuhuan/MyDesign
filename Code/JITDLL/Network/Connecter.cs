using UnityEngine;
using System.Collections;
using System.IO;
using System;

namespace Network
{
    public abstract class Connecter
    {
        protected NetworkThread _networkThread;

        protected Uri _url;
        protected int _timeout = 5000;

        protected object _request = null;
        protected SendReqData _sendReqData;

        public Connecter(NetworkThread networkThread)
        {
            _networkThread = networkThread;
        }

        public virtual void SetUrl(string url)
        {
            _url = new Uri(url);
        }

        /// <summary>
        /// 超时
        /// </summary>
        /// <param name="timeout">毫秒</param>
        public virtual void SetTimeout(int timeout)
        {
            _timeout = timeout;
        }

        public virtual int GetTimeout()
        {
            return _timeout;
        }

        public virtual void Close()
        {

        }

        protected virtual void OnProcessEnter(SendReqData reqData)
        {

        }

        protected virtual void OnProcessExit(SendReqData reqData)
        {

        }

        protected virtual void OnProcessFailed(SendReqData reqData)
        {
            _networkThread.PutFailedData(reqData);
        }

        protected virtual void PostSend(SendReqData reqData)
        {
#if NETWORK_LOG
            //NetworkManager.Log("[网络] 发送协议号: " + reqData.requestCommand);
            _networkThread.AddLog("[网络] 发送协议号: " + reqData.requestCommand);
#endif
        }

        public virtual void ProcessProtocol(SendReqData reqData)
        {
            _request = reqData.request;

            OnProcessEnter(reqData);

            try
            {
                Connect();
                Send(reqData.requestData);
                PostSend(reqData);
                Receive();

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
                Close();
            }
        }

        public virtual void Disconnect()
        {

        }

        public virtual void Connect()
        {

        }

        public virtual void Send(byte[] contents)
        {

        }

        public virtual void Receive()
        {

        }

        public virtual bool NeedTick()
        {
            return false;
        }

        public virtual void Tick()
        {

        }

        protected ProtocolHeader ReadHeader(Stream stream, int timeout)
        {
            byte[] headerData = new byte[ProtocolHeader.DataSize];
            Read(stream, headerData, headerData.Length, timeout);

            ProtocolHeader header = new ProtocolHeader();
            header.FromBytes(headerData);

            return header;
        }

        protected NetworkRspData ReadProtocolData(Stream stream, int timeout)
        {
            NetworkRspData networkRspData = new NetworkRspData();

            ProtocolHeader header = ReadHeader(stream, timeout);

            networkRspData.responseCommand = header.mCommand;
            networkRspData.result = header.mRetCode;
            networkRspData.sequence = header.mSeq;

            byte[] responseData = null;
            if (header.mRetCode == 0)
            {
                responseData = new byte[header.mLen - ProtocolHeader.DataSize];

                if (responseData.Length > 0)
                {
                    Read(stream, responseData, responseData.Length, timeout);
                }
            }

            networkRspData.responseData = responseData;

            return networkRspData;
        }

        public virtual void Read(Stream stream, byte[] buffer, int count, int timeout)
        {
            int readBytes = 0;
            int total = 0;
            System.Threading.ManualResetEvent mre = new System.Threading.ManualResetEvent(false);

            do
            {
                mre.Reset();
                IAsyncResult result = stream.BeginRead(buffer, total, count - total, (ac) => { mre.Set(); }, null);
                bool active;
                if (timeout > 0)
                {
                    active = mre.WaitOne(timeout);
                }
                else
                {
                    active = mre.WaitOne();
                }

                if (active)
                {
                    readBytes = stream.EndRead(result);

                    if (readBytes <= 0)
                    {
                        throw new Exception("Stream closed unexpectedly when read!");
                    }

                    total += readBytes;
                }
                else
                {
                    throw new Exception("Receive timed out!");
                }

            } while (total < count);
        }
    }
}

