using UnityEngine;

public class HitEffect : MonoBehaviour
{
    public static void Create(Vector2 position, Unit unit)
    {
        var he = Resources.Load<GameObject>("HitEffect");
        he = GameObject.Instantiate(he);
        he.transform.position = position;
        var ps = he.GetComponent<ParticleSystem>();

        var main = ps.main;
        main.startColor = unit.GetComponent<SpriteRenderer>().color;
        GameObject.Destroy(he, 5f);
    }
}