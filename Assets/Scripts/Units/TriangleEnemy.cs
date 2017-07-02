using System.Security.Cryptography;
using UnityEngine;

public class TriangleEnemy : BasicEnemy
{
    private int _counterAttack = -1;
    private bool _vulnerable;

    public GameObject Shield;
    private Material _shieldMat;

    public override bool TickUpdate()
    {
        if (_vulnerable)
        {
            _vulnerable = false;
            Shield.gameObject.SetActive(true);
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
        Shield.gameObject.SetActive(false);
        ShieldDieEffect.Create(transform.position);
        var dir = Player.Instance.Position.IntX() - Position.IntX();
        dir = dir > 0 ? 1 : -1;
        MoveOrAttack(dir);
        MoveOrAttack(dir);
        Scale = Vector3.one;
        MoveT = MovePeriod;
        return true;
    }

    public override void TakeDmg(Unit source, int dmg = 1)
    {
        if (_vulnerable)
        {
            base.TakeDmg(source, dmg);
        }
        else
        {
            if (_shieldMat == null)
            {
                _shieldMat = Shield.GetComponent<SpriteRenderer>().material;
            }
            Utils.Animate(6f, 1f, Level.TickTime * 2, (v) => _shieldMat.SetFloat("_Percentage", v), this, true);
            Scale = new Vector3(0.8f, 0.8f, 1f);
            var dir = Position.IntX() - source.Position.IntX();
            dir = dir > 0 ? 1 : -1;
            Move(dir);
            _counterAttack = 1;
        }
    }
}