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
        GameObject.Destroy(he, 5f);
    }
}