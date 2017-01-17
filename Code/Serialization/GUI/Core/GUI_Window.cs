public class GUI_Window : UnityEngine.MonoBehaviour // !!!TODO:存在继承关系时，确保基类已经存在；如果继承链中没有序列化数据，去掉继承，改成MonoBehaviour
{
    public bool CloseOnEscape = true;
    // !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
    public string Sound = "";
    public float Delay = 0;
    public UnityEngine.UI.Button CloseButton;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_Window_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_Window_DL>(gameObject, this);
#endif
    }
}