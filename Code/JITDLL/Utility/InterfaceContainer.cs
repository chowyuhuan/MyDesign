using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InterfaceContainer
{
    static Dictionary<Type, List<object>> listeners = new Dictionary<Type, List<object>>();

    public static void AddListener<T>(T obj)
    {
        Type type = typeof(T);
        List<object> objList = null;

        listeners.TryGetValue(type, out objList);

        if (objList == null)
        {
            objList = new List<object>();
            listeners.Add(type, objList);
        }

        if (!objList.Contains(obj))
        {
            objList.Add(obj);
        }
    }

    public static void RemoveListener<T>(T obj)
    {
        List<object> objList = null;

        listeners.TryGetValue(typeof(T), out objList);

        if (objList != null && objList.Contains(obj))
        {
            objList.Remove(obj);
        }
    }

    public static void RemoveAllListener()
    {
        // 妹的foreach有gc，unity你敢不敢升级下mono 
        Dictionary<Type, List<object>>.Enumerator itr = listeners.GetEnumerator();
        while (itr.MoveNext())
        {
            itr.Current.Value.Clear();
        }

        listeners.Clear();
    }

    public static List<object> GetListeners<T>()
    {
        Type type = typeof(T);

        if (!listeners.ContainsKey(type))
        {
            listeners.Add(type, new List<object>());
        }

        return listeners[type];
    }
}
