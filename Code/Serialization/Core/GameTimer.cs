using UnityEngine;
using System.Collections;

public class GameTimer
{
    public static float time
    {
        get
        {
            return Time.time;
        }
    }

    public static float deltaTime
    {
        get
        {
            return Time.deltaTime;
        }
    }

    public static int frameCount
    {
        get
        {
            return Time.frameCount;
        }
    }
}
