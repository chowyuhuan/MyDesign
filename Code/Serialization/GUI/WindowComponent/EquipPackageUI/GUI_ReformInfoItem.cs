using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_ReformInfoItem : MonoBehaviour {
    public Text RequierdHeroGroupLevel = null;
    public Text WeaponStarRange = null;
    public Text ReformInfoText = null;
    public GameObject LockMask = null;
    public Button UnLockButton = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ReformInfoItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ReformInfoItem_DL>(gameObject, this);
#endif
    }
}
