using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_HeroEquipDisplayItem : GUI_ToggleItem {
    public float ItemExtendRate = 0.05f;
    public float ItemShrinkRate = 0.05f;
    public float ShrinkHeight = 120f;
    public float ExtendHeight = 220f;

    public GUI_EquipSimpleInfo EquipSimpleInfo = null;
    public Text EquipName = null;
    public Image HeroIcon = null;
    public GameObject HeroStar = null;
    public Text ExclusiveText = null;

    public GUI_EquipBasePropertyInfo AttackProperty = null;
    public GUI_EquipBasePropertyInfo AttackSpeedProperty = null;

    public GameObject WordPropertyRoot;
    public GameObject ExtendBagRoot;
    public Button ExtendBagButton;
    public Text NonWordPropertyTip;

    public Color NonBigSuccessColor = Color.white;
    public Color BigSuccessColor = Color.white;
    public List<GUI_EquipWordPropertyInfo> EquipWordInfoList = new List<GUI_EquipWordPropertyInfo>();
    public GameObject ExclusiveWordLoopObject = null;

    public Text EquipLockButtonText = null;
    public Button EquipLockButton;
    public Button DismissEquip = null;
    public Text DismissButtonText = null;
    public Button ReformEquip = null;
    public Button RefineEquip = null;
    public Text RefineEquipText = null;
    public Text RefineEquipRemind = null;
    public GameObject LockMask = null;
    public GameObject LockIcon = null;
    public Button LockMaskButton = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_HeroEquipDisplayItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_HeroEquipDisplayItem_DL>(gameObject, this);
#endif
    }
}
