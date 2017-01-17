using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_HeroGetNewSpecialSkillUI_DL : GUI_Window_DL {
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_HeroGetNewSpecialSkillUI dataComponent = gameObject.GetComponent<GUI_HeroGetNewSpecialSkillUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeroGetNewSpecialSkillUI,GameObject：" + gameObject.name, gameObject);
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

    public void ShowNewSkill(CSV_b_skill_template skillTemplate)
    {
        SkillTemplate = skillTemplate;
        if (null == SkillTemplate)
        {
            HideWindow();
        }
    }

    protected override void OnStart()
    {
        GUI_Tools.IconTool.SetSkillIcon(SkillTemplate.Id, Icon);
        Name.text = SkillTemplate.Name;
        Description.text = SkillTemplate.Description;
    }
    #endregion

}
