using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_SkillCastWarn_DL : MonoBehaviour
{
    public SKILL.Camp _WarnCamp;
    public Text _WarnTitle;
    public Text _Description;
    public GUI_TweenPosition _SlideInTweener;
    public GUI_TweenPosition _SlideOutTweener;
    public RectTransform _SkillNameTransform;

    public void OnSkillCastStart(Actor actor, SKILL.Skill skill)
    {
        if (null != actor && null != skill)
        {
            CSV_c_skill_description skillDes = CSV_c_skill_description.FindData(skill.ID);
            if (skillDes != null)
            {
                CSV_c_skill_cast_warn_pattern warnPattern = CSV_c_skill_cast_warn_pattern.FindData(skillDes.WarnPatternID);
                if (null != warnPattern)
                {
                    if (warnPattern.NameAsTitle == 1)
                    {
                        _WarnTitle.text = GUI_Tools.RichTextTool.Color(warnPattern.TitleColor, skillDes.Name);
                    }
                    else
                    {
                        _WarnTitle.text = GUI_Tools.RichTextTool.Color(warnPattern.TitleColor, warnPattern.Title);
                    }

                    if (warnPattern.IntentAsDescription == 1)
                    {
                        _Description.text = GUI_Tools.RichTextTool.Color(warnPattern.DescriptionColor, skillDes.IntentEffect);
                    }
                    else
                    {
                        _Description.text = GUI_Tools.RichTextTool.Color(warnPattern.TitleColor, warnPattern.Description);
                    }

                    _SlideInTweener.ResetToBeginning();
                    _SlideOutTweener.ResetToBeginning();

                    _SlideInTweener.PlayForward();
                    _SlideOutTweener.PlayForward();

                    if (warnPattern.ShowHeadUpTip == 1)
                    {
                        GUI_BattleManager.Instance.BattleUI.ShowHeadUpWarnTip(actor, skillDes, warnPattern);
                    }
                }
            }
        }
    }

    void OnEnable()
    {
        GUI_BattleManager.Instance.RegistSkillCastWarn(_WarnCamp, this);
    }

    void OnDisable()
    {
        GUI_BattleManager.Instance.UnRegistSkillCastWarn(_WarnCamp);
    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_SkillCastWarn dataComponent = gameObject.GetComponent<GUI_SkillCastWarn>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_SkillCastWarn,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            _WarnCamp = (SKILL.Camp)dataComponent._WarnCamp;
            _WarnTitle = dataComponent._WarnTitle;
            _Description = dataComponent._Description;
            _SlideInTweener = dataComponent._SlideInTweener;
            _SlideOutTweener = dataComponent._SlideOutTweener;
            _SkillNameTransform = dataComponent._SkillNameTransform;
        }
    }
}
