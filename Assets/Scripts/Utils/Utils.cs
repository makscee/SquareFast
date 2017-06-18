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
        var delta = Time.deltaTime;
        if (t + delta > over)
        {
            delta = Math.Max(0, over - t);
        }
        return (to - from) / over * delta;
    }
}