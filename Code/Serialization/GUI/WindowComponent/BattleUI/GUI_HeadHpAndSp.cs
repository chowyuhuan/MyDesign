using UnityEngine;

// TODO：确保此文件中使用到的类型已经存在
public class GUI_HeadHpAndSp : MonoBehaviour
{
    public GameObject _HpSlider = null;
    public GameObject _SpSlider = null;
    public float HeadUpDistance = 0;
    public float UpStepDistance = 20;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_HeadHpAndSp_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_HeadHpAndSp_DL>(gameObject, this);
#endif
    }
}