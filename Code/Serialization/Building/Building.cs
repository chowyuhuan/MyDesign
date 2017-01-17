using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public enum BuildingType
{
    None,
    Store,
    Friend,
    Battle,
    Bread,
    Skill,
    Equipment,
    Hero,
    Arena,
    God,
}

public class Building : MonoBehaviour
{
    public BuildingType SelfType = BuildingType.None;

    void Awake()
    {
#if JIT && !UNITY_IOS
        ScriptAssembly.Assemble(gameObject, "Building_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<Building_DL>(gameObject, this);
#endif
    }
}
