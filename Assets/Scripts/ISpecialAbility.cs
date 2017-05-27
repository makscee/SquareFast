using UnityEngine;

public interface ISpecialAbility
{
    void Use();
}

public class Dash : ISpecialAbility
{
    private readonly int _dir;
    private readonly int _dist = 3;

    public Dash(int dir)
    {
        _dir = dir;
    }
    public void Use()
    {
        var oldPos = Player.Instance.Position;
        var pos = oldPos + _dir * _dist;
        Level.Instance.Attack(pos, Player.Instance);
        Player.Instance.Move(_dir * _dist);
        for (var i = oldPos; i != pos; i = pos < oldPos ? i - 1 : i + 1)
        {
            Level.Instance.Attack(i, Player.Instance);
        }
        Level.Instance.TickUpdate();
     }
}

