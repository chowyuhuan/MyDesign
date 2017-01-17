using UnityEngine;
using UnityEngine.UI;

public class GUI_RegimentUI : GUI_Window // !!!TODO:存在继承关系时，确保基类已经存在；如果继承链中没有序列化数据，去掉继承，改成MonoBehaviour
{
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject RegimentPassive = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public Text RegimentLevel = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public Text RegimentExp = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public Slider RegimentExpSchedule = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public Text RegimentName = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public Image RegimentIcon = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject RegimentPropertyPage = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public Text GoldCoin = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public Text Diamond = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public Text Stamina = null;
    public Text Hornor = null;
    public Text ArenaTicket = null;
    public Text Iron = null;
    public Text ExclusiveWeaponReset = null;
    public Text CrystalPowder = null;
    public Text CrystalPiece = null;
    public Text CrystalRime = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject RegimentPassivePage = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public GameObject RegimentPassiveSkillScollPage = null;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public RectTransform RegimentPassiveSkillArea = null;

    public Button RegimentLevelUpEffectButton = null;
    public Button RepresentHeroButton = null;
    public Button AddDiamondButton = null;
    public Button AddGoldCoinButton = null;
    public Button AddStaminaButton = null;
    public Button AddHornorButton = null;
    public Button AddArenaTicketButton = null;
    public Button AddIronButton = null;
    public Button AddWeaponResetTicketButton = null;
    public Button AddCrystalPowderButton = null;
    public Button AddCrystalPieceButton = null;
    public Button AddCrystalRimeButton = null;
    public GUI_ExtendToggleItem RegimentPassiveToggle = null;
    public GUI_ExtendToggleItem RegimentPropertyToggle = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_RegimentUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_RegimentUI_DL>(gameObject, this);
#endif
    }
}