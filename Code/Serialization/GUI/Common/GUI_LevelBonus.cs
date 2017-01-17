using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public abstract class GUI_LevelBonus : GUI_Window
{
    public List<GameObject> HeroInfoList = new List<GameObject>();
    public Text LevelName;
    public GUI_Transform HeroTransTemplate;
    public string HeroAction;

    public Text GoldCoin;
    int _GoldCoinCount;
    public float HeroExpSimulateRate = 0.02f;
    public GameObject RegimentLevelUpRoot;
    public Text RegimentGroupLevel;
    public Slider RegimentGroupExp;
    public List<GameObject> BonusItemList = new List<GameObject>();
    public Text TaskSchedule;
    public Text TaskFinish;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_LevelBonus_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_LevelBonus_DL>(gameObject, this);
#endif
    }
}
