using UnityEngine;

public class HitEffect
{
	private static readonly Prefab Prefab = new Prefab("HitEffect");
    public static void Create(Vector2 position, Unit unit)
    {
        var he = Prefab.Instantiate();
        he.transform.position = position;
        var ps = he.GetComponent<ParticleSystem>();

        var main = ps.main;
        main.startColor = unit.GetComponent<SpriteRenderer>().color;
        Object.Destroy(he, 5f);
    }
}

public class ShieldDieEffect
{
	private static readonly Prefab Prefab = new Prefab("ShieldDieEffect");
    public static void Create(Vector2 position)
    {
        var he = Prefab.Instantiate();
        he.transform.position = position;
        Object.Destroy(he, 5f);
    }
}

public class SpawnEffect
{
    private static readonly Prefab Prefab = new Prefab("SpawnEffect");
    public static void Create(Vector2 position, Unit unit)
    {
        var he = Prefab.Instantiate();
        he.transform.position = position;
        var ps = he.GetComponent<ParticleSystem>();

        var main = ps.main;
        main.startColor = unit.GetComponent<SpriteRenderer>().color * (unit is Player ? Color.white : UnitedTint.Tint);
        Utils.InvokeDelayed(() =>
        {
            if (ps == null) return;
            ps.Stop();
        }, 0.4f);
        Object.Destroy(he, 0.6f);
    }
}