// TODO：确保此文件中使用到的类型已经存在
public class GUI_HeroStarBar : UnityEngine.MonoBehaviour
{
    public int StarWidth = 0;
    public UnityEngine.RectTransform StarArea = null;
    public System.Collections.Generic.List<UnityEngine.GameObject> Stars = new System.Collections.Generic.List<UnityEngine.GameObject>();
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_HeroStarBar_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_HeroStarBar_DL>(gameObject, this);
#endif
    }
}