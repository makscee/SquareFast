using UnityEngine;

public class Unit : MonoBehaviour
{
    public int Position;
    public int HP = 1;

    private void Start()
    {
        Level.Instance.InitPos(this);
    }

    protected bool MoveOrAttack(int relDir)
    {
        var pos = relDir + Position;
        var atPos = Level.Instance.Get(pos);
        if (atPos != null)
        {
            bool class1 = atPos is Player, class2 = this is Player;
            if (class1 != class2)
            {
                atPos.TakeDmg(this);
                return true;
            }
            else
            {
                return false;
            }
        }

        Move(relDir);
        return true;
    }

    public void Move(int relDir)
    {
        Level.Instance.Move(Position + relDir, this);
        Position += relDir;
        transform.position = new Vector3(Position, transform.position.y, 0);
    }

    public virtual void TakeDmg(Unit source, int dmg = 1)
    {
        Debug.Log("take dmg " + this.name + " " + source.name);
        HP -= dmg;
        HitEffect.Create(transform.position, this);
        if (HP <= 0)
        {
            Die();
        }
        else
        {
            var dir = Position - source.Position;
            dir = dir > 0 ? 1 : -1;
            Move(dir);
        }
    }

    public virtual bool TickUpdate()
    {
        return true;
    }

    public virtual void Die()
    {
        Level.Instance.Clear(Position);
        Destroy(gameObject);
    }
}