using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_ReformSuccess_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ReformSuccessUI dataComponent = gameObject.GetComponent<GUI_ReformSuccessUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ReformSuccessUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            EquipName = dataComponent.EquipName;
            EquipIcon = dataComponent.EquipIcon;
            StarBar = dataComponent.StarBar;
            ReformWordProperty = dataComponent.ReformWordProperty;
        }
    }
    #endregion

    #region window logic
    DataCenter.Equip Equip = null;
    Text EquipName;
    Image EquipIcon;
    GameObject StarBar;
    Text ReformWordProperty;
    CSV_b_equip_template EquipTemplate;
    uint ReformIndex = 0;

    public void ShowReformProperty(DataCenter.Equip equip, uint reformIndex)
    {
        if(null != equip)
        {
            Equip = equip;
            ReformIndex = reformIndex;
            DataCenter.EquipReform equipReform = equip.GetEquipReform(reformIndex);
            if(null != equipReform)
            {
                EquipTemplate = CSV_b_equip_template.FindData(equip.CsvId);                
            }
            else
            {
                GUI_MessageManager.Instance.ShowErrorTip("Not exist equip property !!");
                HideWindow();
            }
        }
        else
        {
            HideWindow();
        }
    }

    protected override void OnStart()
    {
        if(null != EquipTemplate)
        {
            EquipName.text = EquipTemplate.Name;
            GUI_Tools.IconTool.SetIcon(EquipTemplate.IconAtlas, EquipTemplate.IconSprite, EquipIcon);
            GUI_HeroStarBar_DL starBar = StarBar.GetComponent<GUI_HeroStarBar_DL>();
            if(null != starBar)
            {
                starBar.SetStarNum(EquipTemplate.Star);
            }
            DataCenter.EquipReform reformProperty = Equip.GetEquipReform(ReformIndex);
            string propertyFormater = GUI_Tools.TextTool.GetReformTextFormater((PbCommon.EPropertyType)reformProperty.ReformProperty);
            ReformWordProperty.text = string.Format(propertyFormater, reformProperty.ReformValue);
        }
    }
    #endregion
}
