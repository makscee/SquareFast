public class BasicEnemy : Unit
{

    private const int MovePeriod = 2;
    private int _moveT = MovePeriod;
    public override bool TickUpdate()
    {
        if (HP <= 0) return true;
        if (_moveT > 0)
        {
            _moveT--;
            return true;
        }
        var dir = Player.Instance.Position - Position;
        dir = dir > 0 ? 1 : -1;
        
        var check = Level.Instance.Get(Position + dir);
        if (check != null && !(check is Player)) return false;
        
        MoveOrAttack(dir);
        _moveT = MovePeriod;
        return true;
    }

    public override void TakeDmg(Unit source, int dmg = 1)
    {
        base.TakeDmg(source, dmg);
        _moveT = MovePeriod;
    }
}