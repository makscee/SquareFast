using UnityEngine;

public class BasicEnemy : Unit
{
    protected const int MovePeriod = 2;
    protected int MoveT = MovePeriod;
    private bool _skipTurn, _skippedTurn;
    
    public static readonly Prefab Prefab = new Prefab("Enemies/SquareEnemy");

    public bool SkipTurn
    {
        get { return _skipTurn; }
        set
        {
            if (!value) _skipTurn = false;
            if (_skippedTurn) return;
            _skipTurn = true;
            _skippedTurn = true;
        }
    }

    public override Prefab GetPrefab()
    {
        return Prefab;
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

        if (_skipTurn)
        {
            _skipTurn = false;
            MoveT = MovePeriod;
            Scale = Vector3.one;
            return true;
        }
        if (!MoveOrAttack(dir)) return false;
        MoveT = MovePeriod;
        Scale = Vector3.one;
        return true;
    }

    public override bool TakeDmg(Unit source, int dmg = 1)
    {
        MoveT = MovePeriod;
        return base.TakeDmg(source, dmg);
    }
}