// TODO：确保此文件中使用到的类型已经存在
public class GUI_HeroBonusSimpleInfo : UnityEngine.MonoBehaviour
{
public UnityEngine.UI.Text Name = null;
public UnityEngine.UI.Image DisplayIcon = null;
public UnityEngine.UI.Text Level = null;
public UnityEngine.Color MaxLevelColor;
public UnityEngine.Color CurrentLevelColor;
public UnityEngine.UI.Text Expierence = null;
public UnityEngine.UI.Slider ExpSlider = null;
public UnityEngine.GameObject LevelUpIcon = null;
void Awake()
{
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_HeroBonusSimpleInfo_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
ScriptAssembly.Assemble<GUI_HeroBonusSimpleInfo_DL>(gameObject, this);
#endif
}
}