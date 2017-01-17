using UnityEngine;
using System.Collections;

public class GameLogicController_DL : MonoBehaviour
{

    public static GameLogicController_DL Instance
    {
        get;
        protected set;
    }

    public AssetUpdate.AU_VersionInfo ClientVersion
    {
        get;
        protected set;
    }

    void Awake()
    {
        CopyDataFromDataScript();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitManagers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GUI_Manager.Instance.ShowWindowWithName("LoginUI", false);
    }

    void Update()
    {
        AssetManage.AM_LoadOperationManager.Update();
    }

    void InitManagers()
    {
        #region resource and version init
        ClientVersion = AU_UpdateMessage.Instance.Version;
        AssetManage.AM_Manager.Init(AU_UpdateMessage.Instance.InitFromRemote);
        AU_UpdateMessage.Instance.DestroyMessage();
        #endregion

        #region network and playerdata
        DataCenter.PlayerDataCenter.RegisterHandler();
        LoginHelper.RegisterHandler();
        Network.NetworkManager.Initialize();
        Platform.PlatformInterface.Setup();
        gameObject.AddComponent<SkillDataCenter>();
        #endregion

        #region managers or controllers
        TextLocalization.Init();
        GUI_Manager.Instance.Init();
        AudioManager.CreateInstance();
        CountdownUpdate.CreateInstance();
        #endregion
    }
    protected void CopyDataFromDataScript()
    {
        GameLogicController dataComponent = gameObject.GetComponent<GameLogicController>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GameLogicController,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
        }
    }
}
