// TODO：确保此文件中使用到的类型已经存在
public class GUI_HeroDCInfo : UnityEngine.MonoBehaviour
{
public UnityEngine.UI.Text _NameText = null;
public UnityEngine.UI.Text _DamageText = null;
public UnityEngine.UI.Text _CureText = null;
void Awake()
{
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_HeroDCInfo_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
ScriptAssembly.Assemble<GUI_HeroDCInfo_DL>(gameObject, this);
#endif
}
}