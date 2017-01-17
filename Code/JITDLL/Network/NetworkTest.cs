using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Network;

public class NetworkTest : MonoBehaviour
{
    void OnEnable()
    {
        //NetworkManager.RegisterHandler((uint)PbLogin.command.CMD_VERIFY_RSP, OnVerifyRsp);
    }

    void OnDisable()
    {
        //NetworkManager.UnregisterHandler((uint)PbLogin.command.CMD_VERIFY_RSP, OnVerifyRsp);
    }

    // Use this for initialization
    void Start()
    {
        //NetworkManager.SetUrl(ProtocolDataType.TcpShort, "http://192.168.65.121:30400/");
        NetworkManager.SetUrl(ProtocolDataType.TcpShort, NetworkManager.CreateUri(DefaultConfig.GetString("ServerIp"), DefaultConfig.GetString("TcpShortPort")));
        NetworkManager.SetTimeout(ProtocolDataType.TcpShort, 5000);

        //NetworkManager.SetUrl(ProtocolDataType.Http, "http://192.168.65.121:30410/");
        NetworkManager.SetUrl(ProtocolDataType.Http, NetworkManager.CreateUri(DefaultConfig.GetString("ServerIp"), DefaultConfig.GetString("HttpPort")));
        NetworkManager.SetTimeout(ProtocolDataType.Http, 5000);
    }

    void Update()
    {

    }

    void OnDestory()
    {
        Debug.Log("OnDestory");
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 100, 40), "Click"))
        {
            //PbLogin.VerifyReq verifyReq = new PbLogin.VerifyReq();
            //verifyReq.platform = "tourist";
            //verifyReq.third_key = GetHashCode().ToString("X");
            //verifyReq.account = "wahaha";
            //verifyReq.version = 1;

            //NetworkManager.SendRequest(ProtocolDataType.TcpShort, verifyReq, 3);
            //NetworkManager.SendRequest(ProtocolDataType.TcpShort, verifyReq, 3);
            //NetworkManager.SendRequest(ProtocolDataType.TcpShort, verifyReq, 3);

            //NetworkManager.SendRequest(ProtocolDataType.Http, verifyReq, 3);
            //NetworkManager.SendRequest(ProtocolDataType.Http, verifyReq, 3);
            //NetworkManager.SendRequest(ProtocolDataType.Http, verifyReq, 3);
        }

        //if (GUI.Button(new Rect(100, 200, 100, 40), "NetworkTest1"))
        //{
        //    SceneManager.LoadScene("NetworkTest1");
        //}
        //if (GUI.Button(new Rect(100, 300, 100, 40), "NetworkTest2"))
        //{
        //    SceneManager.LoadScene("NetworkTest2");
        //}
    }

    void OnVerifyRsp(ushort result, object response)
    {
        Debug.Log("OnVerifyRsp " + result);
        if (result == 0)
        {
            PbLogin.VerifyRsp verifyRsp = response as PbLogin.VerifyRsp;
        }
    }
}
