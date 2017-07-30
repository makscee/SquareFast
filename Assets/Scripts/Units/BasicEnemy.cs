using UnityEngine;

public class BasicEnemy : Unit
{
    protected const int MovePeriod = 2;
    protected int MoveT = MovePeriod;
    
    public static readonly Prefab Prefab = new Prefab("SquareEnemy");
    public override Prefab GetPrefab()
    {
        return Prefab;
    }

    private void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        var c = sr.color;
        c.b += Random.value;
        sr.color = c;
    }
    
    public override bool TickUpdate()
    {
        if (HP <= 0) return true;
        if (MoveT == 1)
        {
            Scale = new Vector3(0.8f, 0.8f, 1f);
        }
        if (JustPopped)
        {
            MoveT--;
            JustPopped = false;
            return true;
        }
        if (MoveT > 0)
        {
            MoveT--;
            return true;
        }
        var dir = Player.Instance.Position.IntX() - Position.IntX();
        dir = dir > 0 ? 1 : -1;

        if (!MoveOrAttack(dir)) return false;
        MoveT = MovePeriod;
        Scale = Vector3.one;
        return true;
    }

    public override void TakeDmg(Unit source, int dmg = 1)
    {
        base.TakeDmg(source, dmg);
        MoveT = MovePeriod;
    }
}