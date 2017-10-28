using UnityEngine;

public class CircleEnemy : BasicEnemy
{
    public new static readonly Prefab Prefab = new Prefab("Enemies/CircleEnemy");
    public override Prefab GetPrefab()
    {
        return Prefab;
    }
    
    public override void TakeDmg(Unit source, int dmg = 1)
    {
        if (HP - dmg > 0)
        {
            if (Random.value > 0.5f)
            {
                var dir = Player.Instance.Position.IntX() - Position.IntX();
                Move(dir * 2);
            }
        }
        base.TakeDmg(source, dmg);
        MoveT = 4;
    }
}