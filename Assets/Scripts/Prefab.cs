using System.Collections.Generic;
using UnityEngine;

class Prefab
{
    private readonly string _path;
    private GameObject _resource;
	private static List<Prefab> _prefabs = new List<Prefab>();

	public Prefab(string path)
    {
        _path = path;
		_prefabs.Add(this);
    }

    public static void PreloadPrefabs()
    {
        foreach (var prefab in _prefabs)
        {
            prefab._resource = Resources.Load<GameObject>(prefab._path);
			var go = Object.Instantiate(prefab._resource);
			GameObject.Destroy(go);
        }
    }

    public GameObject Instantiate()
    {
        return Object.Instantiate(_resource);
    }
}