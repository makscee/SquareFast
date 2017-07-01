using UnityEngine;

public class HitEffect : MonoBehaviour
{
	private static readonly Prefab Prefab = new Prefab("HitEffect");
    public static void Create(Vector2 position, Unit unit)
    {
        var he = Prefab.Instantiate();
        he.transform.position = position;
        var ps = he.GetComponent<ParticleSystem>();

        var main = ps.main;
        main.startColor = unit.GetComponent<SpriteRenderer>().color;
        Destroy(he, 5f);
    }
}

public class ShieldDieEffect : MonoBehaviour
{
	private static readonly Prefab Prefab = new Prefab("ShieldDieEffect");
    public static void Create(Vector2 position)
    {
        var he = Prefab.Instantiate();
        he.transform.position = position;
        Destroy(he, 5f);
    }
}