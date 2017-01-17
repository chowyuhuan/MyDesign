// TODO：确保此文件中使用到的类型已经存在
public class GUI_WarnNumber : UnityEngine.MonoBehaviour
{
    public UnityEngine.Rect _DisplayArea;
    public float _UpMove = 0;
    public System.Collections.Generic.List<UnityEngine.SpriteRenderer> _ImageNumList = new System.Collections.Generic.List<UnityEngine.SpriteRenderer>();
    public string _NumberAtlasName = null;
    public string _NumberPrefix = null;
    public GUI_TweenAlpha _AlphaTweener = null;
    public GUI_TweenScale _ScaleTweener = null;
    public GUI_TweenPosition _PositionTweener = null;
    public float HeapUpDistance = 0.02f;
    public float HeapFrontDistance = -0.05f;
    public float BaseDepth = -1f;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_WarnNumber_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_WarnNumber_DL>(gameObject, this);
#endif
    }
}