using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_FruitUI : GUI_Window {
    public Text BigSuccessText = null;
    public Slider BigSuccessValue = null;
    public List<Button> SelectedFruitIconButtonList = new List<Button>();
    public List<Image> SelectedFruitIconList = new List<Image>();
    public List<GameObject> FieldAttributes = new List<GameObject>();
    public GameObject LayoutHelperObject = null;
    public List<GameObject> FruitTabPageObjectList = new List<GameObject>();
    public Text SellPrice = null;
    public Text EatPrice = null;
    public Button EatButton = null;
    public Button SellButton = null;
    public GameObject SellControlArea = null;
    public Button CancelSellButton = null;
    public Button ConfirmSellButton = null;
    public GameObject ForbiddenEatRoot = null;
    public Text ForbiddenEatText = null;
    public Button ExtendFruitPackageButton = null;
    public Text FruitBagVolText = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_FruitUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_FruitUI_DL>(gameObject, this);
#endif
    }
}
