using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GUI_HeroBonusSimpleInfo_DL : MonoBehaviour
{
    public Text Name;
    public Image DisplayIcon;
    public Text Level;
    public Color MaxLevelColor;
    public Color CurrentLevelColor;
    public Text Expierence;
    public Slider ExpSlider;
    public GameObject LevelUpIcon;
    DataCenter.HeroAddExpInfo _AddExpInfo;
    float _ExpSimulateRate;
    CSV_b_hero_template _HeroTemplate;
    CSV_b_hero_limit _HeroLimit;
    Action _OnExpSimulateEnd;

    public void SetHeroInfo(DataCenter.HeroAddExpInfo expInfo, GUI_Transform heroTrans, string heroAction, float expSimulateRate)
    {
        _AddExpInfo = expInfo;
        _ExpSimulateRate = expSimulateRate;
        LevelUp(false);
        if (_AddExpInfo != null)
        {
            _HeroTemplate = CSV_b_hero_template.FindData(_AddExpInfo.HeroData.CsvId);
            _HeroLimit = CSV_b_hero_limit.FindData(_HeroTemplate.Star);
            if (null == _HeroTemplate)
            {
                ResetInfo();
            }
            else
            {
                DisplayModel(heroTrans, heroAction);
            }
            SetStartInfo();
        }
        else
        {
            ResetInfo();
        }
    }

    public void SimulateExpUp(Action onExpSimulateEnd)
    {
        _OnExpSimulateEnd = onExpSimulateEnd;
        if (null != _AddExpInfo)
        {
            StartCoroutine("GrowUpExp");
        }
        else if (null != _OnExpSimulateEnd)
        {
            SetEndInfo();
            _OnExpSimulateEnd();
        }
    }

    IEnumerator GrowUpExp()
    {
        int currentLevel = (int)_AddExpInfo.StartLevel;
        int currentExp = (int)_AddExpInfo.StartExp;
        int endExp = (int)_AddExpInfo.EndExp;
        int totalAddExp = (int)(_AddExpInfo.EndExp - _AddExpInfo.StartExp);
        int currentLevelExp = CSV_b_level_template.GetCurrentLevelExp(currentExp, currentLevel);
        CSV_b_level_template curLevelTemplate = CSV_b_level_template.FindData(currentLevel);
        while (currentExp < endExp)
        {
            float growExp = totalAddExp * _ExpSimulateRate;
            currentExp += (int)growExp;
            if (currentExp > endExp)
            {
                currentExp = endExp;
            }
            if (currentExp > curLevelTemplate.Exp)
            {
                LevelUp(true);
                ++currentLevel;
                if (currentLevel > _HeroLimit.MaxLevel)
                {
                    currentLevel = _HeroLimit.MaxLevel;
                }
                curLevelTemplate = CSV_b_level_template.FindData(currentLevel);
            }
            currentLevelExp = CSV_b_level_template.GetCurrentLevelExp(currentExp, currentLevel);
            int curLevelUpExp = CSV_b_level_template.GetLevelGrowExp(currentLevel);
            Expierence.text = string.Format("{0}/{1}", currentLevelExp.ToString(), curLevelUpExp.ToString());
            ExpSlider.value = Mathf.Clamp01((float)currentLevelExp / (float)curLevelUpExp);
            yield return null;
        }
        SetEndInfo();
        if (null != _OnExpSimulateEnd)
        {
            _OnExpSimulateEnd();
            _OnExpSimulateEnd = null;
        }
    }

    void LevelUp(bool levelUp)
    {
        LevelUpIcon.SetActive(levelUp);
    }

    void DisplayModel(GUI_Transform heroTrans, string heroAction)
    {
        GameObject heroModel;
        if (GUI_Tools.ModelTool.SpawnModel(DisplayIcon.gameObject, _HeroTemplate.Prefab, heroTrans, out heroModel))
        {
            Animator anim;
            if (GUI_Tools.ModelTool.AnimateUIModel(heroModel, _HeroTemplate.UiAnimCtrl, out anim))
            {
                anim.Play(heroAction);
            }

            ActorWeaponInfo actorWeaponInfo = _AddExpInfo.HeroData.GetActorWeaponInfo();
            ActorWeaponHelper.SetActorWeapon(heroModel, ActorWeaponHelper.SpawnActorWeaponWithUIScale(DisplayIcon.gameObject.transform, heroTrans, actorWeaponInfo), actorWeaponInfo);
        }
    }

    void SetStartInfo()
    {
        Name.text = _HeroTemplate.Name;

        Level.text = GUI_Tools.RichTextTool.Color(CurrentLevelColor, _AddExpInfo.StartLevel.ToString() + "/") + GUI_Tools.RichTextTool.Color(MaxLevelColor, _HeroLimit.MaxLevel.ToString());

        int currentExp = CSV_b_level_template.GetCurrentLevelExp((int)_AddExpInfo.StartExp, (int)_AddExpInfo.StartLevel);
        int curLevelGrowExp = CSV_b_level_template.GetLevelGrowExp((int)_AddExpInfo.StartLevel);
        Expierence.text = currentExp.ToString() + "/" + curLevelGrowExp.ToString();
        ExpSlider.value = (float)currentExp / (float)curLevelGrowExp;
    }

    void SetEndInfo()
    {
        Level.text = GUI_Tools.RichTextTool.Color(CurrentLevelColor, _AddExpInfo.EndLevel.ToString() + "/") + GUI_Tools.RichTextTool.Color(MaxLevelColor, _HeroLimit.MaxLevel.ToString());

        int currentExp = CSV_b_level_template.GetCurrentLevelExp((int)_AddExpInfo.EndExp, (int)_AddExpInfo.EndLevel);
        int curLevelGrowExp = CSV_b_level_template.GetLevelGrowExp((int)_AddExpInfo.EndLevel);
        Expierence.text = currentExp.ToString() + "/" + curLevelGrowExp.ToString();
        ExpSlider.value = (float)currentExp / (float)curLevelGrowExp;
    }

    void ResetInfo()
    {
        Name.text = "";
        DisplayIcon.sprite = null;
        Level.text = "";
        Expierence.text = "";
    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_HeroBonusSimpleInfo dataComponent = gameObject.GetComponent<GUI_HeroBonusSimpleInfo>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeroBonusSimpleInfo,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            Name = dataComponent.Name;
            DisplayIcon = dataComponent.DisplayIcon;
            Level = dataComponent.Level;
            MaxLevelColor = dataComponent.MaxLevelColor;
            CurrentLevelColor = dataComponent.CurrentLevelColor;
            Expierence = dataComponent.Expierence;
            ExpSlider = dataComponent.ExpSlider;
            LevelUpIcon = dataComponent.LevelUpIcon;
        }
    }
}
