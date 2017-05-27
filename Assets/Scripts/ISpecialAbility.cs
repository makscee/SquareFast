using UnityEngine;

public interface ISpecialAbility
{
    void Use();
}

public class DashL : ISpecialAbility
{
    public void Use()
    {
        Debug.Log("SWOOOSH");
    }
}

public class DashR : ISpecialAbility
{
    public void Use()
    {
        Debug.Log("SHOOOOWS");
    }
}

