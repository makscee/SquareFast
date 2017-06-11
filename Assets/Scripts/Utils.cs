using System;
using UnityEngine;

public static class Utils
{
    public static int IntX(this Vector3 v)
    {
        return (int)v.x;
    }

    public static float Interpolate(float from, float to, float over, float t)
    {
        if (t > over)
        {
            return to;
        }
        if (Math.Abs(from - to) < 0.001f)
        {
            return to;
        }
        return from + (to - from) * (t / over);
    }
}