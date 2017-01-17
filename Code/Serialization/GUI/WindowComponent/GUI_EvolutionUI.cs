using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_EvolutionUI : GUI_Window
{
    public GameObject CurrentHero;
    public GameObject EvolutionHero;

    public Button ConrimEvolutionButton;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_EvolutionUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_EvolutionUI_DL>(gameObject, this);
#endif
    }
}