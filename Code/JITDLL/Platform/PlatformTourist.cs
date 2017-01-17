using UnityEngine;
using System.Collections;

namespace Platform
{
    public class PlatformTourist : PlatformBase
    {
        public override void CallPlatformLogin()
        {
            //Debug.Log("CallPlatformLogin");

            // 没有底层的sdk，直接调用登录回调 
            _platformObject.SendMessage("OnPlatformLogin", "", SendMessageOptions.DontRequireReceiver);
        }

        public override PbLogin.VerifyReq GetVerifyReqInfo(string data)
        {
            //Debug.Log("GetVerifyReqInfo");
            PbLogin.VerifyReq verifyReq = base.GetVerifyReqInfo(data);

            verifyReq.platform = "tourist";
            verifyReq.third_key = System.DateTime.Now.Ticks.ToString("X");
            verifyReq.account = DataCenter.PlayerDataCenter.DeviceId;
            verifyReq.account = "zyh123";
            verifyReq.version = 1;

            return verifyReq;
        }

        public override void AppendCreateReqInfo(ref PbLogin.CreateReq req, string data)
        {
            req.aid = 1;
            req.sid = 1;
            req.role_name = "Newbie";

            DataCenter.PlayerDataCenter.Aid = req.aid;
            DataCenter.PlayerDataCenter.Sid = req.sid;
            DataCenter.PlayerDataCenter.Nickname = req.role_name;
        }
    }
}

