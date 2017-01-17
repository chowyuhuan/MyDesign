using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_ResolveAwardItem : MonoBehaviour {
    public Image ItemIcon;
    public Text ItemCount;
    public GameObject AdditionalIcon;
    public string IronCountFormater;
    public string CrystalPowderCountFormater;
    public string CrystalPieceCountFormater;
    public string CrystalRimeCountFormater;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ResolveAwardItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ResolveAwardItem_DL>(gameObject, this);
#endif
    }
}
