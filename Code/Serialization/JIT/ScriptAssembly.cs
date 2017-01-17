using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class ScriptAssembly 
{
    static Dictionary<string, Type> LogicTypes = new Dictionary<string, Type>();

    public static void LoadAllLogicTypes(Assembly assembly) 
    {
        Type[] types = assembly.GetTypes();
        for (int i = 0; i < types.Length; ++i )
        {
            if(!types[i].IsSubclassOf(typeof(MonoBehaviour)))
            {
                continue;
            }
            //Debug.LogError(LogTag.JIT + "逻辑DLL中获取出类型：" + types[i].Name);
            LogicTypes.Add(types[i].Name,types[i]); // TODO:这里的name可能是有路径的，有的话，需要去掉
        }
	}
	
    public static bool GetLogicType(string name, out Type type)
    {
        return LogicTypes.TryGetValue(name, out type);
    }

    public static void Assemble(GameObject obj, string name, Component old)
    {
        if (obj == null)
        {
            return;
        }
        Type logic = null;
        if (GetLogicType(name, out logic))
        {
            obj.AddComponent(logic);
            GameObject.Destroy(old); // 为了优化，先销毁掉，看看会不会遇到问题
        }
        else
        {
            Debug.LogError(LogTag.JIT + "没有找到指定名字的逻辑类型：" + name + ",GameObject：" + obj.name);
        }
    }

    public static void Assemble<T>(GameObject obj, Component old) where T : Component
    {
        if (obj == null)
        {
            return;
        }
        obj.AddComponent<T>();
        GameObject.Destroy(old); // 为了优化，先销毁掉，看看会不会遇到问题
    }
}