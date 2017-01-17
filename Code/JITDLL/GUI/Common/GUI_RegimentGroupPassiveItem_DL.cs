using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_RegimentGroupPassiveItem_DL : GUI_LogicObject
{
    public Image DisplayIcon;
    public Image EffectBuffIcon;
    public Text LevelText;
    public Text Name;
    public Image EffectDesIcon;
    public Text EffectDescription;
    public Text ConditionText;
    public Text ConditionScheduleText;
    public Slider ConditionSchedule;
    public GameObject LockMask;

    CSV_c_hero_group_effect_template _GroupEffectTemplate;

    public void SetGroupPassvie(DataCenter.GroupPassiveInfo passiveInfo)
    {
        int combinedId = (int)(passiveInfo.PassiveType * GUI_Const.Config_Type_Distance + passiveInfo.PassiveLevel);
        _GroupEffectTemplate = CSV_c_hero_group_effect_template.FindData(combinedId);
        if (null != _GroupEffectTemplate)
        {
            GUI_Tools.IconTool.SetIcon(_GroupEffectTemplate.DisplayIconAtlas, _GroupEffectTemplate.DisplayIconSprite, DisplayIcon);
            GUI_Tools.IconTool.SetIcon(_GroupEffectTemplate.EffectBuffAtlas, _GroupEffectTemplate.EffectBuffSprite, EffectBuffIcon);
            LevelText.text = _GroupEffectTemplate.LevelText;
            Name.text = _GroupEffectTemplate.Name;
            GUI_Tools.IconTool.SetIcon(_GroupEffectTemplate.DescriptionAtlas, _GroupEffectTemplate.DescriptionSrpite, EffectDesIcon);
            EffectDescription.text = _GroupEffectTemplate.Description;
            ConditionText.text = _GroupEffectTemplate.Condition;
            ConditionScheduleText.text = string.Format("{0}/{1}", passiveInfo.CurrentCount.ToString(), passiveInfo.ConditionCount.ToString());
            ConditionSchedule.value = Mathf.Clamp01((float)passiveInfo.CurrentCount / (float)passiveInfo.ConditionCount);
            LockMask.SetActive(passiveInfo.CurrentCount < passiveInfo.ConditionCount);
        }
    }

    protected override void OnRecycle()
    {

    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_RegimentGroupPassiveItem dataComponent = gameObject.GetComponent<GUI_RegimentGroupPassiveItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_RegimentGroupPassiveItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            DisplayIcon = dataComponent.DisplayIcon;
            EffectBuffIcon = dataComponent.EffectBuffIcon;
            LevelText = dataComponent.LevelText;
            Name = dataComponent.Name;
            EffectDesIcon = dataComponent.EffectDesIcon;
            EffectDescription = dataComponent.EffectDescription;
            ConditionText = dataComponent.ConditionText;
            ConditionScheduleText = dataComponent.ConditionScheduleText;
            ConditionSchedule = dataComponent.ConditionSchedule;
            LockMask = dataComponent.LockMask;
        }
    }
}
