using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLevel : Level
{
    private static readonly Prefab GridSquare = new Prefab("GridSquare");
    private static readonly Prefab MenuItemPrefab = new Prefab("MenuItemText");
    public Canvas WorldCanvas;

    private class MenuItem
    {
        public readonly Text Text;
        public Action Action;

        public MenuItem(string text, Action action)
        {
            Action = action;
            var go = MenuItemPrefab.Instantiate();
            Text = go.GetComponent<Text>();
            Text.text = text;
            go.transform.SetParent(((MenuLevel) Instance).WorldCanvas.transform);
        }
    }

    private List<MenuItem> _items;

    public void NextItem()
    {
        var item = _items[0];
        _items.RemoveAt(0);
        _items.Add(item);
        RefreshItems();
    }

    public void Confirm()
    {
        _items[0].Action();
    }

    private const float AnimationWindow = 0.2f;
    private void RefreshItems()
    {
        var i = 0;
        foreach (var menuItem in _items)
        {
            var t = menuItem.Text;
            Utils.Animate(t.transform.position, new Vector3(i * 4, 7f - i * 1.5f), AnimationWindow, (v) =>
            {
                t.transform.position += v;
            });
            var scale = 1f / (i + 1) + 0.5f;
            Utils.Animate(t.transform.localScale, new Vector3(scale, scale), AnimationWindow, (v) =>
            {
                t.transform.localScale += v;
            });
            Utils.Animate(t.color.a, 1f / (i + 1), AnimationWindow, (v) =>
            {
                var c = t.color;
                c.a += v;
                t.color = c;
            });
            t.gameObject.SetActive(true);
            i++;
        }
    }

    private void Awake()
    {
        TouchStatics();
        Prefab.PreloadPrefabs();
        Instance = this;

        _items = new List<MenuItem>
        {
            new MenuItem("PLAY", () =>
            {
                SceneManager.LoadScene(1);
                Level.IsFirstStart = true;
            }),
            new MenuItem("HIGH SCORES", () => { }),
            new MenuItem("QUIT", Application.Quit),
        };
        RefreshItems();
        
        const int offset = 1;
        for (var i = -offset; i <= offset; i++)
        {
            var square = GridSquare.Instantiate();
            square.transform.position = new Vector3(i, -0.7f, 0);
            square.transform.SetParent(transform);
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
}