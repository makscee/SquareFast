using System;
using UnityEngine;

public enum TickAction
{
    None, SimpleSquare, Square, Rhombus, Triangle
}

public class LevelSpawner
{
    private const int Distance = 8;
    public TickAction CurAction = TickAction.None;
    
    private static readonly Prefab Square = new Prefab("SquareEnemy");
    private static readonly Prefab Triangle = new Prefab("TriangleEnemy");
    private static readonly Prefab Rhombus = new Prefab("RhombusEnemy");

    public void TickUpdate()
    {
        switch (CurAction)
        {
            case TickAction.None:
                break;
            case TickAction.SimpleSquare:
                if (Level.Ticks % 3 == 0)
                {
                    var go = Square.Instantiate();
                    go.GetComponent<Unit>().Shielded = false;
                    if (Level.Ticks / 3 % 2 == 0)
                    {
                        go.transform.position = new Vector3(Distance, 0, 0);
                    }
                    else
                    {
                        go.transform.position = new Vector3(-Distance, 0, 0);
                    }
                }
                break;
            case TickAction.Square:
                break;
            case TickAction.Rhombus:
                break;
            case TickAction.Triangle:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}