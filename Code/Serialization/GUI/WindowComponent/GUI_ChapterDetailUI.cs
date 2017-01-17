using UnityEngine;
using System.Collections.Generic;
public class GUI_ChapterDetailUI : GUI_Window // !!!TODO:存在继承关系时，确保基类已经存在；如果继承链中没有序列化数据，去掉继承，改成MonoBehaviour
{
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject GridLayoutHelperObject = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public List<GameObject> TabPageObjectList = new List<GameObject>();
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public List<GameObject> BossInfoList = new List<GameObject>();
    public UnityEngine.UI.Button BeginButton;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ChapterDetailUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ChapterDetailUI_DL>(gameObject, this);
#endif
    }
}