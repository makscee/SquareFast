using System;
using System.Collections;
using System.Security.Cryptography;
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

    public static void Animate(Vector3 from, Vector3 to, float over, Action<Vector3> onChange, MonoBehaviour obj = null, bool fullValue = false)
    {
        obj = obj == null ? Level.Instance : obj;
        obj.StartCoroutine(Animation(from, to, over, onChange, fullValue));
    }

    public static void Animate(float from, float to, float over, Action<float> onChange, MonoBehaviour obj = null, bool fullValue = false)
    {
        obj = obj == null ? Level.Instance : obj;
        obj.StartCoroutine(Animation(new Vector3(from, 0), new Vector3(to, 0), over, v => onChange(v.x), fullValue));
    }

    private static IEnumerator Animation(Vector3 from, Vector3 to, float over, Action<Vector3> action, bool fullValue)
    {
        var unit = action.Target as Unit;
        if (unit != null)
        {
            unit.RunningAnimations++;
        }
        var t = 0f;
        var result = from;
        while (t - Time.deltaTime < over)
        {
            var x = Interpolate(from.x, to.x, over, t);
            var y = Interpolate(from.y, to.y, over, t);
            var z = Interpolate(from.z, to.z, over, t);
            var temp = new Vector3(x, y, z);
            result += temp;
            action(fullValue ? result : temp);
            t += Time.deltaTime;
            yield return null;
        }
        if (unit != null)
        {
            unit.RunningAnimations--;
        }
    }
}