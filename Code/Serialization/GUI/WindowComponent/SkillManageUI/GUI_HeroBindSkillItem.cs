using UnityEngine;

public sealed class GUI_HeroBindSkillItem : GUI_ToggleItem
{
    public GUI_SkillSimpleInfo SkillSimpleInfo = null;
    public GUI_ActorSimpleInfo HeroSimpleInfo = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_HeroBindSkillItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_HeroBindSkillItem_DL>(gameObject, this);
#endif
    }
}