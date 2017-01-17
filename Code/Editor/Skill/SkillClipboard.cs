using UnityEngine;
using System.Collections;
using SKILL;
using System.Reflection;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;

public class SkillClipboard
{
    static MetaBase _cache;
    public static void Copy(MetaBase data)
    {
        _cache = data;

        MetaBase.isSkillMetaUseForCopy = data is Skill;
    }

    public static T Paste<T>() where T : MetaBase
    {
        T temp = _cache as T;
        if(temp != null)
        {
            return temp.DeepClone() as T;
        }
        return null;
    }

    // 这是浅拷贝
    //public static T DeepCopy<T>(T obj)
    //{
    //    //如果是字符串或值类型则直接返回
    //    if (obj is string || obj.GetType().IsValueType) return obj;

    //    object retval = Activator.CreateInstance(obj.GetType());
    //    FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
    //    foreach (FieldInfo field in fields)
    //    {
    //        try { field.SetValue(retval, DeepCopy(field.GetValue(obj))); }
    //        catch { }
    //    }
    //    return (T)retval;
    //}

    //public static T DeepCopy<T>(T obj)
    //{
    //    object retval;
    //    using (MemoryStream ms = new MemoryStream())
    //    {
    //        BinaryFormatter bf = new BinaryFormatter();
    //        //序列化成流
    //        bf.Serialize(ms, obj);
    //        ms.Seek(0, SeekOrigin.Begin);
    //        //反序列化成对象
    //        retval = bf.Deserialize(ms);
    //        ms.Close();
    //    }
    //    return (T)retval;
    //}

    //public static T DeepCopy<T>(T RealObject)
    //{
    //    using (Stream stream = new MemoryStream())
    //    {
    //        XmlSerializer serializer = new XmlSerializer(typeof(T));
    //        serializer.Serialize(stream, RealObject);
    //        stream.Seek(0, SeekOrigin.Begin);
    //        return (T)serializer.Deserialize(stream);
    //    }
    //}  

    //public static T DeepCopy<T>(T obj)
    //{
    //    object retval;
    //    using (MemoryStream ms = new MemoryStream())
    //    {
    //        DataContractSerializer ser = new DataContractSerializer(typeof(T));
    //        ser.WriteObject(ms, obj);
    //        ms.Seek(0, SeekOrigin.Begin);
    //        retval = ser.ReadObject(ms);
    //        ms.Close();
    //    }
    //    return (T)retval;
    //}
}
