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
        public readonly Action Action, OnSelect;

        public MenuItem(string text, Action action, Action onSelect = null)
        {
            Action = action;
            var go = MenuItemPrefab.Instantiate();
            Text = go.GetComponent<Text>();
            Text.text = text;
            go.transform.SetParent((Instance).WorldCanvas.transform);
            OnSelect = onSelect ?? (() => { });
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
    private void RefreshItems(bool initial = false)
    {
        var i = 0;
        _items[0].OnSelect();
        if (initial)
        {
            foreach (var menuItem in _items)
            {
                var t = menuItem.Text;
                t.transform.position = new Vector3(i * 4, 7f - i * 1.5f);
                var scale = 1f / (i + 1) + 0.5f;
                t.transform.localScale = new Vector3(scale, scale, 1);
                var c = t.color;
                c.a = 1f / (i + 1);
                t.color = c;
                t.gameObject.SetActive(true);
                i++;
            }
            return;
        }
        foreach (var menuItem in _items)
        {
            var t = menuItem.Text;
            var ib = (i + 1) % _items.Count;
            Utils.Animate(new Vector3(ib * 4, 7f - ib * 1.5f), new Vector3(i * 4, 7f - i * 1.5f), AnimationWindow, (v) =>
            {
                t.transform.position += v;
            });
            var scale = 1f / (i + 1) + 0.5f;
            var scaleb = 1f / (ib + 1) + 0.5f;
            Utils.Animate(new Vector3(scaleb, scaleb), new Vector3(scale, scale), AnimationWindow, (v) =>
            {
                t.transform.localScale += v;
            });
            Utils.Animate(1f / (ib + 1), 1f / (i + 1), AnimationWindow, (v) =>
            {
                var c = t.color;
                c.a += v;
                t.color = c;
            });
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
        
        var secondList = new List<MenuItem>
        {
            new MenuItem("LEVEL 1", () =>
            {
                gameObject.SetActive(false);
                Level.Instance.gameObject.SetActive(true);
            }, () =>
            {
                var p = Pattern.Instance;
                p.SetPatterns(1);
                p.Reset();
                p.NextLevel(2);
                CameraScript.Instance.SwitchColor = Color.white;
            }),
            new MenuItem("LEVEL 2", () =>
            {
                gameObject.SetActive(false);
                Level.Instance.gameObject.SetActive(true);
            }, () =>
            {
                var p = Pattern.Instance;
                p.SetPatterns(2);
                p.Reset();
                p.NextLevel(2);
                CameraScript.Instance.SwitchColor = new Color(0.97f, 0.64f, 1f);
            }),
        };

        var firstList = new List<MenuItem>
        {
            new MenuItem("PLAY", () =>
            {
                foreach (var item in _items)
                {
                    item.Text.gameObject.SetActive(false);
                }
                _items = secondList;
                RefreshItems(true);
                OnEnable();
            }),
            new MenuItem("HIGH SCORES", () => SceneManager.LoadScene(0)),
            new MenuItem("QUIT", Application.Quit)
        };
        secondList.Add(new MenuItem("BACK", () =>
        {
            foreach (var item in _items)
            {
                item.Text.gameObject.SetActive(false);
            }
            _items = firstList;
            RefreshItems(true);
            OnEnable();
        }, () =>
        {
            var p = Pattern.Instance;
            p.Reset();
            CameraScript.Instance.SwitchColor = Color.white;
        }));

        _items = firstList;
        
        RefreshItems(true);
    }
}