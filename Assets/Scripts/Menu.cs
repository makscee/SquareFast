using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private static readonly Prefab MenuItemPrefab = new Prefab("MenuItemText");
    public Canvas WorldCanvas;
    public static Menu Instance;
    public Text ControlsText;
    public GameObject HintCanvas;
    public Text HintText;
    public AudioClip Click, Select;
    private AudioSource _as;
    private static bool _firstEnable = true;

    private class MenuItem
    {
        public readonly Text Text;
        public readonly Action Action, OnSelect;
        public readonly float Scale;
        public readonly bool Fade;

        public MenuItem(string text, Action action, Action onSelect = null, float scale = 1, bool fade = true)
        {
            Action = action;
            var go = MenuItemPrefab.Instantiate();
            Text = go.GetComponent<Text>();
            Text.text = text;
            go.transform.SetParent((Instance).WorldCanvas.transform);
            OnSelect = onSelect ?? (() => { });
            Scale = scale;
            Fade = fade;
        }
    }

    private List<MenuItem> _items;

    private void SwitchItems(List<MenuItem> items)
    {
        foreach (var item in _items)
        {
            item.Text.gameObject.SetActive(false);
        }
        _items = items;
        
        var p = Pattern.Instance;
        p.Reset();
        UnitedTint.Tint = Color.white;
        Camera.main.backgroundColor = Color.black;
        RefreshItems(true);
        OnEnable();
    }

    public void PlayClick()
    {
        _as.clip = Click;
        _as.Play();
    }

    public void PlaySelect()
    {
        _as.clip = Select;
        _as.Play();
    }

    public void NextItem()
    {
        PlayClick();
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

    public bool CanvasHidden = false;
    private void Update()
    {
        if (!CanvasHidden && (Input.anyKeyDown || Input.touchCount > 0))
        {
            CanvasHidden = true;
            CameraScript.Instance.SwitchScene(() =>
            {
                HintCanvas.SetActive(false);
                var c = ControlsText.color;
                c.a = 1;
                var ut = ControlsText.GetComponent<UnitedTint>();
                ut.Color = c;
                c = new Color(c.r, c.g, c.b, 0);
                Utils.Animate(1f, 0f, 3f, (v) =>
                {
                    c.a = v;
                    ut.Color = c;
                }, null, true, 2f);
            });
        }
        foreach (var item in _items)
        {
            if (!item.Fade)
            {
                item.Text.gameObject.SetActive(item.Text.GetComponent<UnitedTint>().Color.a > 0.5f);
            }
        }
    }
    
    public void RefreshItems(bool initial = false)
    {
        var i = 0;
        _items[0].OnSelect();
        const float offset = 1f;
        if (initial)
        {
            foreach (var menuItem in _items)
            {
                var t = menuItem.Text;
                t.transform.position = new Vector3(i * 5 + offset, 8.5f - i * 2f);
                var scale = menuItem.Scale / (i + 1) + 0.5f;
                t.transform.localScale = new Vector3(scale, scale, 1);
                var c = t.color;
                c.a = menuItem.Fade ? 1f / (i + 1) : (i == 0 ? 1 : 0);
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
            Utils.Animate(new Vector3(ib * 5 + offset, 8.5f - ib * 2f), new Vector3(i * 5 + offset, 8.5f - i * 2f), AnimationWindow, (v) =>
            {
                t.transform.position += v;
            });
            var scale = menuItem.Scale / (i + 1) + 0.5f;
            var scaleb = menuItem.Scale / (ib + 1) + 0.5f;
            Utils.Animate(new Vector3(scaleb, scaleb), new Vector3(scale, scale), AnimationWindow, (v) =>
            {
                t.transform.localScale += v;
            });
            var ut = t.GetComponent<UnitedTint>();
            Utils.Animate(menuItem.Fade ? 1f / (ib + 1) : (ib == 0 ? 1 : 0),
                menuItem.Fade ? 1f / (i + 1) : (i == 0 ? 1 : 0), AnimationWindow, (v) =>
            {
                var c = ut.Color;
                c.a += v;
                ut.Color = c;
            });
            i++;
        }
    }

    public Action Enter = () => { };
    private void OnEnable()
    {
        Enter();
        Level.IsFirstStart = true;
        var gm = GridMarks.Instance;
        var dist = _firstEnable ? 3 : 1;
        gm.Set("< NEXT", "CONFIRM >", -dist, dist, -dist, dist, NextItem, () =>
        {
            CameraScript.Instance.SwitchScene(Confirm);
        }, true);
        _firstEnable = false;
        if (Player.Instance == null)
        {
            Player.Prefab.Instantiate();
        }
    }

    private List<MenuItem> getSecondList()
    {
        return new List<MenuItem>
        {
            new MenuItem("LEVEL 1", () =>
            {
                Level.CurrentLevel = 0;
                gameObject.SetActive(false);
                Level.Instance.gameObject.SetActive(true);
            }, p0),
            new MenuItem("LEVEL 2", () =>
            {
                Level.CurrentLevel = 1;
                gameObject.SetActive(false);
                Level.Instance.gameObject.SetActive(true);
            }, p1),
            new MenuItem("BACK", () => SwitchItems(getFirstList()), pReset)
        };
    }

    private List<MenuItem> getFirstList()
    {
        return new List<MenuItem>
        {
            new MenuItem("PLAY", () =>
            {
                SwitchItems(getSecondList());
            }),
            new MenuItem("HIGH SCORES", () =>
            {
                SwitchItems(new List<MenuItem>
                {
                    new MenuItem(HighScores.GetString(0), () => SwitchItems(getFirstList()), p0, 0.5f, false),
                    new MenuItem(HighScores.GetString(1), () => SwitchItems(getFirstList()), p1, 0.5f, false),
                    new MenuItem("BACK", () => SwitchItems(getFirstList()), pReset, 1f, false)
                });
            }),
            new MenuItem("QUIT", Application.Quit)
        };
    }

    Action p0 = () =>
        {
            var p = Pattern.Instance;
            p.SetPatterns(1);
            p.Reset();
            p.NextLevel(2);
            UnitedTint.Tint = new Color(1f, 0.63f, 0.31f);
            CameraScript.ChangeColorTinted(UnitedTint.Tint);
        },
        p1 = () =>
        {
            var p = Pattern.Instance;
            p.SetPatterns(2);
            p.Reset();
            p.NextLevel(2);
            UnitedTint.Tint = new Color(1f, 0.51f, 0.69f);
            CameraScript.ChangeColorTinted(UnitedTint.Tint);
        },
        pReset = () =>
        {
            var p = Pattern.Instance;
            p.Reset();
            UnitedTint.Tint = Color.white;
            Camera.main.backgroundColor = Color.black;
        };

    private void Awake()
    {
        _as = CameraScript.Instance.GetComponent<AudioSource>();
        if (Application.platform == RuntimePlatform.Android)
        {
            HintText.text =
                "<color=white>SQUARE FAST</color> CONTROLS ONLY WITH\n<color=white>LEFT</color> AND <color=white>RIGHT</color> TAP OF SCREEN\nEVEN IN MENU\n\n<color=white>TAP ANYWHERE TO CONTINUE</color>";
        }
        else
        {
            HintText.text =
                "<color=white>SQUARE FAST</color> CONTROLS ONLY WITH\nBUTTONS <color=white>LEFT</color> AND <color=white>RIGHT</color>\nEVEN IN MENU\n\n<color=white>PRESS ANY KEY TO CONTINUE</color>";
        }
        Saves.Load();
        WebUtils.FetchScores();
        Prefab.TouchStatics();
        Prefab.PreloadPrefabs();
        Instance = this;

        _items = getFirstList();
        
        RefreshItems(true);
    }
}