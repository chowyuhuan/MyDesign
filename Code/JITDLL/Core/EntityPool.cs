using UnityEngine;
using System.Collections;

/// <summary>
/// 实体缓冲池
/// GameObject的创建和销毁走这里，后期会优化成池，也可能会针对不同的类型做分类，现阶段先用这个类
/// </summary>
public class EntityPool
{
    // 先不提供模板函数，防止用得太多，后期优化难以处理
    public static Object Spwan(string name)
    {
        GameObject temp = Resources.Load<GameObject>(name);
        if(temp != null)
        {
            return Object.Instantiate(temp);
        }
        return null;
    }

    public static Object Spwan(string name, Vector3 pos)
    {
        GameObject temp = Resources.Load<GameObject>(name);
        if(temp != null)
        {
            return Object.Instantiate(temp, pos, temp.transform.rotation);
        }
        return null;
    }

    public static void Destroy(UnityEngine.Object obj, float t = 0)
    {
        Object.Destroy(obj, t);
    }
}
