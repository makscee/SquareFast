using System.Collections.Generic;
using UnityEngine;

class Prefab
{
    private readonly string _path;
    private GameObject _resource;
    private static List<Prefab> _prefabs;

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
        }
    }

    public GameObject Instantiate()
    {
        if (_resource == null)
        {
            _resource = Resources.Load<GameObject>(_path);
        }
        return Object.Instantiate(_resource);
    }
}