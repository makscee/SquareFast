using UnityEngine;

public class TriangleEnemy : BasicEnemy
{
    private int _counterAttack = -1;
    private bool _vulnerable;

    public override bool TickUpdate()
    {
        if (_vulnerable)
        {
            _vulnerable = false;
            return true;
        }
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
        _vulnerable = true;
        var dir = Player.Instance.Position.IntX() - Position.IntX();
        dir = dir > 0 ? 1 : -1;
        MoveOrAttack(dir);
        MoveOrAttack(dir);
        Scale = Vector3.one;
        MoveT = MovePeriod;
        return true;
    }

    private SpritePainter sp;
    public override void TakeDmg(Unit source, int dmg = 1)
    {
        if (sp == null)
        {
            sp = GetComponent<SpritePainter>();
        }
        if (_vulnerable)
        {
            base.TakeDmg(source, dmg);
        }
        else
        {
            sp.Paint(Color.clear, Level.TickTime * 1.5f, true);
            Scale = new Vector3(0.8f, 0.8f, 1f);
            var dir = Position.IntX() - source.Position.IntX();
            dir = dir > 0 ? 1 : -1;
            Move(dir);
            _counterAttack = 1;
        }
    }
}