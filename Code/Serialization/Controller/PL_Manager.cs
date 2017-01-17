using UnityEngine;
public class PL_Manager : UnityEngine.MonoBehaviour // !!!TODO:存在继承关系时，确保基类已经存在；如果继承链中没有序列化数据，去掉继承，改成MonoBehaviour
{
// !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在
public GameObject LoadingUIObject = null;
void Awake()
{
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"PL_Manager_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
ScriptAssembly.Assemble<PL_Manager_DL>(gameObject, this);
#endif
}
}