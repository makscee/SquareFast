using UnityEngine;

public class RhombusEnemy : BasicEnemy
{
    public override void TakeDmg(Unit source, int dmg = 1)
    {
        if (Shield)
        {
            HadShield = true;
            Scale = new Vector3(0.8f, 0.8f, 1f);
            MoveT = 1;
            var dir = Player.Instance.Position.IntX() - Position.IntX();
            Move(dir * 2);
            DestroyShield();
            return;
        }
        base.TakeDmg(source, dmg);
    }
}