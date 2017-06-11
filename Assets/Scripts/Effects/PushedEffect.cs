using System.Runtime.Remoting.Messaging;
using UnityEngine;

class PushedEffect
{
    private static readonly Prefab Prefab = new Prefab("PushedEffect");

    public static GameObject Create()
    {
        var go = Prefab.Instantiate();
        return go;
    }
}