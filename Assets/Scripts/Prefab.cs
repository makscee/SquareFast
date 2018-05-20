using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Prefab
{
    private readonly string _path;
    private GameObject _resource;
	private static readonly List<Prefab> Prefabs = new List<Prefab>();

    public static void TouchStatics() {
        var types = new List<Type>
        {
            typeof(HitEffect),
            typeof(PushedEffect),
            typeof(ShieldDieEffect),
            typeof(Unit),
            typeof(SpawnEffect),
            typeof(LevelSpawner),
            typeof(GridMarks),
        };
        foreach (var t in types) 
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor (t.TypeHandle);
        }
    }
    
	public Prefab(string path)
    {
        _path = path;
		Prefabs.Add(this);
    }

    public static void PreloadPrefabs()
    {
        foreach (var prefab in Prefabs)
        {
            if (prefab._resource == null)
            {
                prefab._resource = Resources.Load<GameObject>(prefab._path);
            }
        }
    }

    public GameObject Instantiate()
    {
        if (_path == "Enemies/SquareEnemy") Debug.LogWarning("spawn " + _resource);
        if (_resource == null)
        {
            Debug.Log("Resource " + _path + " was null. Loading.");
            _resource = Resources.Load<GameObject>(_path);
        }
        return Object.Instantiate(_resource);
    }

    public override string ToString()
    {
        return _path;
    }
}