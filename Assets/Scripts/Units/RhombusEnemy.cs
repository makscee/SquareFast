public class RhombusEnemy : BasicEnemy
{
    public new static readonly Prefab Prefab = new Prefab("Enemies/RhombusEnemy");
    public override Prefab GetPrefab()
    {
        return Prefab;
    }
    
    public override bool TakeDmg(Unit source, int dmg = 1)
    {
        if (HP - dmg > 0)
        {
            MoveT = 2;
            var dir = Player.Instance.Position.IntX() - Position.IntX();
            Move(dir * 2);
        }
        return base.TakeDmg(source, dmg);
    }
}