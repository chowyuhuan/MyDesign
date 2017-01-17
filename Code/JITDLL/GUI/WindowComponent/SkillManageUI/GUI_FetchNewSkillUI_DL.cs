using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_FetchNewSkillUI_DL : GUI_Window_DL
{

    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_FetchNewSkillUI dataComponent = gameObject.GetComponent<GUI_FetchNewSkillUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_FetchNewSkillUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            Icon = dataComponent.Icon;
            Name = dataComponent.Name;
            Description = dataComponent.Description;
        }
    }
    #endregion

    #region window logic
    public Image Icon;
    public Text Name;
    public Text Description;

    CSV_b_skill_template SkillTemplate;

    public void ShowNewSkill(DataCenter.SpecialSkillUnit skillUnit)
    {
        SkillTemplate = CSV_b_skill_template.FindData(skillUnit.ShowSkillId);
        if (null == SkillTemplate)
        {
            HideWindow();
        }
    }

    protected override void OnStart()
    {
        Name.text = SkillTemplate.Name;
        Description.text = SkillTemplate.Description;
        GUI_Tools.IconTool.SetSkillIcon(SkillTemplate.Id, Icon);
    }
    #endregion
}
