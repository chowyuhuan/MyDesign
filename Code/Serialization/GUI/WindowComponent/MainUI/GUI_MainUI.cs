using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GUI_MainUI : GUI_Window // !!!TODO:存在继承关系时，确保基类已经存在；如果继承链中没有序列化数据，去掉继承，改成MonoBehaviour
{
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public UnityEngine.UI.Image RegimentIcon = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public UnityEngine.UI.Text RegimentLevel = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public UnityEngine.UI.Slider RegimentExpSlider = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public UnityEngine.UI.Text RegimentExp = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public UnityEngine.UI.Text RegimentName = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public UnityEngine.UI.Text Honor = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public UnityEngine.UI.Text Diamond = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public UnityEngine.UI.Text GoldCoin = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public UnityEngine.UI.Text Stamina = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public UnityEngine.UI.Text StaminaCounter = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GUI_TweenPosition CityMenuPosTweener = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GUI_TweenPosition AdventureMenuPosTweener = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public UnityEngine.UI.ScrollRect SlideBG = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public UnityEngine.RectTransform BGPos = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public UnityEngine.RectTransform CityMenuPos = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public UnityEngine.RectTransform SwichMenuPos = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public UnityEngine.RectTransform AdventureMenuPos = null;

    public Button RegimentIconButton = null;
    public Button HonorQuestionButton = null;
    public Button AddDiamondButton = null;
    public Button AddGoldCoinButton = null;
    public Button AddStaminaButton = null;

    public Button ShopButton = null;
    public Button FriendButton = null;
    public Button BakeryButton = null;
    public Button SkillButton = null;
    public Button EquipButton = null;
    public Button HeroManageButton = null;

    public Button AdverntureButton = null;
    public Button CityMenuButton = null;
    public Button PricticeButton = null;
    public Button ExpeditionButton = null;
    public Button SoulFortressButton = null;
    public Button WorldBossButton = null;

    public Button ShowRoleIdButton = null;//测试用，正式发布应该删除

    public GUI_TweenPosition LeftHiddenButtonTweener = null;
    public RectTransform LeftHiddenButtonArrow = null;
    public Button SwitchLeftHiddenButton = null;
    public Button AnnouncementButton = null;
    public Button SignInButton = null;
    public Button HelpButton = null;
    public Button GoddessButton = null;
    public Button SkinButton = null;
    public Button SettingButton = null;

    public Button DailyTaskButton = null;
    public List<GameObject> CommonTaskObjectList = new List<GameObject>();


    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_MainUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_MainUI_DL>(gameObject, this);
#endif
    }
}