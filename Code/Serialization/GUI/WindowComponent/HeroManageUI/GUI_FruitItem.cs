using UnityEngine;
using System.Collections;

public class GUI_FruitItem : GUI_TrainItem {

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_FruitItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_FruitItem_DL>(gameObject, this);
#endif
    }
}
