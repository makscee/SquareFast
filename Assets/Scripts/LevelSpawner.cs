using System;
using System.Collections.Generic;
using UnityEngine;

public enum TickAction
{
    None, SimpleSquare, Square, Rhombus, Triangle, RhombusSSquare, Save
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
    private static readonly Prefab SSquare = new Prefab("SimpleSquareEnemy");
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
                PlaceGo(SSquare);
                break;
            case TickAction.Square:
                PlaceGo(Square);
                break;
            case TickAction.Rhombus:
                PlaceGo(Rhombus);
                break;
            case TickAction.Triangle:
                PlaceGo(Triangle);
                break;
            case TickAction.RhombusSSquare:
                PlaceGo(Rhombus, SSquare);
                break;
            case TickAction.Save:
                Level.SaveTicks = Level.Ticks;
                CameraScript.Instance.GetComponent<SpritePainter>().Paint(new Color(0.08f, 0.43f, 0.24f), 0.5f, true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private GameObject PlaceGo(Prefab l, Prefab r = null)
    {
        GameObject go = null;
        if (Level.Ticks % 3 == 0)
        {
            if (Level.Ticks / 3 % 2 == 0)
            {
                go = r == null ? l.Instantiate() : r.Instantiate();
                go.transform.position = new Vector3(Distance, 0, 0);
            }
            else
            {
                go = l.Instantiate();
                go.transform.position = new Vector3(-Distance, 0, 0);
            }
        }
        return go;
    }
}