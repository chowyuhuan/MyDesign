// TODO：确保此文件中使用到的类型已经存在
public class GUI_ChapterItem : UnityEngine.MonoBehaviour
{
    public UnityEngine.UI.Text ChapterName = null;
    public UnityEngine.UI.Text ChaperDesciption = null;
    public UnityEngine.UI.Image ChapterContentIcon = null;
    public UnityEngine.UI.Text Restriction = null;
    public UnityEngine.UI.Button EnterButton = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ChapterItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ChapterItem_DL>(gameObject, this);
#endif
    }
}