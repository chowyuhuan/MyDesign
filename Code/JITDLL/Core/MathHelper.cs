using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MathHelper
{
    /// <summary>
    /// 将总和(total)分成多块(count),每块偏离不会超过平均数 
    /// </summary>
    /// <param name="total">总和数</param>
    /// <param name="count">分块个数</param>
    /// <returns></returns>
    public static List<int> RandomAverageDivide(int total, int count)
    {
        List<int> result = new List<int>();
        if (total < count)
        {
            int cnt = 0;
            for (; cnt < total; ++cnt)
            {
                result.Add(1);
            }
            for (; cnt < count; ++cnt)
            {
                result.Add(0);
            }

            return result;
        }

        int average = total / count;
        for (int i = 0; i < count; ++i)
        {
            if (i < count - 1)
            {
                result.Add(average);
            }
            else
            {
                result.Add(average + total - count * average);
            }
        }

        int temp = 0;
        for (int i = 0; i < count; ++i)
        {
            temp = Random.Range(0, average);
            result[i] -= temp; 
            result[(i + 1) % count] += temp;
        }

        return result;
    }

    public static List<int> RandomDivide(int total, int count)
    {
        List<int> result = new List<int>();
        if (total < count)
        {
            int cnt = 0;
            for (; cnt < total; ++cnt)
            {
                result.Add(1);
            }
            for (; cnt < count; ++cnt)
            {
                result.Add(0);
            }

            return result;
        }

        List<int> divide = new List<int>();

        int temp = 0;
        for (int i = 0; i < count - 1; ++i)
        {
            temp = Random.Range(1, total);
            while(divide.Contains(temp))
            {
                temp = Random.Range(1, total);
            }

            divide.Add(temp);
        }
        divide.Sort();

        int current = 0;
        for (int i = 0; i < divide.Count; ++i)
        {
            result.Add(divide[i] - current);
            current = divide[i];
        }
        result.Add(total - current);

        return result;
    }
}
