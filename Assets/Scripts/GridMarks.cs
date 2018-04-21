using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridMarks : MonoBehaviour
{
    private static readonly Prefab GridSquare = new Prefab("GridSquare");
    public GameObject RightBorder;
    public GameObject LeftBorder;
    public bool RightSolid, LeftSolid;
    public static GridMarks Instance;
    private const int MaxSize = 10;
    private readonly Dictionary<int, GameObject> _marks = new Dictionary<int, GameObject>();
    public Text LeftText, RightText;

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

    public void Set(string lText, string rText, int lPos, int rPos, int lSize, int rSize, Action lAction, Action rAction, bool lSolid = false,
        bool rSolid = false)
    {
        LeftText.text = lText;
        RightText.text = rText;
        SetBorders(lPos, rPos);
        SetBorderHandlers(lAction, rAction);
        SetGrids(lSize, rSize);
        LeftSolid = lSolid;
        RightSolid = rSolid;
        var c = LeftText.GetComponent<UnitedTint>().Color;
        LeftText.GetComponent<UnitedTint>().Color = new Color(c.r, c.g, c.b, 1);
        c = RightText.GetComponent<UnitedTint>().Color;
        RightText.GetComponent<UnitedTint>().Color = new Color(c.r, c.g, c.b, 1);
    }

    public void SetText(string lText, string rText)    
    {
        LeftText.text = lText;
        RightText.text = rText;
    }

    public void ShiftBorder(int dir)
    {
        if (dir == 1)
        {
            Utils.Animate(Vector3.zero, Vector3.right, Unit.AnimationWindow * 2, (v) =>
            {
                LeftBorder.transform.position += v;
                LeftText.transform.position += v;
            });
            for (var i = -MaxSize; i <= MaxSize; i++)
            {
                if (!_marks[i].activeSelf) continue;
                Deactivate(i);
                break;
            }
        }
        else
        {
            Utils.Animate(Vector3.zero, Vector3.right, Unit.AnimationWindow * 2, (v) =>
            {
                RightBorder.transform.position -= v;
                RightText.transform.position -= v;
            });
            for (var i = MaxSize; i >= -MaxSize; i--)
            {
                if (!_marks[i].activeSelf) continue;
                Deactivate(i);
                break;
            }
        }
    }

    public int FieldSize()
    {
        var res = 0;
        for (var i = -MaxSize; i <= MaxSize; i++)
        {
            if (_marks[i].activeSelf) res++;
        }
        return res;
    }

    public void SetGrids(int l, int r = 0)
    {
        if (r == 0)
        {
            r = l;
        }
        for (var i = -MaxSize; i <= MaxSize; i++)
        {
            _marks[i].SetActive(i >= l && i <= r);
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
        LeftText.transform.position = new Vector3(l - 0.5f - 0.2f, 0, 0);
        RightText.transform.position = new Vector3(r + 0.5f + 0.2f, 0, 0);
    }

    public void SetBorderHandlers(Action l, Action r)
    {
        HandlerLeft = l;
        HandlerRight = r;
    }
}