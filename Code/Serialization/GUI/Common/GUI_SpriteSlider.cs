// TODO：确保此文件中使用到的类型已经存在
public class GUI_SpriteSlider : UnityEngine.MonoBehaviour
{
    public UnityEngine.SpriteRenderer FillSprite = null;
    public float _Value = 1f;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_SpriteSlider_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_SpriteSlider_DL>(gameObject, this);
#endif
    }
}