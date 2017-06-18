using UnityEngine;

public class BasicEnemy : Unit
{
    private const int MovePeriod = 2;
    private int _moveT = MovePeriod;

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
        if (_moveT == 1)
        {
            Scale = new Vector3(0.8f, 0.8f, 1f);
        }
        if (JustPopped)
        {
            _moveT--;
            JustPopped = false;
            return true;
        }
        if (_moveT > 0)
        {
            _moveT--;
            return true;
        }
        var dir = Player.Instance.Position.IntX() - Position.IntX();
        dir = dir > 0 ? 1 : -1;

        if (!MoveOrAttack(dir)) return false;
        _moveT = MovePeriod;
        Scale = Vector3.one;
        return true;
    }

    public override void TakeDmg(Unit source, int dmg = 1)
    {
        base.TakeDmg(source, dmg);
        _moveT = MovePeriod;
    }
}