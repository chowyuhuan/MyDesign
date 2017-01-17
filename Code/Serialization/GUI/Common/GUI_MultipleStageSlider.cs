// TODO：确保此文件中使用到的类型已经存在
public class GUI_MultipleStageSlider : UnityEngine.MonoBehaviour
{
    public ESliderStage StageType = 0;
    public UnityEngine.RectTransform StageContainer = null;
    public System.Collections.Generic.List<UnityEngine.UI.Image> StageImage = new System.Collections.Generic.List<UnityEngine.UI.Image>();
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_MultipleStageSlider_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_MultipleStageSlider_DL>(gameObject, this);
#endif
    }
}