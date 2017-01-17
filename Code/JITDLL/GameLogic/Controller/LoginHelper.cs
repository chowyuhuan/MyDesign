using UnityEngine;
using System.Collections;
using Network;
using DataCenter;
using Platform;

public sealed class LoginHelper
{
    public static System.Action<float> OnProgress;

    public static System.Action<bool> OnFinished;

    static string _loginData = "";

    //[RuntimeInitializeOnLoadMethod]
    public static void RegisterHandler()
    {
        NetworkManager.RegisterHandler((uint)PbLogin.command.CMD_VERIFY_RSP, OnVerifyRsp);
        NetworkManager.RegisterHandler((uint)PbLogin.command.CMD_CREATE_RSP, OnCreatRoleRsp);
        NetworkManager.RegisterHandler((uint)PbLogin.command.CMD_LOGON_RSP, OnLogonRsp);

        NetworkManager.RegisterHandler((uint)gsproto.command.CMD_LOGIN_RSP, OnLoginGsRsp);
        NetworkManager.RegisterHandler((uint)gsproto.command.CMD_DATA_RSP, OnPlayerDataRsp);

        NetworkManager.OnSendProtocolError += OnSendProtocolError;
    }

    static void RaiseOnProgress(float value)
    {
        if (OnProgress != null)
        {
            OnProgress(value);
        }
    }

    static void RaiseOnFinished(bool result)
    {
        if (OnFinished != null)
        {
            OnFinished(result);
        }
    }

    public static void StartLogin(string data)
    {
        RaiseOnProgress(0);

        _loginData = data;

        InitializeNetwork();

        VerifyRequest(_loginData);
    }

    static void InitializeNetwork()
    {
        NetworkManager.SetUrl(ProtocolDataType.TcpShort, NetworkManager.CreateUri(DefaultConfig.GetString("ServerIp"), DefaultConfig.GetString("TcpShortPort")));
        NetworkManager.SetTimeout(ProtocolDataType.TcpShort, DefaultConfig.GetInt("NetworkTimeout"));

        NetworkManager.SetUrl(ProtocolDataType.Http, NetworkManager.CreateUri(DefaultConfig.GetString("ServerIp"), DefaultConfig.GetString("HttpPort")));
        NetworkManager.SetTimeout(ProtocolDataType.Http, DefaultConfig.GetInt("NetworkTimeout"));

        NetworkManager.SetUrl(ProtocolDataType.Udp, NetworkManager.CreateUri(DefaultConfig.GetString("ServerIp"), DefaultConfig.GetString("UdpPort")));
        NetworkManager.SetTimeout(ProtocolDataType.Udp, DefaultConfig.GetInt("NetworkTimeout"));
    }

    static void VerifyRequest(string data)
    {
        PbLogin.VerifyReq verifyReq = PlatformInterface.GetVerifyReqInfo(data);

        NetworkManager.SendRequest(ProtocolDataType.TcpShort, verifyReq);
    }

    static void CreateRoleRequest()
    {
        PbLogin.CreateReq createReq = new PbLogin.CreateReq();

        createReq.account = PlayerDataCenter.Account;
        createReq.session_id = PlayerDataCenter.SessionId;

        PlatformInterface.AppendCreateReqInfo(ref createReq, _loginData);

        NetworkManager.SendRequest(ProtocolDataType.TcpShort, createReq);
    }

    static void LogonRequest()
    {
        PbLogin.LogonReq logonReq = new PbLogin.LogonReq();
        logonReq.account = PlayerDataCenter.Account;
        logonReq.aid = PlayerDataCenter.Aid;
        logonReq.sid = PlayerDataCenter.Sid;
        logonReq.role_name = PlayerDataCenter.Nickname;
        logonReq.session_id = PlayerDataCenter.SessionId;
        logonReq.roleid = PlayerDataCenter.RoleId;

        NetworkManager.SendRequest(ProtocolDataType.TcpShort, logonReq);
    }

    static void LoginGsRequest()
    {
        gsproto.LoginReq loginReq = new gsproto.LoginReq();

        loginReq.account = PlayerDataCenter.Account;
        loginReq.name = PlayerDataCenter.Nickname;
        loginReq.imgid = 0;
        loginReq.device_id = PlayerDataCenter.DeviceId;

        loginReq.token = new byte[PlayerDataCenter.UserToken.Length];
        loginReq.is_reconnect = 0;

        System.Array.Copy(PlayerDataCenter.UserToken, 0, loginReq.token, 0, PlayerDataCenter.UserToken.Length);
        loginReq.roleid = PlayerDataCenter.RoleId;

        NetworkManager.SendRequest(ProtocolDataType.TcpShort, loginReq);
    }

    static void PlayerDataRequest()
    {
        gsproto.PlayerDataReq playerDataReq = new gsproto.PlayerDataReq();

        playerDataReq.session_id = PlayerDataCenter.SessionId;

        NetworkManager.SendRequest(ProtocolDataType.TcpShort, playerDataReq);
    }

    static void OnSendProtocolError()
    {
        RaiseOnFinished(false);
    }

    static void OnVerifyRsp(ushort result, object response, object request)
    {
        RaiseOnProgress(0.1f);

        if (result == 0)
        {
            PbLogin.VerifyRsp verifyRsp = response as PbLogin.VerifyRsp;

            if (verifyRsp.roles.Count == 0)
            {
                CreateRoleRequest();
            }
            else
            {
                LogonRequest();
            }
        }
        else
        {
            RaiseOnFinished(false);
        }
    }

    static void OnCreatRoleRsp(ushort result, object response, object request)
    {
        RaiseOnProgress(0.2f);

        if (result == 0)
        {
            PbLogin.CreateRsp createRsp = response as PbLogin.CreateRsp;

            LogonRequest();
        }
        else
        {
            RaiseOnFinished(false);
        }
    }

    static void OnLogonRsp(ushort result, object response, object request)
    {
        RaiseOnProgress(0.3f);

        if (result == 0)
        {
            PbLogin.LogonRsp logonRsp = response as PbLogin.LogonRsp;

            NetworkManager.SetUrl(ProtocolDataType.TcpShort, NetworkManager.CreateUri(logonRsp.gs_ip, logonRsp.gs_port.ToString()));
            NetworkManager.SetUrl(ProtocolDataType.Http, NetworkManager.CreateUri(logonRsp.gs_ip, logonRsp.gs_http_port.ToString()));
            NetworkManager.SetUrl(ProtocolDataType.Udp, NetworkManager.CreateUri(logonRsp.gs_ip, logonRsp.gs_udp_port.ToString()));

            LoginGsRequest();
        }
        else
        {
            RaiseOnFinished(false);
        }
    }

    static void OnLoginGsRsp(ushort result, object response, object request)
    {
        RaiseOnProgress(0.4f);

        if (result == 0)
        {
            gsproto.LoginRsp loginRsp = response as gsproto.LoginRsp;

            PlayerDataRequest();
        }
        else
        {
            RaiseOnFinished(false);
        }
    }

    static void OnPlayerDataRsp(ushort result, object response, object request)
    {
        RaiseOnProgress(1f);
        if (result == 0)
        {
            RaiseOnFinished(true);
        }
        else
        {
            RaiseOnFinished(false);
        }
    }
}
