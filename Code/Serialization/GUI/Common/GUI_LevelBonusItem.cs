// TODO：确保此文件中使用到的类型已经存在
public class GUI_LevelBonusItem : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject ItemRoot = null;
    public UnityEngine.UI.Image ItemIcon = null;
    public UnityEngine.UI.Image SchoolIcon = null;
    public UnityEngine.GameObject ItemStarRoot = null;
    public UnityEngine.UI.Text ItemStarNum = null;
    public UnityEngine.UI.Text ItemCount = null;
    public UnityEngine.UI.Image BoxIcon = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_LevelBonusItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_LevelBonusItem_DL>(gameObject, this);
#endif
    }
}