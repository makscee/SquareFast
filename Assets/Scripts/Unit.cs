using UnityEngine;

public class Unit : MonoBehaviour
{
    public int Position;
    public int HP = 1;

    private void Start()
    {
        Level.Instance.InitPos(this);
    }

    protected void MoveOrAttack(int relDir)
    {
        if (Level.Instance.MoveOrAttack(Position + relDir, this))
        {
            Position += relDir;
            transform.position = new Vector3(Position, 0, 0);
        }
    }

    protected void Move(int relDir)
    {
        if (Level.Instance.Move(Position + relDir, this))
        {
            Position += relDir;
            transform.position = new Vector3(Position, 0, 0);
        }

    }

    public void TakeDmg(Unit source, int dmg = 1)
    {
        HP -= dmg;
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

    public void Die()
    {
        Destroy(gameObject);
    }
}