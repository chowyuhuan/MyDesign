// TODO：确保此文件中使用到的类型已经存在
public class GUI_MonsterWave : UnityEngine.MonoBehaviour
{
    public UnityEngine.UI.Text _PreNumber = null;
    public UnityEngine.UI.Text _PostNumber = null;
    public GUI_TweenPosition _TweenIn = null;
    public GUI_TweenPosition _TweenOut = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_MonsterWave_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_MonsterWave_DL>(gameObject, this);
#endif
    }
}