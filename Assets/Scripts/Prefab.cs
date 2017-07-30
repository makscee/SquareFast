using System.Collections.Generic;
using UnityEngine;

public class Prefab
{
    private readonly string _path;
    private GameObject _resource;
	private static readonly List<Prefab> Prefabs = new List<Prefab>();

	public Prefab(string path)
    {
        _path = path;
		Prefabs.Add(this);
    }

    public static void PreloadPrefabs()
    {
        foreach (var prefab in Prefabs)
        {
            prefab._resource = Resources.Load<GameObject>(prefab._path);
			var go = Object.Instantiate(prefab._resource);
			Object.Destroy(go);
        }
    }

    public GameObject Instantiate()
    {
        return Object.Instantiate(_resource);
    }

    public override string ToString()
    {
        return _path;
    }
}