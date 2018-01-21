using UnityEngine;

public class DownTriangleEnemy : BasicEnemy
{
    public new static readonly Prefab Prefab = new Prefab("Enemies/DownTriangleEnemy");
    public override Prefab GetPrefab()
    {
        return Prefab;
    }
}