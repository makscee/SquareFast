using UnityEngine;

public class Unit : MonoBehaviour
{
    public int Position;

    private void Start()
    {
        Level.Instance.InitPos(this);
    }

    public void Move(int pos)
    {
        Position = pos;
        transform.position = new Vector3(pos, 0, 0);
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}