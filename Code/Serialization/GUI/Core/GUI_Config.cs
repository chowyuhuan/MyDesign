using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum E_GameLevel_CostType
{
    Meat = 1,
    Key = 2,
}

public enum E_GameLevel_Difficulty
{
    Easy = 1,
    Normal = 2,
    Difficult = 4,
}

public enum E_ChapterType
{
    Plot = 1,
    Dungeon = 2,
    Bakery = 4,
    WorldBoss = 8,
    SoulFortress = 16,
}

public enum E_Hero_OrderType
{
    Attack = 0,//攻击力
    Hp = 1,//体力
    CritRatio = 2,//暴击率
    PhysicalDefend = 3,//物理防御
    MagicalDefend = 4,//魔法防御
    CritDamage = 5,//暴击伤害
    Precision = 6,//命中率
    Dodge = 7,//回避率
    StarNum = 8,//星级
    Level = 9,//等级
    TrainingLevel = 10,//训练
    Difficulty = 11,//入手
    Max
}

public enum E_Layout_Group
{
    Horizontal,
    Vertical,
    Grid
}

public enum ESliderStage
{
    Single = 1,
    Double = 2,
    Trible = 3,
}

public enum EWarnNumberType
{
    Cure = 1,
    Damage = 2,
    Crit = 3,
}

public class GUI_Const
{
    public const int Config_Type_Distance = 1000;
}

[System.Serializable]
public class GUI_Transform
{
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
}

[System.Serializable]
public class GUI_ActorSimpleInfo
{
    public Image HeadIcon;
    public Image SchoolIcon;
    public Text LevelText;
    public Text StarText;
}

[System.Serializable]
public class GUI_EquipSimpleInfo
{
    public Image EquipIcon;
    public Image SchoolIcon;
    public Text ReformText;
    public Text StarText;
}

[System.Serializable]
public class GUI_EquipBasePropertyInfo
{
    public Text Name;
    public Text Description;
}

[System.Serializable]
public class GUI_EquipWordPropertyInfo
{
    public GameObject WordRootObject;
    public GameObject DescriptionRootObject;
    public Text WordDescription;
    public Text ReformText;
    public GameObject WorldLevlRootObject;
    public Text WordLevelText;
    public Text RemindText;
}

[System.Serializable]
public class GUI_EquipWordReformInfo : GUI_EquipWordPropertyInfo
{
    public Text ReformCost;
    public Button ReformButton;
    public Button ResetCountButton;
}

[System.Serializable]
public class GUI_SkillSimpleInfo
{
    public Image Icon;
    public Text Level;
}

[System.Serializable]
public class GUI_ItemShrinkInfo
{
    public float ItemExtendRate = 0.05f;
    public float ItemShrinkRate = 0.05f;
    public float ShrinkHeight = 120f;
    public float ExtendHeight = 360f;
    public bool ExtendingItem = false;
    public bool ShrinkingItem = false;
    public GameObject ShrinkTarget = null;
}

[System.Serializable]
public class GUI_CommonTaskInfo
{
    public Image TaskIcon;
    public Image TaskBgIcon;
    public Text StateText;
    public Text ScheduleText;
    public uint TaskIndex;
    public Button TaskButton;
    public Text ColdTime;
    public Image ColdMask;
}

[System.Serializable]
public class GUI_ItemSimpleInfo
{
    public Image Icon;
    public Text Name_Star_Count;
}

[System.Serializable]
public class GUI_RootedItemSimpleInfo
{
    public GUI_ItemSimpleInfo Item;
    public GameObject InfoRoot;
    public GameObject BgRoot;
}

[System.Serializable]
public class GUI_ItemBgUnit
{
    public Image Item;
    public Image Bg;
}

[System.Serializable]
public class GUI_ExpeditionRandomAwardBox
{
    public GameObject RootObject;
    public GUI_ItemBgUnit AwardBox;
    public Toggle MaskToggle;
    public Toggle CheckMark;
    public Text ConditionCount;
}

[System.Serializable]
public class GUI_ExpeditionRandomAwardCondition
{
    public GUI_ItemSimpleInfo Condition;
    public GameObject CheckMark;
    public Button ConditionButton;
}

[System.Serializable]
public class GUI_ColorfulSlider
{
    public Slider Slider;
    public Image ProgressImage;
}

public delegate int CompareComponent(int item1Index, int item2Index);//0:item1 = item2, -1:item1 < item2, 1: item1 > item2