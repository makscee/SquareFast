using System;
using System.Collections.Generic;
using UnityEngine;

public enum TickAction
{
    None, SimpleSquare, Square, Rhombus, Triangle
}

[Serializable]
public struct Event
{
    public int Tick;
    public TickAction TickAction;
    public bool CountDistance;
}

public class LevelSpawner
{
    private const int Distance = 8;
    public TickAction CurAction = TickAction.None;
    public List<Event> TickEvents;
    
    private static readonly Prefab Square = new Prefab("SquareEnemy");
    private static readonly Prefab Triangle = new Prefab("TriangleEnemy");
    private static readonly Prefab Rhombus = new Prefab("RhombusEnemy");

    public void TickUpdate()
    {
        foreach (var tickEvent in TickEvents)
        {
            var t = Level.Ticks + (tickEvent.CountDistance ? Distance * 3 : 0);
            if (tickEvent.Tick == t)
            {
                CurAction = tickEvent.TickAction;
            }
        }
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