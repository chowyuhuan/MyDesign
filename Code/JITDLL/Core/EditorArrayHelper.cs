using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EditorArrayHelper
{
    public static T[] AddOne<T>(ref T[] array, ref T element)
    {
        if (array == null || array.Length <= 0)
        {
            return new T[1] { element };
        }
        else
        {
            return array.Concat<T>(new T[1] { element }).ToArray();
        }
    }


    public static T[] AddRange<T>(ref T[] array, ref T[] elements)
    {
        if (array == null || array.Length <= 0)
        {
            return elements; // !!!注意：这里返回的直接是elements的引用
        }
        else
        {
            return array.Concat<T>(elements).ToArray();
        }
    }

    public static void RemoveOne<T>(ref T[] array, ref T element)
    {
        T[] temp = new T[array.Length - 1];
        int j = 0;
        for (int i = 0; i < array.Length; ++i)
        {
            if (array[i].Equals(element))
            {
                continue;
            }
            temp[j++] = array[i]; 
        }
        array = temp;
    }
}
