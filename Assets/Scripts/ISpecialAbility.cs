using UnityEngine;

public interface ISpecialAbility
{
    void Use();
}

public class Dash : ISpecialAbility
{
    private readonly int _dir;
    private const int Dist = 3;

    public Dash(int dir)
    {
        _dir = dir;
    }
    public void Use()
    {
        var oldPos = Player.Instance.Position.IntX();
        var pos = oldPos + _dir * Dist;
        Level.Instance.Attack(pos, Player.Instance);
        Player.Instance.Move(_dir * Dist);
        for (var i = oldPos; i != pos; i = pos < oldPos ? i - 1 : i + 1)
        {
            Level.Instance.Attack(i, Player.Instance);
        }
        Level.Instance.TickUpdate();
     }
}

