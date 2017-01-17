using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_HeroManage : GUI_Window // !!!TODO:存在继承关系时，确保基类已经存在；如果继承链中没有序列化数据，去掉继承，改成MonoBehaviour
{
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public Text HeroCount = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public List<GameObject> HeroTabPageList = new List<GameObject>();
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public List<GameObject> HeroDisplayList = new List<GameObject>();
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject GridLayoutHelper = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject OrderMenu = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public List<GameObject> HeroOrderMenuItems = new List<GameObject>();
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public Text CurrentSelectOrderType = null;
    public bool DecreaseOrder = true;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public RectTransform OrderDirectonIcon = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject ManageButtonArea = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject FireButtonArea = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject RegimentLeaderHero = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public Text FireHonor = null;

    public Button ExtendHeroBagButton;
    public Button OrderDirectionButton;
    public Button OrderButton;
    public Button CreateTeamButton;
    public Button SetRepresentHero;
    public Button FireHeroButton;
    public Button ConfirmSetRepresentHeroButton;
    public Button CancelFireHeroButton;
    public Button ConfirmFireHeroButton;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_HeroManage_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_HeroManage_DL>(gameObject, this);
#endif
    }
}