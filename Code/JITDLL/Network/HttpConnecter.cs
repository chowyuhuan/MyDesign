using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Net;
using System;

namespace Network
{
    /// <summary>
    /// Http连接 
    /// </summary>
    public class HttpConnecter : Connecter
    {
        HttpWebRequest httpRequest = null;
        HttpWebResponse httpResponse = null;
        Stream requestStream = null;
        Stream responseStream = null;

        string _boundary = "";

        byte[] _bodyHeader;
        byte[] _bodyTail;
        int _contentLength;

        public HttpConnecter(NetworkThread networkThread)
            : base(networkThread)
        {
            CreateBoundary();
            CreateBodyHeader();
            CreateBodyTail();
        }

        void CreateBoundary()
        {
            _boundary = "--" + DateTime.Now.Ticks.ToString("X");
        }

        void CreateBodyHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(_boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append("data");
            sb.Append("\"; filename=\"");
            sb.Append("data.dat");
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append("application/octet-stream");
            //sb.Append("\r\n");
            //sb.Append("Content-Length: " + packet_data.Length.ToString());
            sb.Append("\r\n");
            sb.Append("\r\n");

            _bodyHeader = Encoding.UTF8.GetBytes(sb.ToString());
        }

        void CreateBodyTail()
        {
            _bodyTail = Encoding.UTF8.GetBytes("\r\n--" + _boundary + "\r\n");
        }

        protected override void OnProcessEnter(SendReqData reqData)
        {
            _contentLength = reqData.requestData.Length + _bodyHeader.Length + _bodyTail.Length;
        }

        public override void Connect()
        {
            httpRequest = WebRequest.Create(_url) as HttpWebRequest;

            httpRequest.AllowWriteStreamBuffering = false;
            httpRequest.ContentType = "multipart/form-data; boundary=" + _boundary;
            httpRequest.Method = "POST";
            httpRequest.Timeout = _timeout;
            httpRequest.ContentLength = _contentLength;

            requestStream = httpRequest.GetRequestStream();
        }

        public override void Send(byte[] contents)
        {
            byte[] totalData = new byte[_contentLength];
            Array.Copy(_bodyHeader, 0, totalData, 0, _bodyHeader.Length);
            Array.Copy(contents, 0, totalData, _bodyHeader.Length, contents.Length);
            Array.Copy(_bodyTail, 0, totalData, _bodyHeader.Length + contents.Length, _bodyTail.Length);

            requestStream.Write(totalData, 0, totalData.Length);
            requestStream.Flush();

            httpResponse = (HttpWebResponse)httpRequest.GetResponse();
        }

        public override void Receive()
        {
            responseStream = httpResponse.GetResponseStream();

            NetworkRspData networkRspData = null;
            networkRspData = ReadProtocolData(responseStream, _timeout);
            networkRspData.request = _request;

            _networkThread.PutResponseData(networkRspData);
        }

        public override void Close()
        {
            if (httpResponse != null)
                httpResponse.Close();
            if (requestStream != null)
                requestStream.Close();
            if (responseStream != null)
                responseStream.Close();

            httpRequest = null;
            httpResponse = null;
            requestStream = null;
            responseStream = null;
        }
    }
}
