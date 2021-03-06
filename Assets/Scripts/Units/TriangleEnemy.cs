using System.Security.Cryptography;
using UnityEngine;

public class TriangleEnemy : BasicEnemy
{
    private int _counterAttack = -1;
    public new static readonly Prefab Prefab = new Prefab("Enemies/TriangleEnemy");
    public override Prefab GetPrefab()
    {
        return Prefab;
    }

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

    public override bool TakeDmg(Unit source, int dmg = 1)
    {
        if (HP - dmg > 0)
        {
            Scale = new Vector3(0.8f, 0.8f, 1f);
            _counterAttack = 1;
            var dir = Player.Instance.Position.IntX() - Position.IntX();
            dir = dir > 0 ? -1 : 1;
            Move(dir);
        }
        return base.TakeDmg(source, dmg);
    }
}