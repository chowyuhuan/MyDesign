using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System;

namespace Network
{
    public sealed class NetworkReqData
    {
        // 协议类别 
        public ProtocolDataType protocolType;
        // 协议类 
        public object Request;
        // 序列号 
        public uint sequence;
        // 重发次数 
        public int resendCount;
    }

    public sealed class NetworkRspData
    {
        // 请求协议类
        public object request;
        // 协议号 
        public uint responseCommand;
        // 服务器返回的结果 
        public ushort result;
        // 序列号 
        public uint sequence;
        // 协议数据 
        public byte[] responseData;
    }

    public sealed class DispatchData
    {
        // 协议号 
        public uint responseCommand;
        // 服务器返回的结果 
        public ushort result;
        // 返回协议类 
        public object response;
        // 请求协议类
        public object request;
    }

    public sealed class SendReqData
    {
        // 请求协议号
        public uint requestCommand;
        // 请求协议类
        public object request;
        // 协议类别 
        public ProtocolDataType protocolType;
        // 协议数据 
        public byte[] requestData;
        // 序列号 
        public uint sequence;
        // 重发次数 
        public int resendCount;


        // 超时时间 
        public int timeout;
        // 发送时间 
        public DateTime sendTime;
        // 实际重发次数 
        public int retryCount;
    }

    public sealed class LogData
    {
        public string log;
        public Exception exception;

        public LogData()
        {

        }

        public LogData(string log)
        {
            this.log = log;
        }

        public LogData(Exception exception)
        {
            this.exception = exception;
        }

        public LogData(string log, Exception exception)
        {
            this.log = log;
            this.exception = exception;
        }

        public void Show()
        {
            if (exception != null)
            {
                Debug.LogException(exception);
            }
            if (!string.IsNullOrEmpty(log))
            {
                Debug.Log(log);
            }
        }
    }

    public sealed class NetworkThread
    {
        Connecter[] _connecters = new Connecter[(int)ProtocolDataType.Max];

        Thread _serializeThread;
        Thread _deserializeThread;
        Thread _sendThread;
        Thread _tickThread;

        // 发送前未序列化的数据 
        Queue<NetworkReqData> _networkReqDatas = new Queue<NetworkReqData>();

        // 接收后未反序列化的接收数据 
        Queue<NetworkRspData> _networkRspDatas = new Queue<NetworkRspData>();

        ManualResetEvent _mreReq = new ManualResetEvent(false);

        ManualResetEvent _mreRsp = new ManualResetEvent(false);

        // 反序列化后可以分发回调的数据 
        Queue<DispatchData> _dispatchDatas = new Queue<DispatchData>();

        // 序列化后等待发送的数据 
        Queue<SendReqData> _sendReqDatas = new Queue<SendReqData>();

        ManualResetEvent _mreSend = new ManualResetEvent(false);

        // 确认超时的数据 
        List<SendReqData> _confirmSendTimeoutDatas = new List<SendReqData>();

        // 发送失败的数据 
        Queue<SendReqData> _sendFailedDatas = new Queue<SendReqData>();

        Queue<LogData> _logQueue = new Queue<LogData>();

        public void Initialize()
        {
            _connecters[(int)ProtocolDataType.TcpShort] = new TcpConnecterShort(this);
            _connecters[(int)ProtocolDataType.TcpPersistent] = new TcpConnecterPersistent(this);
            _connecters[(int)ProtocolDataType.Http] = new HttpConnecter(this);
            _connecters[(int)ProtocolDataType.Udp] = new UdpConnecterEnet(this);

            _serializeThread = new Thread(SerializeThread);
            //_thread.Name = "SerializeThread";
            _serializeThread.IsBackground = true;
            _serializeThread.Start();

            _deserializeThread = new Thread(DeserializeThread);
            //_thread.Name = "DeserializeThread";
            _deserializeThread.IsBackground = true;
            _deserializeThread.Start();

            _sendThread = new Thread(SendThread);
            //_thread.Name = "SendThread";
            _sendThread.IsBackground = true;
            _sendThread.Start();

            CheckEnableTick();
        }

        void CheckEnableTick()
        {
            for (int i = 0; i < _connecters.Length; ++i)
            {
                if (_connecters[i] != null && _connecters[i].NeedTick())
                {
                    _tickThread = new Thread(TickThread);
                    //_thread.Name = "TickThread";
                    _tickThread.IsBackground = true;
                    _tickThread.Start();
                }
            }
        }

        bool IsConnecterValid(ProtocolDataType protocolType)
        {
            if (_connecters[(int)protocolType] != null)
            {
                return true;
            }
            else
            {
                Debug.LogError(protocolType.ToString() + " connecter is invalid!");
                return false;
            }
        }

        public void SetUrl(ProtocolDataType protocolType, string url)
        {
            if (IsConnecterValid(protocolType))
            {
                _connecters[(int)protocolType].SetUrl(url);
            }
        }

        public void SetTimeout(ProtocolDataType protocolType, int timeout)
        {
            if (IsConnecterValid(protocolType))
            {
                _connecters[(int)protocolType].SetTimeout(timeout);
            }
        }

        public void ClearData()
        {
            lock(_networkReqDatas)
            {
                _networkReqDatas.Clear();
            }
            lock (_networkRspDatas)
            {
                _networkRspDatas.Clear();
            }
            lock (_dispatchDatas)
            {
                _dispatchDatas.Clear();
            }
            lock (_sendReqDatas)
            {
                _sendReqDatas.Clear();
            }
            lock (_confirmSendTimeoutDatas)
            {
                _confirmSendTimeoutDatas.Clear();
            }
            lock (_sendFailedDatas)
            {
                _sendFailedDatas.Clear();
            }
        }

        public void ThreadAbort()
        {
            _serializeThread.Abort();
            _deserializeThread.Abort();
            _sendThread.Abort();

            if (_tickThread != null) _tickThread.Abort();
        }

        public void AddLog(string logString)
        {
            lock(_logQueue)
            {
                _logQueue.Enqueue(new LogData(logString));
            }
        }

        public void AddLog(Exception e)
        {
            lock (_logQueue)
            {
                _logQueue.Enqueue(new LogData(e));
            }
        }

        public LogData GetLog()
        {
            lock (_logQueue)
            {
                if (_logQueue.Count > 0)
                {
                    return _logQueue.Dequeue();
                }
                else
                {
                    return null;
                }
            }
        }

        void SerializeThread()
        {
            //Debug.Log("SerializeThread");

            while(true)
            {
                _mreReq.WaitOne();

                if (Monitor.TryEnter(_networkReqDatas))
                {

                    NetworkReqData networkReqData = null;

                    if (_networkReqDatas.Count > 0)
                    {
                        networkReqData = _networkReqDatas.Dequeue();
                    }
                    else
                    {
                        _mreReq.Reset();
                    }

                    Monitor.Exit(_networkReqDatas);

                    if (networkReqData != null)
                    {
                        SerializeReqData(networkReqData);
                    }

                }
            }
        }

        void DeserializeThread()
        {
            //Debug.Log("DeserializeThread");
            while (true)
            {
                _mreRsp.WaitOne();

                if (Monitor.TryEnter(_networkRspDatas))
                {
                    NetworkRspData networkRspData = null;

                    if (_networkRspDatas.Count > 0)
                    {
                        networkRspData = _networkRspDatas.Dequeue();
                    }
                    else
                    {
                        _mreRsp.Reset();
                    }

                    Monitor.Exit(_networkRspDatas);

                    if (networkRspData != null)
                    {
                        DeserializeRspData(networkRspData);
                    }
                }
            }
        }

        public void PutRequestData(NetworkReqData reqData)
        {
            //Debug.Log("PutRequestData");
            lock(_networkReqDatas)
            {
                _networkReqDatas.Enqueue(reqData);
                _mreReq.Set();
            }
        }

        public void PutResponseData(NetworkRspData networkRspData)
        {
            //Debug.Log("PutResponseData");
            lock (_networkRspDatas)
            {
                _networkRspDatas.Enqueue(networkRspData);
                _mreRsp.Set();
            }
        }

        void SerializeReqData(NetworkReqData reqData)
        {
            //Debug.Log("SerializeReqData");
            try
            {
                uint requestCommand = NetworkManager.GetRequestCommand(reqData.Request.GetType());
                if (requestCommand == 0)
                {
                    return;
                }

                MemoryStream ms = new MemoryStream();
                ProtoBuf.Serializer.Serialize(ms, reqData.Request);
                byte[] bodyData = ms.ToArray();

                ProtocolHeader header = new ProtocolHeader();
                header.mCommand = requestCommand;
                header.mSeq = reqData.sequence;
                header.mRetCode = 0;
                header.mMagic = 53556;
                header.mLen = (uint)bodyData.Length + ProtocolHeader.DataSize;
                byte[] totalData = new byte[header.mLen];

                int pos = 0;
                header.ToBytes(totalData, ref pos);
                Array.Copy(bodyData, 0, totalData, pos, bodyData.Length);

                SendReqData sendReqData = new SendReqData();
                sendReqData.requestCommand = header.mCommand;
                sendReqData.request = reqData.Request;
                sendReqData.protocolType = reqData.protocolType;
                sendReqData.requestData = totalData;
                sendReqData.sequence = reqData.sequence;
                sendReqData.resendCount = reqData.resendCount;

                sendReqData.retryCount = reqData.resendCount;

                lock(_sendReqDatas)
                {
                    _sendReqDatas.Enqueue(sendReqData);
                    _mreSend.Set();
                }
            }
            catch (Exception e)
            {
#if UNITY_EDITOR && !NETWORK_LOG
                Debug.LogException(e);
#endif
#if NETWORK_LOG
                AddLog(e);
#endif
            }
        }

        void DeserializeRspData(NetworkRspData rspData)
        {
            //Debug.Log("DeserializeRspData");

            DispatchData dd = new DispatchData();

            try
            {
                object response = null;
                Type responseType = NetworkManager.GetResponseType(rspData.responseCommand);

                if (rspData.result == 0 && responseType != null)
                {
                    MemoryStream ms = new MemoryStream(rspData.responseData, 0, rspData.responseData.Length);
                    response = ProtoBuf.Serializer.Deserialize(ms, responseType);
                }

                dd.responseCommand = rspData.responseCommand;
                dd.result = rspData.result;
                dd.response = response;
                dd.request = rspData.request;

                lock (_confirmSendTimeoutDatas)
                {
                    for (int i = 0; i < _confirmSendTimeoutDatas.Count; ++i)
                    {
                        // 是长连接的response 
                        if (_confirmSendTimeoutDatas[i].sequence == rspData.sequence)
                        {
                            dd.request = _confirmSendTimeoutDatas[i].request;
                            _confirmSendTimeoutDatas.RemoveAt(i);
                            break;
                        }
                    }
                }

#if NETWORK_LOG
                //NetworkManager.Log("[网络] 接收协议号: " + dd.responseCommand);
                AddLog("[网络] 接收协议号: " + dd.responseCommand);
#endif
            }
            catch (Exception e)
            {
#if UNITY_EDITOR && !NETWORK_LOG
                Debug.LogException(e);
#endif
#if NETWORK_LOG
                AddLog(e);
#endif
                dd = null;
            }

            if (dd != null)
            {
                lock (_dispatchDatas)
                {
                    _dispatchDatas.Enqueue(dd);
                }
            }
        }

        public DispatchData GetDispatchData()
        {
            lock (_dispatchDatas)
            {
                if (_dispatchDatas.Count > 0)
                {
                    return _dispatchDatas.Dequeue();
                }
            }

            return null;
        }

        void SendThread()
        {
            while (true)
            {
                _mreSend.WaitOne();

                SendReqData sendReqData = null;

                if (Monitor.TryEnter(_sendReqDatas))
                {
                    if (_sendReqDatas.Count > 0)
                    {
                        sendReqData = _sendReqDatas.Dequeue();
                    }
                    else
                    {
                        _mreSend.Reset();
                    }

                    Monitor.Exit(_sendReqDatas);

                    if (sendReqData != null)
                    {
                        if (IsConnecterValid(sendReqData.protocolType))
                        {
                            _connecters[(int)sendReqData.protocolType].ProcessProtocol(sendReqData);
                        }
                    }
                }
            }
        }

        public void CheckSendTimeout()
        {
            lock (_confirmSendTimeoutDatas)
            {
                for (int i = 0; i < _confirmSendTimeoutDatas.Count; ++i)
                {
                    if ((DateTime.Now - _confirmSendTimeoutDatas[i].sendTime).TotalMilliseconds > _confirmSendTimeoutDatas[i].timeout)
                    {
                        PutFailedData(_confirmSendTimeoutDatas[i]);

                        _confirmSendTimeoutDatas.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public void PutConfirmTimeoutData(SendReqData reqData)
        {
            // 可能要排除一些不需要超时确认的协议，待拓展 

            reqData.sendTime = DateTime.Now;
            reqData.timeout = _connecters[(int)reqData.protocolType].GetTimeout();

            lock (_confirmSendTimeoutDatas)
            {
                _confirmSendTimeoutDatas.Add(reqData);
            }
        }

        public void PutFailedData(SendReqData failedData)
        {
#if NETWORK_LOG
            //NetworkManager.Log("[网络] 协议处理失败,协议号: " + failedData.requestCommand);
            AddLog("[网络] 协议处理失败,协议号: " + failedData.requestCommand);

            if (IsConnecterValid(failedData.protocolType))
            {
                _connecters[(int)failedData.protocolType].Disconnect();
            }
#endif
            if (failedData.retryCount > 0)
            {
                lock (_sendReqDatas)
                {
                    _sendReqDatas.Enqueue(failedData);
                    _mreSend.Set();
                }

                failedData.retryCount--;
#if NETWORK_LOG
                //NetworkManager.Log("[网络] 重发协议号: " + failedData.requestCommand + " 次数 " + (failedData.resendCount - failedData.retryCount) + "/" + failedData.resendCount);
                AddLog("[网络] 重发协议号: " + failedData.requestCommand + " 次数 " + (failedData.resendCount - failedData.retryCount) + "/" + failedData.resendCount);
#endif
                //GameUI.GUI_MessageManager.Instance.ShowErrorTip("重发次数 " + (failedData.resendCount - failedData.retryCount) + "/" + failedData.resendCount);
            }
            else
            {
                lock (_sendFailedDatas)
                {
                    _sendFailedDatas.Enqueue(failedData);
                }
            }
        }

        public bool HasSendFailedData()
        {
            lock (_sendFailedDatas)
            {
                return _sendFailedDatas.Count > 0;
            }
        }

        public void ClearFailedData()
        {
            lock (_sendFailedDatas)
            {
                _sendFailedDatas.Clear();
            }
        }

        public void ResendFailedData()
        {
            lock (_sendFailedDatas)
            {
                while(_sendFailedDatas.Count > 0)
                {
                    SendReqData failedData = _sendFailedDatas.Dequeue();
                    failedData.retryCount = failedData.resendCount;

                    lock(_sendReqDatas)
                    {
                        _sendReqDatas.Enqueue(failedData);
                    }

                    _mreSend.Set();
                }
            }
        }

        void TickThread()
        {
            while (true)
            {
                try
                {
                    for (int i = 0; i < _connecters.Length; ++i)
                    {
                        if (_connecters[i] != null && _connecters[i].NeedTick())
                        {
                            _connecters[i].Tick();
                        }
                    }

                    Thread.Sleep(1);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}

