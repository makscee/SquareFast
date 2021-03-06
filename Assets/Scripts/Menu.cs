using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private static readonly Prefab MenuItemPrefab = new Prefab("MenuItemText");
    public Canvas WorldCanvas;
    public static Menu Instance;
    public GameObject HintCanvas;
    public Text HintText, PressAnyKeyText, BestTimeText;
    public AudioClip Click, Select;
    public GameObject LockIcon;
    private AudioSource _as;
    private static bool _inHs = false;

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
            go.transform.position = Vector3.zero;
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
            Destroy(item.Text.gameObject);
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

    public bool CanvasHidden;
    private float _t;
    private bool _canvasHideStarted;
    private void Update()
    {
        if (Player.Instance != null)
        {
            WorldCanvas.transform.position = Player.Instance.transform.position;
            BestTimeText.transform.position =
                BestTimeText.transform.position.SetX(Player.Instance.transform.position.x);
        }
        if (!CanvasHidden)
        {
            _t += Time.deltaTime;
            var k = (float)Math.Abs(Math.Sin(_t * 5) / 6) + 1f;
            PressAnyKeyText.transform.localScale = new Vector3(k, k);
        }
        if (!_canvasHideStarted && !CanvasHidden && (Input.anyKeyDown || Input.touchCount > 0))
        {
            _canvasHideStarted = true;
            CameraScript.Instance.SwitchScene(() =>
            {
                var p = Pattern.Instance; 
                p.Reset();
                UnitedTint.Tint = Color.white;
                if (Level.Tutorial)
                {
//                    Destroy(Player.Instance.gameObject);
                    Level.CurrentLevel = 0;
                    gameObject.SetActive(false);
                    Level.Instance.gameObject.SetActive(true);
                }
                CanvasHidden = true;
                HintCanvas.SetActive(false);
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
        const float scaleT = 0.2f;
        const float fadeT = 3f;
        const float xT = 3.5f;
        if (initial)
        {
            foreach (var menuItem in _items)
            {
                var t = menuItem.Text;
                t.transform.position = new Vector3(i * xT + offset, 8.5f - i * 2f);
                var scale = menuItem.Scale / (i + 1) + scaleT;
                t.transform.localScale = new Vector3(scale, scale, 1);
                var c = t.color;
                c.a = menuItem.Fade ? 1f / (i * fadeT + 1) : (i == 0 ? 1 : 0);
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
            Utils.Animate(new Vector3(ib * xT + offset, 8.5f - ib * 2f), new Vector3(i * xT + offset, 8.5f - i * 2f), AnimationWindow, (v) =>
            {
                t.transform.position += v;
            }, null, false, 0f, InterpolationType.InvSquare);
            var scale = menuItem.Scale / (i + 1) + scaleT;
            var scaleb = menuItem.Scale / (ib + 1) + scaleT;
            Utils.Animate(new Vector3(scaleb, scaleb), new Vector3(scale, scale), AnimationWindow, (v) =>
            {
                t.transform.localScale += v;
            }, null, false, 0f, InterpolationType.Square);
            var ut = t.GetComponent<UnitedTint>();
            Utils.Animate(menuItem.Fade ? 1f / (ib * fadeT + 1) : (ib == 0 ? 1 : 0),
                menuItem.Fade ? 1f / (i * fadeT + 1) : (i == 0 ? 1 : 0), AnimationWindow, (v) =>
            {
                var c = ut.Color;
                c.a += v;
                ut.Color = c;
            }, null, false, 0f, InterpolationType.Square);
            i++;
        }
    }

    public Action Enter = () => { };
    private void OnEnable()
    {
        Enter();
        Level.IsFirstStart = true;
        var gm = GridMarks.Instance;
        var dist = 1;
        gm.Set("< NEXT", _inHs ? "BACK >" : "CONFIRM >", -dist, dist, -dist, dist, NextItem, () =>
        {
            CameraScript.Instance.SwitchScene(Confirm);
        }, true);
        if (Player.Instance == null && !Level.Tutorial)
        {
            Player.Prefab.Instantiate();
        }
        gm.DisplayBorders(false);
    }

    private List<MenuItem> getSecondList()
    {
        Instance.BestTimeText.gameObject.SetActive(true);
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
            new MenuItem("LEVEL 3", () =>
            {
                Level.CurrentLevel = 2;
                gameObject.SetActive(false);
                Level.Instance.gameObject.SetActive(true);
            }, p2),
            new MenuItem("LEVEL 4", () =>
            {
                Level.CurrentLevel = 3;
                gameObject.SetActive(false);
                Level.Instance.gameObject.SetActive(true);
            }, p3),
            new MenuItem("LEVEL 5", () =>
            {
                Level.CurrentLevel = 4;
                gameObject.SetActive(false);
                Level.Instance.gameObject.SetActive(true);
            }, p4),
            new MenuItem("LEVEL 6", () =>
            {
                Level.CurrentLevel = 5;
                gameObject.SetActive(false);
                Level.Instance.gameObject.SetActive(true);
            }, p5),
            new MenuItem("LEVEL 7", () =>
            {
                Level.CurrentLevel = 6;
                gameObject.SetActive(false);
                Level.Instance.gameObject.SetActive(true);
            }, p6),
            new MenuItem("BACK", () => SwitchItems(getFirstList()), pReset)
        };
    }

    private List<MenuItem> getFirstList()
    {
        Instance.BestTimeText.gameObject.SetActive(false);
        _inHs = false;
        Instance.LockIcon.SetActive(false);
        return new List<MenuItem>
        {
            new MenuItem("PLAY", () =>
            {
                SwitchItems(getSecondList());
            }),
            new MenuItem("HIGH SCORES", () =>
            {
                _inHs = true;
                WebUtils.FetchScores();
                var item0 = new MenuItem(HighScores.GetString(0), () => SwitchItems(getFirstList()), p0, 0.5f, false); 
                var item1 = new MenuItem(HighScores.GetString(1), () => SwitchItems(getFirstList()), p1, 0.5f, false);
                var item2 = new MenuItem(HighScores.GetString(2), () => SwitchItems(getFirstList()), p2, 0.5f, false);
                var item3 = new MenuItem(HighScores.GetString(3), () => SwitchItems(getFirstList()), p3, 0.5f, false);
                var item4 = new MenuItem(HighScores.GetString(4), () => SwitchItems(getFirstList()), p4, 0.5f, false);
                var item5 = new MenuItem(HighScores.GetString(5), () => SwitchItems(getFirstList()), p5, 0.5f, false);
                var item6 = new MenuItem(HighScores.GetString(6), () => SwitchItems(getFirstList()), p6, 0.5f, false);
                HighScores.WhenFetched[0] = () => item0.Text.text = HighScores.GetString(0);
                HighScores.WhenFetched[1] = () => item1.Text.text = HighScores.GetString(1);
                HighScores.WhenFetched[2] = () => item2.Text.text = HighScores.GetString(2);
                HighScores.WhenFetched[3] = () => item3.Text.text = HighScores.GetString(3);
                HighScores.WhenFetched[4] = () => item4.Text.text = HighScores.GetString(4);
                HighScores.WhenFetched[5] = () => item5.Text.text = HighScores.GetString(5);
                HighScores.WhenFetched[6] = () => item6.Text.text = HighScores.GetString(6);
                SwitchItems(new List<MenuItem>
                {
                    item0,
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                });
            }),
            new MenuItem("RESET", () =>
            {
                PlayerData.Instance = new PlayerData();
                var pd = PlayerData.Instance;
                for (var i = 0; i < pd.Scores.Length; i++)
                {
                    PlayerPrefs.SetString("score" + i, "0");
                }
                UnitedTint.Tint = Color.white;
                Saves.Save();
                SceneManager.LoadScene(0);
                Time.timeScale = 1;
            }),
            new MenuItem("QUIT", Application.Quit)
        };
    }

    private static void LevelSelect(int l, int patterns)
    {
        var p = Pattern.Instance;
        p.SetPatterns(patterns);
        p.Reset();
        p.NextLevel(2);
        if (float.Parse(PlayerData.Instance.Scores[l]) > 0f)
        {
            Instance.BestTimeText.text = PlayerData.Instance.Scores[l];
        }
        else
        {
            Instance.BestTimeText.text = "0.00";
        }
    }

    private Action p0 = () =>
        {
            LevelSelect(0, 1);
            UnlockLevel(new Color(1f, 0.63f, 0.31f));
            CameraScript.ChangeColorTinted(UnitedTint.Tint);
        },
        p1 = () =>
        {
            LevelSelect(1, 2);
            UnlockLevel(new Color(1f, 0.51f, 0.69f));
            CameraScript.ChangeColorTinted(UnitedTint.Tint);
        },
        p2 = () =>
        {
            LevelSelect(2, 3);
            UnlockLevel(new Color(1f, 0.9f, 0.54f));
            CameraScript.ChangeColorTinted(UnitedTint.Tint);
        },
        p3 = () =>
        {
            var locked = float.Parse(PlayerData.Instance.Scores[0]) < 60f;
            LevelSelect(3, 1);
            if (locked) LockLevel();
            else UnlockLevel(new Color(0.56f, 0.59f, 1f));
            CameraScript.ChangeColorTinted(UnitedTint.Tint);
        },
        p4 = () =>
        {
            var locked = float.Parse(PlayerData.Instance.Scores[1]) < 60f;
            LevelSelect(4, 2);
            if (locked) LockLevel();
            else UnlockLevel(new Color(0.46f, 1f, 0.69f));
            CameraScript.ChangeColorTinted(UnitedTint.Tint);
        },
        p5 = () =>
        {
            var locked = float.Parse(PlayerData.Instance.Scores[2]) < 60f;
            LevelSelect(5, 3);
            if (locked) LockLevel();
            else UnlockLevel(new Color(0.81f, 0.78f, 1f));
            CameraScript.ChangeColorTinted(UnitedTint.Tint);
        },
        p6 = () =>
        {
            var locked = float.Parse(PlayerData.Instance.Scores[5]) < 60f;
            LevelSelect(6, 7);
            if (locked) LockLevel();
            else
            {
                UnlockLevel(Color.white);
                Camera.main.backgroundColor = Color.black;
            }
        },
        pReset = () =>
        {
            var p = Pattern.Instance;
            p.Reset();
            UnlockLevel(Color.white);
            Camera.main.backgroundColor = Color.black;
            Instance.BestTimeText.text = "";
        };

    private static void LockLevel()
    {
        UnitedTint.Tint = new Color(0.57f, 0.57f, 0.57f);
        Instance.LockIcon.SetActive(true);
        if (_inHs) return;
        var gm = GridMarks.Instance;
        gm.SetText("< NEXT", "");
        gm.SetBorderHandlers(Instance.NextItem, () => { });
        gm.RightSolid = true;
    }

    private static void UnlockLevel(Color c)
    {
        UnitedTint.Tint = c;
        Instance.LockIcon.SetActive(false);
        if (_inHs) return;
        var gm = GridMarks.Instance;
        gm.SetText("< NEXT", "CONFIRM >");
        gm.SetBorderHandlers(Instance.NextItem, () =>
        {
            CameraScript.Instance.SwitchScene(Instance.Confirm);
        });
        gm.RightSolid = false;
    }
    private void Awake()
    {
        _as = CameraScript.Instance.GetComponent<AudioSource>();
//        HintCanvas.SetActive(true);
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
        Level.Tutorial = !PlayerData.Instance.TutorialComplete;
        Prefab.TouchStatics();
        Prefab.PreloadPrefabs();
        
        Instance = this;
        if (Level.Tutorial)
        {
            _items = getSecondList();
        }
        else
        {
            _items = getFirstList();
            RefreshItems(true);
        }
        var p = Pattern.Instance; 
        p.SetPatterns(1);
        p.Reset();
        p.NextLevel(2);
        UnitedTint.Tint = new Color(1f, 0.63f, 0.31f);
    }
}