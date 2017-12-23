using System;
using System.Collections.Generic;
using UnityEngine;

public class GridMarks : MonoBehaviour
{
    private static readonly Prefab GridSquare = new Prefab("GridSquare");
    public GameObject RightBorder;
    public GameObject LeftBorder;
    public static GridMarks Instance;
    private const int MaxSize = 10;
    private readonly Dictionary<int, GameObject> _marks = new Dictionary<int, GameObject>();

    [NonSerialized]
    public Action HandlerLeft, HandlerRight;
    private void Awake()
    {
        Instance = this;
        for (var i = -MaxSize; i <= MaxSize; i++)
        {
            var square = GridSquare.Instantiate();
            square.transform.position = new Vector3(i, -0.7f, 0);
            square.transform.SetParent(transform);
            _marks[i] = square;
        }
        var border = GridSquare.Instantiate();
        RightBorder = border;
        border.transform.position = new Vector3(1.5f, 0f, 0);
        border.transform.Rotate(0f, 0f, 90f);
        border.transform.SetParent(transform);
        border = GridSquare.Instantiate();
        LeftBorder = border;
        border.transform.position = new Vector3(-1.5f, 0f, 0);
        border.transform.Rotate(0f, 0f, 90f);
        border.transform.SetParent(transform);
    }

    public void SetSize(int size, int sizer = 0)
    {
        if (sizer == 0)
        {
            sizer = size;
        }
        for (var i = -MaxSize; i <= MaxSize; i++)
        {
            _marks[i].SetActive(i >= -size && i <= sizer);
        }
    }

    public void Deactivate(int i)
    {
        _marks[i].SetActive(false);
    }

    public void SetBorders(int l, int r)
    {
        LeftBorder.transform.position = new Vector3(l - 0.5f, 0, 0);
        RightBorder.transform.position = new Vector3(r + 0.5f, 0, 0);
    }

    public void SetBorderHandlers(Action l, Action r)
    {
        HandlerLeft = l;
        HandlerRight = r;
    }
}