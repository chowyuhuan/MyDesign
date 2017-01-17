using UnityEngine.UI;

// TODO：确保此文件中使用到的类型已经存在
public class GUI_BakeryItem : UnityEngine.MonoBehaviour
{
    public UnityEngine.UI.Text Name = null;
    public UnityEngine.UI.Image IconImage = null;
    public UnityEngine.UI.Text Product = null;
    public UnityEngine.UI.Text TimeDesciption = null;
    public UnityEngine.UI.Text BakeScheduleCount = null;
    public UnityEngine.UI.Slider BakeSchedule = null;
    public UnityEngine.UI.Text BakeScheduleText = null;
    public UnityEngine.GameObject LockMask = null;
    public UnityEngine.GameObject BakeDone = null;
    public UnityEngine.GameObject Baking = null;

    public Button ItemButton;
    public Button FinishImmediateButton;
    public Button AbortButton;
    public Button GetBakeryProductButton;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_BakeryItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_BakeryItem_DL>(gameObject, this);
#endif
    }
}