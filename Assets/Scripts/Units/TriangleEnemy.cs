using System.Security.Cryptography;
using UnityEngine;

public class TriangleEnemy : BasicEnemy
{
    private int _counterAttack = -1;

    public override bool TickUpdate()
    {
        if (_counterAttack == -1)
        {
            return base.TickUpdate();
        }
        if (_counterAttack > 0)
        {
            _counterAttack--;
            return true;
        }
        _counterAttack = -1;
        var dir = Player.Instance.Position.IntX() - Position.IntX();
        dir = dir > 0 ? 1 : -1;
        MoveOrAttack(dir);
        MoveOrAttack(dir);
        Scale = Vector3.one;
        MoveT = MovePeriod;
        return true;
    }

    public override void TakeDmg(Unit source, int dmg = 1)
    {
        if (Shield )
        {
            Scale = new Vector3(0.8f, 0.8f, 1f);
            _counterAttack = 1;
        }
        base.TakeDmg(source, dmg);
    }
}