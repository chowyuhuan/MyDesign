using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_GoddessSkill_DL : MonoBehaviour
{
    #region angle logic
    public Image _HeadIcon;
    public Slider _SPSlider;
    protected int _CurSP;
    protected int _MaxSP;
    protected int _CostSP;
    CSV_c_goddess_config _Goddess;
    float ColdCounting = 0f;
    bool CoolDownSkill = false;

    void Start()
    {
        int maxSp = DefaultConfig.GetInt("GoddessSkillCount");
        int costSp = DefaultConfig.GetInt("GoddessSkillSp");
        _MaxSP = maxSp * costSp;
        _CurSP = 0;
        _CostSP = costSp;
        _SPSlider.value = 0f;

        Init();        
    }

    DataCenter.TeamInfo GetAdventureTeam()
    {
        DataCenter.TeamInfo teamInfo = null;
        switch ((E_ChapterType)GUI_BattleManager.Instance.SelectedChapter.ChapterType)
        {
            case E_ChapterType.Plot:
                {
                    teamInfo = DataCenter.PlayerDataCenter.TeamCollectionData.GetMissionTeam();
                    break;
                }
        }
        return teamInfo;
    }

    IEnumerator RegisEvent()
    {
        while(null == BattleManager_DL.Instance || !BattleManager_DL.Instance.InitDone)
        {
            yield return null;
        }
        ACTOR.ActorManager.Instance.GetTeam(SKILL.Camp.Comrade).OnGoddessSpChange += UpdateAngleSP;
    }

    void Init()
    {
        DataCenter.TeamInfo teamInfo = GetAdventureTeam();
        if (null != teamInfo)
        {
            _Goddess = CSV_c_goddess_config.FindData((int)teamInfo.Goddess);
            GUI_Tools.ObjectTool.ActiveObject(gameObject, null != _Goddess);
            if(null != _Goddess)
            {
                GUI_Tools.IconTool.SetIcon(_Goddess.HeadIconAtlas, _Goddess.HeadIcon, _HeadIcon);
                StartCoroutine(RegisEvent());
            }
        }
        else
        {
            GUI_Tools.ObjectTool.ActiveObject(gameObject, false);
        }
    }

    public void UpdateAngleSP(int sp)
    {
        _CurSP = Mathf.Clamp(sp, 0, _MaxSP);
        _SPSlider.value = (float)_CurSP / _MaxSP;
    }

    void TryUseAngleSkill()
    {
        if (EnoughSP())
        {
            if(!CoolDownSkill)
            {
                GUI_MessageManager.Instance.ShowErrorTip("<color=green>GoddessSkill !!!!!!!!!!</color>");
                ACTOR.ActorManager.Instance.GetTeam(SKILL.Camp.Comrade).CastGoddessSkill();
                StartCoroutine("CoolDown");
            }
            else
            {
                GUI_MessageManager.Instance.ShowErrorTip("<color=red>Skill Cool Down!!</color>");
            }
        }
        else
        {
            GUI_MessageManager.Instance.ShowErrorTip("<color=red>Not enough sp!!</color>");
        }
    }

    bool EnoughSP()
    {
        return _CurSP >= _CostSP;
    }

    IEnumerator CoolDown()
    {
        CoolDownSkill = true;
        ColdCounting = _Goddess.ColdTime;
        while(ColdCounting > 0f)
        {
            ColdCounting -= GameTimer.deltaTime;
            yield return null;
        }
        ColdCounting = 0f;
        CoolDownSkill = false;
    }

    void OnDisable()
    {
        if(CoolDownSkill)
        {
            StopCoroutine("CoolDown");
        }
    }
    #endregion

    #region jit init
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_GoddessSkill dataComponent = gameObject.GetComponent<GUI_GoddessSkill>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_GoddessSkill,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            _HeadIcon = dataComponent.HeadIcon;
            _SPSlider = dataComponent.SPSlider;
            dataComponent.SkillButton.onClick.AddListener(TryUseAngleSkill);
        }
    }
    #endregion
}
