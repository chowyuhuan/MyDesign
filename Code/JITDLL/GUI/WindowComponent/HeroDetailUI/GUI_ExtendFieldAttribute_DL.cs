using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_ExtendFieldAttribute_DL : MonoBehaviour
{
    #region field attribute logic
    public PbCommon.EHeroAttributeType FieldType = PbCommon.EHeroAttributeType.E_Hero_Attribute_Attack;
    public Text FieldText = null;
    public Color ExtendAttributeColor = Color.yellow;
    public Color ExtendAttributeMaxColor = Color.magenta;
    public GameObject AttributeProgressObject = null;

    GUI_MultipleStageSlider_DL MultpleSlider = null;

    public void RefreshAttribute(float currentValue, float addValue, float maxValue, float bigSuccessRate, float bigSuccessAppendPercent)
    {
        if (null == MultpleSlider)
        {
            MultpleSlider = AttributeProgressObject.GetComponent<GUI_MultipleStageSlider_DL>();
        }

        float lastCurrentValue = bigSuccessRate >= 100f ? (currentValue + addValue + addValue * bigSuccessAppendPercent) : (currentValue + addValue);
        Color currentAC;

        if(lastCurrentValue > maxValue)
        {
            currentAC = ExtendAttributeMaxColor;
        }
        else
        {
            currentAC = ExtendAttributeColor;
        }
        float appendValue = bigSuccessRate > 0f ? addValue * bigSuccessAppendPercent : 0f;
        FieldText.text = string.Format("{0}{1}",
            GUI_Tools.RichTextTool.Color(currentAC, lastCurrentValue.ToString()),
            GUI_Tools.RichTextTool.Color(ExtendAttributeColor, "/" + maxValue.ToString()));
        MultpleSlider.SetStageData(ESliderStage.Trible, maxValue, currentValue, addValue, appendValue);
    }

    public void GrowAttribute(float currentValue, float maxValue)
    {
        FieldText.text = string.Format("{0}{1}",
            GUI_Tools.RichTextTool.Color(ExtendAttributeColor, currentValue.ToString()),
            GUI_Tools.RichTextTool.Color(ExtendAttributeColor, "/" + maxValue.ToString()));
        MultpleSlider.SetStageData(ESliderStage.Trible, maxValue, currentValue, 0f, 0f);
    }
    #endregion

    #region jit init
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_ExtendFieldAttribute dataComponent = gameObject.GetComponent<GUI_ExtendFieldAttribute>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ExtendFieldAttribute,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            FieldType = (PbCommon.EHeroAttributeType)dataComponent.FieldType;
            FieldText = dataComponent.FieldText;
            ExtendAttributeColor = dataComponent.ExtendAttributeColor;
            ExtendAttributeMaxColor = dataComponent.ExtendAttributeMaxColor;
            AttributeProgressObject = dataComponent.AttributeProgressObject;
        }
    }
    #endregion
}
