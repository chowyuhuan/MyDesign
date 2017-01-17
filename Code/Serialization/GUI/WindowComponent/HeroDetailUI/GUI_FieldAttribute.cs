using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_FieldAttribute : MonoBehaviour
{
    public int FieldType;
    public Text FieldText;
    public Text ExtendAttributeText;
    public Color ExtendAttributeColor = Color.green;
    public Slider AttributeProgress;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_FieldAttribute_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_FieldAttribute_DL>(gameObject, this);
#endif
    }
}