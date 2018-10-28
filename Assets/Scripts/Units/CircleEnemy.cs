using UnityEngine;

public class CircleEnemy : BasicEnemy
{
    public new static readonly Prefab Prefab = new Prefab("Enemies/CircleEnemy");
    public override Prefab GetPrefab()
    {
        return Prefab;
    }

    private bool _hasSwallowed;

    public override bool TickUpdate()
    {
        return _hasSwallowed || base.TickUpdate();
    }

    public void Swallow()
    {
        AnimationSpeedUp = 3f;
        Scale = new Vector3(1.5f, 1.5f, 1f);
        var ut = GetComponent<UnitedTint>();
        ut.Color = ut.Color.ChangeAlpha(0.4f);
        Level.Instance.Clear(Position.IntX());
        Player.Instance.Swallowed = this;
        _hasSwallowed = true;
        Position = Player.Instance.Position;
        _actPos = new Vector3(100, 0, 0);
        HitEffect.Create(Player.Instance.transform.position, Player.Instance);
    }
}