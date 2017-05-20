public class BasicEnemy : Unit
{

    private const int MovePeriod = 2;
    private int _moveT = MovePeriod;
    public override void TickUpdate()
    {
        if (_moveT > 0)
        {
            _moveT--;
            return;
        }
        var dir = Player.Instance.Position - Position;
        dir = dir > 0 ? 1 : -1;
        MoveOrAttack(dir);
        _moveT = MovePeriod;
    }
}