public class BattleManager : UnityEngine.MonoBehaviour // !!!TODO:存在继承关系时，确保基类已经存在；如果继承链中没有序列化数据，去掉继承，改成MonoBehaviour
{
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"BattleManager_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<BattleManager_DL>(gameObject, this);
#endif
    }
}