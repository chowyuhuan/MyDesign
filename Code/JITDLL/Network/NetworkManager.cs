using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// 先在InitTypeAndCommand里加上待添加的协议数据
// 上层发送时调用SendRequest
// 上层接收时注册NetworkManager.RegisterHandler(),注销使用NetworkManager.UnregisterHandler()


namespace Network
{

    public enum ProtocolDataType
    {
        TcpShort,
        TcpPersistent,
        Http,
        Udp,

        Max,
    }


    public sealed partial class NetworkManager
    {
        static NetworkThread _networkThread;

        static uint _sequence = 0;

        static uint NewSequence
        {
            get
            {
                _sequence++;
                return _sequence;
            }
        }

        public static uint CurrentSequence
        {
            get
            {
                return _sequence;
            }
        }

        public static System.Action OnSendProtocolError;

        //[RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            _networkThread = new NetworkThread();
            _networkThread.Initialize();

            InitTypeAndCommand();

            NetworkUpdate.CreateInstance();
        }

        public static string CreateUri(string ip, string port)
        {
            return "http://" + ip + ":" + port;
        }

        public static void SetUrl(ProtocolDataType protocolType, string url)
        {
            _networkThread.SetUrl(protocolType, url);
        }

        public static void SetTimeout(ProtocolDataType protocolType, int timeout)
        {
            _networkThread.SetTimeout(protocolType, timeout);
        }

        public static void ClearData()
        {
            _networkThread.ClearData();
        }

        public static void ThreadAbort()
        {
            _networkThread.ThreadAbort();
        }

        static Dictionary<uint, Delegate> _callback = new Dictionary<uint, Delegate>();
        static Dictionary<Type, uint> _typeToRequestCommand = new Dictionary<Type, uint>();
        static Dictionary<uint, Type> _responseCommandToType = new Dictionary<uint, Type>();

        public static uint GetRequestCommand(Type classType)
        {
            if (_typeToRequestCommand.ContainsKey(classType))
            {
                return _typeToRequestCommand[classType];
            }
            else
            {
                Debug.LogError("GetRequestCommand : Failed to found " + classType.ToString());
            }

            return 0;
        }
        public static Type GetResponseType(uint responseCommand)
        {
            if (_responseCommandToType.ContainsKey(responseCommand))
            {
                return _responseCommandToType[responseCommand];
            }
            else
            {
                Debug.LogError("GetResponseType : Failed to found command " + responseCommand.ToString());
            }

            return null;
        }

        static void InitTypeAndCommand()
        {
            _typeToRequestCommand.Clear();
            _responseCommandToType.Clear();

            // logon server 
            _typeToRequestCommand.Add(typeof(PbLogin.VerifyReq), (uint)PbLogin.command.CMD_VERIFY_REQ);
            _responseCommandToType.Add((uint)PbLogin.command.CMD_VERIFY_RSP, typeof(PbLogin.VerifyRsp));

            _typeToRequestCommand.Add(typeof(PbLogin.CreateReq), (uint)PbLogin.command.CMD_CREATE_REQ);
            _responseCommandToType.Add((uint)PbLogin.command.CMD_CREATE_RSP, typeof(PbLogin.CreateRsp));

            _typeToRequestCommand.Add(typeof(PbLogin.LogonReq), (uint)PbLogin.command.CMD_LOGON_REQ);
            _responseCommandToType.Add((uint)PbLogin.command.CMD_LOGON_RSP, typeof(PbLogin.LogonRsp));

            // gs server 
            _typeToRequestCommand.Add(typeof(gsproto.LoginReq), (uint)gsproto.command.CMD_LOGIN_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_LOGIN_RSP, typeof(gsproto.LoginRsp));

            _typeToRequestCommand.Add(typeof(gsproto.PlayerDataReq), (uint)gsproto.command.CMD_DATA_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_DATA_RSP, typeof(gsproto.PlayerDataRsp));

            _typeToRequestCommand.Add(typeof(gsproto.TrainHeroReq), (uint)gsproto.command.CMD_TRAIN_HERO_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_TRAIN_HERO_RSP, typeof(gsproto.TrainHeroRsp));

            _typeToRequestCommand.Add(typeof(gsproto.HeroEvolutionReq), (uint)gsproto.command.CMD_HERO_EVOLUTION_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_HERO_EVOLUTION_RSP, typeof(gsproto.HeroEvolutionRsp));

            _typeToRequestCommand.Add(typeof(gsproto.ExtendBagReq), (uint)gsproto.command.CMD_EXTEND_BAG_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_EXTEND_BAG_RSP, typeof(gsproto.ExtendBagRsp));

            _typeToRequestCommand.Add(typeof(gsproto.SellItemReq), (uint)gsproto.command.CMD_SELL_ITEM_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_SELL_ITEM_RSP, typeof(gsproto.SellItemRsp));

            _typeToRequestCommand.Add(typeof(gsproto.StartRoastReq), (uint)gsproto.command.CMD_START_ROAST_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_START_ROAST_RSP, typeof(gsproto.StartRoastRsp));

            _typeToRequestCommand.Add(typeof(gsproto.CancelRoastReq), (uint)gsproto.command.CMD_CANCEL_ROAST_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_CANCEL_ROAST_RSP, typeof(gsproto.CancelRoastRsp));

            _typeToRequestCommand.Add(typeof(gsproto.ReceiveBreadReq), (uint)gsproto.command.CMD_RECEIVE_BREAD_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_RECEIVE_BREAD_RSP, typeof(gsproto.ReceiveBreadRsp));

            _typeToRequestCommand.Add(typeof(gsproto.FinishRoastReq), (uint)gsproto.command.CMD_FINISH_ROAST_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_FINISH_ROAST_RSP, typeof(gsproto.FinishRoastRsp));

            _typeToRequestCommand.Add(typeof(gsproto.DismissHeroReq), (uint)gsproto.command.CMD_DISMISS_HERO_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_DISMISS_HERO_RSP, typeof(gsproto.DismissHeroRsp));

            _typeToRequestCommand.Add(typeof(gsproto.LockHeroReq), (uint)gsproto.command.CMD_LOCK_HERO_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_LOCK_HERO_RSP, typeof(gsproto.LockHeroRsp));

            _typeToRequestCommand.Add(typeof(gsproto.ChangeRepresentHeroReq), (uint)gsproto.command.CMD_CHANGE_REPRESENT_HERO_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_CHANGE_REPRESENT_HERO_RSP, typeof(gsproto.ChangeRepresentHeroRsp));

            _typeToRequestCommand.Add(typeof(gsproto.EnterPassReq), (uint)gsproto.command.CMD_ENTER_PASS_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_ENTER_PASS_RSP, typeof(gsproto.EnterPassRsp));

            _typeToRequestCommand.Add(typeof(gsproto.PassOverReq), (uint)gsproto.command.CMD_PASS_OVER_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_PASS_OVER_RSP, typeof(gsproto.PassOverRsp));

            _typeToRequestCommand.Add(typeof(gsproto.HeroFruitReq), (uint)gsproto.command.CMD_HERO_FRUIT_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_HERO_FRUIT_RSP, typeof(gsproto.HeroFruitRsp));

            _typeToRequestCommand.Add(typeof(gsproto.EquipUpToHeroReq), (uint)gsproto.command.CMD_EQUIP_UP_TO_HERO_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_EQUIP_UP_TO_HERO_RSP, typeof(gsproto.EquipUpToHeroRsp));

            _typeToRequestCommand.Add(typeof(gsproto.EquipDownFromHeroReq), (uint)gsproto.command.CMD_EQUIP_DOWN_FROM_HERO_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_EQUIP_DOWN_FROM_HERO_RSP, typeof(gsproto.EquipDownFromHeroRsp));

            _typeToRequestCommand.Add(typeof(gsproto.LockEquipReq), (uint)gsproto.command.CMD_LOCK_EQUIP_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_LOCK_EQUIP_RSP, typeof(gsproto.LockEquipRsp));

            _typeToRequestCommand.Add(typeof(gsproto.WeaponReformReq), (uint)gsproto.command.CMD_WEAPON_REFORM_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_WEAPON_REFORM_RSP, typeof(gsproto.WeaponReformRsp));

            _typeToRequestCommand.Add(typeof(gsproto.WeaponBreakReq), (uint)gsproto.command.CMD_WEAPON_BREAK_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_WEAPON_BREAK_RSP, typeof(gsproto.WeaponBreakRsp));

            _typeToRequestCommand.Add(typeof(gsproto.WeaponRefineReq), (uint)gsproto.command.CMD_WEAPON_REFINE_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_WEAPON_REFINE_RSP, typeof(gsproto.WeaponRefineRsp));

            _typeToRequestCommand.Add(typeof(gsproto.ReformCostResetReq), (uint)gsproto.command.CMD_REFORM_COST_RESET_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_REFORM_COST_RESET_RSP, typeof(gsproto.ReformCostResetRsp));

            _typeToRequestCommand.Add(typeof(gsproto.SpecialWeaponReformResetReq), (uint)gsproto.command.CMD_SPECIAL_WEAPON_REFORM_RESET_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_SPECIAL_WEAPON_REFORM_RESET_RSP, typeof(gsproto.SpecialWeaponReformResetRsp));

            _typeToRequestCommand.Add(typeof(gsproto.AcquireSkillReq), (uint)gsproto.command.CMD_ACQUIRE_SKILL_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_ACQUIRE_SKILL_RSP, typeof(gsproto.AcquireSkillRsp));

            _typeToRequestCommand.Add(typeof(gsproto.EquipSkillReq), (uint)gsproto.command.CMD_EQUIP_SKILL_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_EQUIP_SKILL_RSP, typeof(gsproto.EquipSkillRsp));

            _typeToRequestCommand.Add(typeof(gsproto.ReceiveTaskReq), (uint)gsproto.command.CMD_RECEIVE_TASK_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_RECEIVE_TASK_RSP, typeof(gsproto.ReceiveTaskRsp));

            _typeToRequestCommand.Add(typeof(gsproto.RefuseTaskReq), (uint)gsproto.command.CMD_REFUSE_TASK_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_REFUSE_TASK_RSP, typeof(gsproto.RefuseTaskRsp));

            _typeToRequestCommand.Add(typeof(gsproto.DrawTaskAwardReq), (uint)gsproto.command.CMD_DRAW_TASK_AWARD_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_DRAW_TASK_AWARD_RSP, typeof(gsproto.DrawTaskAwardRsp));

            _typeToRequestCommand.Add(typeof(gsproto.GetDailyTaskReq), (uint)gsproto.command.CMD_GET_DAILY_TASK_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_GET_DAILY_TASK_RSP, typeof(gsproto.GetDailyTaskRsp));

            _typeToRequestCommand.Add(typeof(gsproto.GetWeeklyTaskReq), (uint)gsproto.command.CMD_GET_WEEKLY_TASK_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_GET_WEEKLY_TASK_RSP, typeof(gsproto.GetWeeklyTaskRsp));

            _typeToRequestCommand.Add(typeof(gsproto.BeginExpeditionReq), (uint)gsproto.command.CMD_BEGIN_EXPEDITION_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_BEGIN_EXPEDITION_RSP, typeof(gsproto.BeginExpeditionRsp));

            _typeToRequestCommand.Add(typeof(gsproto.EndExpeditionReq), (uint)gsproto.command.CMD_END_EXPEDITION_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_END_EXPEDITION_RSP, typeof(gsproto.EndExpeditionRsp));

            _typeToRequestCommand.Add(typeof(gsproto.CancelExpeditionReq), (uint)gsproto.command.CMD_CANCEL_EXPEDITION_REQ);
            _responseCommandToType.Add((uint)gsproto.command.CMD_CANCEL_EXPEDITION_RSP, typeof(gsproto.CancelExpeditionRsp));
        }

        public static void SendRequest(ProtocolDataType protocolType, object request, int resendCount = 0)
        {
            NetworkReqData reqData = new NetworkReqData();

            reqData.protocolType = protocolType;
            reqData.Request = request;
            reqData.resendCount = resendCount;
            reqData.sequence = NewSequence;

            _networkThread.PutRequestData(reqData);
        }

        public delegate void ProtocolCallback(ushort result, object response, object request);
        public static void RegisterHandler(uint responseCommand, ProtocolCallback callback)
        {
            if (!_callback.ContainsKey(responseCommand))
            {
                _callback.Add(responseCommand, null);
            }

            _callback[responseCommand] = (ProtocolCallback)_callback[responseCommand] + callback;
        }
        public static void UnregisterHandler(uint responseCommand, ProtocolCallback callback)
        {
            if (_callback.ContainsKey(responseCommand))
            {
                _callback[responseCommand] = (ProtocolCallback)_callback[responseCommand] - callback;
            }
        }

        static void Dispatch(DispatchData data)
        {
            if (_callback.ContainsKey(data.responseCommand))
            {
                ProtocolCallback callback = _callback[data.responseCommand] as ProtocolCallback;
                if (callback != null) callback(data.result, data.response, data.request);
            }
            else
            {
                Debug.Log("Protocol has not callback, response command: " + data.responseCommand);
            }
        }

        /// <summary>
        /// MonoBehaviour's Update 
        /// </summary>
        public static void OnUpdate()
        {
            DispatchData data = null;
            while ((data = _networkThread.GetDispatchData())!= null)
            {
                if (data.result != 0)
                {
                    // show error message ? 
#if NETWORK_LOG
                    //NetworkManager.Log("[网络] 接收协议号: " + data.responseCommand + " 错误码: " + data.result);
                    _networkThread.AddLog("[网络] 接收协议号: " + data.responseCommand + " 错误码: " + data.result);
#endif
                    GUI_MessageManager.Instance.ShowErrorTip(data.result);
                }

                Dispatch(data);
            }

            _networkThread.CheckSendTimeout();

            if (_networkThread.HasSendFailedData())
            {
                // 暂时给个提示吧，有消息对话框再做详细处理，比如提示重发什么的 
                //_networkThread.ResendFailedData();

                if (OnSendProtocolError != null)
                {
                    OnSendProtocolError();
                }

                _networkThread.ClearFailedData();

                GUI_MessageManager.Instance.ShowErrorTip("Protocol failed");
            }

#if NETWORK_LOG
            ShowLog();
#endif
        }

        public static void ShowLog()
        {

            LogData log = _networkThread.GetLog();
            while (log != null)
            {
                log.Show();
                log = _networkThread.GetLog();
            }
        }
    }
}
