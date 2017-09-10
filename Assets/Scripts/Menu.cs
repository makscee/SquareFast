using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private static readonly Prefab MenuItemPrefab = new Prefab("MenuItemText");
    public Canvas WorldCanvas;
    public static Menu Instance;

    private class MenuItem
    {
        public readonly Text Text;
        public readonly Action Action;

        public MenuItem(string text, Action action)
        {
            Action = action;
            var go = MenuItemPrefab.Instantiate();
            Text = go.GetComponent<Text>();
            Text.text = text;
            go.transform.SetParent((Instance).WorldCanvas.transform);
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

    private void OnEnable()
    {
        var gm = GridMarks.Instance;
        gm.SetSize(1);
        gm.SetBorders(-1, 1);
        gm.SetBorderHandlers(() =>
        {
            Player.Instance.TakeDmg(Player.Instance, 999);
            CameraScript.Instance.SwitchScene(Confirm);
        }, NextItem);
        if (Player.Instance == null)
        {
            Player.Prefab.Instantiate();
        }
    }

    private void Awake()
    {
        Prefab.TouchStatics();
        Prefab.PreloadPrefabs();
        Instance = this;

        _items = new List<MenuItem>
        {
            new MenuItem("PLAY", () =>
            {
                gameObject.SetActive(false);
                Level.Instance.gameObject.SetActive(true);
            }),
            new MenuItem("HIGH SCORES", () => SceneManager.LoadScene(0)),
            new MenuItem("QUIT", Application.Quit)
        };
        RefreshItems();
    }
}