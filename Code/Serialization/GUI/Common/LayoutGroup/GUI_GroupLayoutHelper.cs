using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class GUI_GroupLayoutHelper : MonoBehaviour {
    public bool UseConfigCellSize;
    public Vector2 CellSize;
    public LayoutGroup LayoutComponent = null;
    public RectTransform ContentRect = null;
    public ScrollRect ScrollViewRect = null;
    public RectTransform ScrollCullRect = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_LoginUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_GroupLayoutHelper_DL>(gameObject, this);
#endif
    }
}
