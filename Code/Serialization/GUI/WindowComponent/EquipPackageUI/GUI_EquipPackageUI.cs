using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class GUI_EquipPackageUI : GUI_Window // !!!TODO:存在继承关系时，确保基类已经存在；如果继承链中没有序列化数据，去掉继承，改成MonoBehaviour
{
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public List<GameObject> HeroTabPageObjectList = new List<GameObject>();
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject GroupLayoutHelperObject = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject DefaultEquipDisplayObject = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject ResolveButtonObject = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public Text EquipHoldText = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public Text EquipBagVolume = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject ControlButtonArea = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject SellButtonArea = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject ResolveButtonArea = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public Text SellPrice = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public Text ResolveMat = null;

    public GameObject EquipScrollPageObject;

    public GameObject WeaponPageToggleObject;
    public GameObject RingPageToggleObject;
    public Button ExtendEquipBagButton;
    public Button SellButton;
    public Button ResolveButton;
    public Button CancelSellButton;
    public Button ConfirmSellButton;
    public Button SellAllButton;
    public Button CancelResolveButton;
    public Button ConfirmResolveButton;
    public Button ResolveAllButton;

    public Button ReformInfoButton;
    public Button RingInfoButton;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_EquipPackageUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_EquipPackageUI_DL>(gameObject, this);
#endif
    }
}