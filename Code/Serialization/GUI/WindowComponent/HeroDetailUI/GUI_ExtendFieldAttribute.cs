using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_ExtendFieldAttribute : MonoBehaviour
{
    public int FieldType = 1;
    public Text FieldText = null;
    public Color ExtendAttributeColor = Color.yellow;
    public Color ExtendAttributeMaxColor = Color.magenta;
    public GameObject AttributeProgressObject = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ExtendFieldAttribute_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ExtendFieldAttribute_DL>(gameObject, this);
#endif
    }
}