using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_FieldAttribute_DL : MonoBehaviour
{
    public ACTOR.ActorField FieldType;
    public Text FieldText;
    public Text ExtendAttributeText;
    public Color ExtendAttributeColor = Color.green;
    public Slider AttributeProgress;

    public void SetPropertyValue(float baseAttributeValue, float extendAttributeValue)
    {
        if (extendAttributeValue == 0)
        {
            ExtendAttributeText.text = GUI_Tools.RichTextTool.Color(ExtendAttributeText.color, "(-)");
        }
        else
        {
            ExtendAttributeText.text = GUI_Tools.RichTextTool.Color(ExtendAttributeColor, string.Format("{0}{1}{2}", "(", extendAttributeValue.ToString(), ")"));
        }
        FieldText.text = baseAttributeValue.ToString();
    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_FieldAttribute dataComponent = gameObject.GetComponent<GUI_FieldAttribute>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_FieldAttribute,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            FieldType = (ACTOR.ActorField)dataComponent.FieldType;
            FieldText = dataComponent.FieldText;
            ExtendAttributeText = dataComponent.ExtendAttributeText;
            AttributeProgress = dataComponent.AttributeProgress;
        }
    }
}
