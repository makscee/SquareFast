using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyPattern
{
    private int _length = 0;
    private readonly List<Prefab> _left = new List<Prefab>(), _right = new List<Prefab>();
    private readonly List<int> _leftHp = new List<int>(), _rightHp = new List<int>();

    public EnemyPattern AddLeft(Prefab p, int hp = 1)
    {
        _left.Add(p);
        _leftHp.Add(hp);
        _length++;
        return this;
    }
    public EnemyPattern AddRight(Prefab p, int hp = 1)
    {
        _right.Add(p);
        _rightHp.Add(hp);
        _length++;
        return this;
    }

    public EnemyPattern SetRepeats(int v)
    {
        _length *= v;
        return this;
    }

    private int _li, _ri, _produced;
    public GameObject GetNext()
    {
        GameObject go = null;
        if (_produced % 2 == 0)
        {
            if (_left[_li] != null)
            {
                go = _left[_li].Instantiate();
                go.transform.position = new Vector3(-LevelSpawner.Distance, 0, 0);
                go.GetComponent<Unit>().HP = _leftHp[_li];
            }
            _li = (_li + 1) % _left.Count;
        }
        else
        {
            if (_right[_ri] != null)
            {
                go = _right[_ri].Instantiate();
                go.transform.position = new Vector3(LevelSpawner.Distance, 0, 0);
                go.GetComponent<Unit>().HP = _rightHp[_ri];
            }
            _ri = (_ri + 1) % _right.Count;
        }
        _produced++;
        return go;
    }

    public bool Ended()
    {
        return _produced >= _length;
    }

    public void Reset()
    {
        _produced = 0;
        _li = 0;
        _ri = 0;
    }
}

public class LevelSpawner
{
    public static int Distance = 8;
    public const float SublevelTime = 15f;

    private static readonly Prefab Square = BasicEnemy.Prefab;
    private static readonly Prefab Triangle = TriangleEnemy.Prefab;
    private static readonly Prefab Rhombus = RhombusEnemy.Prefab;
    private static readonly Prefab Circle = CircleEnemy.Prefab;
    private static readonly Prefab DownTriangle = DownTriangleEnemy.Prefab;

    private List<List<EnemyPattern>> _patterns;
    private readonly List<Color> _switchColors = new List<Color>();

    public LevelSpawner(int level = 0)
    {
        InitLevel(level);
    }

    public Action StartAction;

    private EnemyPattern _curPattern;
    private int _ci = 0, _cl = 0, _nextLevel;
    private bool _spawning = true;
    private GameObject _lastSpawned1, _lastSpawned2;
    private bool _firstTick = true;
    public void TickUpdate()
    {
        if (Level.Ticks % 3 != 0 || !_spawning) return;
        if (_firstTick)
        {
            for (var i = Distance - 3; i >= 0; i--)
            {
                SpawnNext(i);
            }
            _firstTick = false;
            return;
        }
        SpawnNext();
    }

    private int _2hpTutorialSpawned = 0;
    private void SpawnNext(int centerOffset = 0)
    {
        var next = _curPattern.GetNext();
        if (next != null)
        {
            _lastSpawned2 = _lastSpawned1;
            _lastSpawned1 = next;
            var v = next.transform.position;
            v.x += v.x > 0 ? -centerOffset : centerOffset;
            next.transform.position = v;
            Unit u;
            if (Level.Tutorial && _2hpTutorialSpawned < 1 && (u = next.GetComponent<Unit>()).HP == 2)
            {
                _2hpTutorialSpawned++;
                var uh = UnitHint.CreateUnitText("^\n2 HP", u);
                Utils.Animate(new Color(0.3f, 0.3f, 0.3f), new Color(0.9f, 0.9f, 0.9f), 3f, (c) =>
                {
                    uh.Tint.Color = c;
                }, null, true);
            }
        }
        if (!_curPattern.Ended()) return;
        
        _curPattern.Reset();
        if (_cl >= _patterns.Count)
        {
            return;
        }
        _ci += Random.Range(1, _patterns[_cl].Count - 1);
        _ci %= _patterns[_cl].Count;
        _curPattern = _patterns[_cl][_ci];
    }

    private void InitLevel(int level)
    {
        Pattern.Instance.Reset();
        switch (level)
        {
            case 0:
            {
                Level.Instance.TickAction += () =>
                {
                    if (Level.Ticks % 3 == 0 && !Level.GameOver)
                    {
                        BeatBGEffect.Create(0);
                    }
                };
                Level.Instance.GetComponent<AudioSource>().clip = Level.Instance.L1;
                Level.Instance.MusicStart = 0f;
                Level.Instance.MusicDelay = 2.88f;
                Level.Instance.LevelBridge = 2.5f;
                _nextLevel = 3;
                
                Level.TickTime = 60f / 100 / 3f;
                
                _switchColors.Add(new Color(1f, 0.63f, 0.31f));
                _switchColors.Add(new Color(1f, 0.48f, 0.21f));
                _switchColors.Add(new Color(1f, 0.43f, 0f));
                _switchColors.Add(new Color(1f, 0.09f, 0f));
                
                
                _patterns = new List<List<EnemyPattern>>
                {
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square).AddRight(Square).AddLeft(Square).AddRight(Square).AddLeft(null)
                            .AddRight(Square, 2),
                        new EnemyPattern().AddLeft(Square).AddRight(Square).AddLeft(Square, 2).AddRight(Square).AddLeft(Square).AddRight(Square, 2),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square).AddRight(Square).AddLeft(null).AddRight(Rhombus, 2),
                        new EnemyPattern().AddLeft(Square).AddRight(Rhombus, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Square, 2).AddLeft(null, 2).AddRight(Rhombus, 2),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square).AddRight(Square).AddLeft(null).AddRight(Rhombus, 2),
                        new EnemyPattern().AddLeft(Square,2).AddRight(Square, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Rhombus, 2).AddRight(Rhombus, 2).SetRepeats(2),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square).AddRight(Square).SetRepeats(2),
                        new EnemyPattern().AddRight(Rhombus, 2).AddLeft(Rhombus, 2).SetRepeats(2),
                    }
                };
                break;
            }
            case 1:
            {
                Level.Instance.TickAction += () =>
                {
                    if (Level.Ticks % 3 == 0 && !Level.GameOver)
                    {
                        BeatBGEffect.Create(1);
                    }
                };
                Level.Instance.GetComponent<AudioSource>().clip = Level.Instance.L2;
//                Level.Instance.MusicStart = 13.8f;
                Level.Instance.MusicDelay = 2.0f;
                Level.Instance.LevelBridge = 2.5f;
                Level.TickTime = 60f / 130 / 3f;
                _nextLevel = 4;
                Distance = 5;
                
                _switchColors.Add(new Color(1f, 0.51f, 0.69f));
                _switchColors.Add(new Color(1f, 0.37f, 0.78f));
                _switchColors.Add(new Color(1f, 0.27f, 0.83f));
                _switchColors.Add(new Color(0.88f, 0f, 1f));
                
                
                _patterns = new List<List<EnemyPattern>>
                {
                    new List<EnemyPattern>
                    {
                        
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Square).AddLeft(Square, 2).AddRight(Square)
                            .AddLeft(Square, 2).AddRight(Square, 2),
                        new EnemyPattern().AddLeft(Rhombus, 2).AddRight(Square, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Rhombus, 2).AddLeft(Rhombus, 2).AddRight(Square, 2),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square).AddRight(Square).SetRepeats(2),
                        new EnemyPattern().AddLeft(null).AddRight(Triangle, 2).AddLeft(null).AddRight(Square).SetRepeats(1),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(null).SetRepeats(1),
                        new EnemyPattern().AddLeft(Rhombus, 2).AddRight(Square, 2).SetRepeats(1),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3).SetRepeats(1),
                        new EnemyPattern().AddLeft(Rhombus, 2).AddRight(Rhombus, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(null).AddRight(Triangle, 2).AddLeft(null).AddRight(Rhombus, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(null).AddRight(null).AddLeft(Square, 3),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Square, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3),
                        new EnemyPattern().AddLeft(Rhombus, 2).AddRight(Rhombus, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(null).AddRight(null).AddLeft(Square, 3),
                        new EnemyPattern().AddLeft(null).AddRight(Triangle, 2).AddLeft(null).AddRight(Rhombus, 2).SetRepeats(2),
                    }
                };
                break;
            }
            case 2:
            {
                Level.Instance.TickAction += () =>
                {
                    if (Level.Ticks % 3 == 0 && !Level.GameOver)
                    {
                        BeatBGEffect.Create(2);
                    }
                };
                Level.Instance.GetComponent<AudioSource>().clip = Level.Instance.L3;
                Level.Instance.MusicStart = 0f;
                Level.Instance.MusicDelay = 2f;
                Level.Instance.BeatOffset = 0.167f;
                Level.Instance.LevelBridge = 2.5f;
                
                Level.TickTime = 60f / 150 / 3f;
                _nextLevel = 5;
                Distance = 5;
                
                _switchColors.Add(new Color(1f, 0.9f, 0.54f));
                _switchColors.Add(new Color(1f, 0.98f, 0.4f));
                _switchColors.Add(new Color(0.86f, 0.81f, 0.24f));
                _switchColors.Add(new Color(1f, 0.83f, 0f));
                
                
                _patterns = new List<List<EnemyPattern>>
                {
                    new List<EnemyPattern>
                    {
                        
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Square).AddLeft(Square, 2).AddRight(Square)
                            .AddLeft(Square, 2).AddRight(Square, 2),
                        new EnemyPattern().AddLeft(Rhombus, 2).AddRight(Square, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Rhombus, 2).AddLeft(Rhombus, 2).AddRight(Square, 2),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square).AddRight(Square).SetRepeats(2),
                        new EnemyPattern().AddLeft(null).AddRight(Triangle, 2).AddLeft(null).AddRight(Square, 2).SetRepeats(3),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3).SetRepeats(2),
                        new EnemyPattern().AddLeft(Rhombus, 2).AddRight(Rhombus, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(null).SetRepeats(2),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(null).AddRight(Square, 2).AddLeft(Square, 3),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Square, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3),
                        new EnemyPattern().AddLeft(Rhombus, 3).AddRight(Rhombus, 3).SetRepeats(2),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(null).AddRight(null).AddLeft(Square, 3),
                        new EnemyPattern().AddLeft(null).AddRight(Triangle, 2).AddLeft(null).AddRight(Rhombus, 2).SetRepeats(2),
                    }
                };
                break;
            }
            case 3:
            {
                Level.Instance.TickAction += () =>
                {
                    if (Level.Ticks % 3 == 0 && !Level.GameOver)
                    {
                        BeatBGEffect.Create(0);
                    }
                };
                Level.Instance.GetComponent<AudioSource>().clip = Level.Instance.L1;
                Level.Instance.MusicStart = 0f;
                Level.Instance.MusicDelay = 2f;
                _nextLevel = 4;
                
                Level.TickTime = 60f / 100 / 3f;
                
                _switchColors.Add(new Color(0.56f, 0.59f, 1f));
                _switchColors.Add(new Color(0.36f, 0.45f, 1f));
                _switchColors.Add(new Color(0.22f, 0.26f, 1f));
                _switchColors.Add(new Color(0f, 0f, 1f));


                _patterns = new List<List<EnemyPattern>>
                {
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(null).AddRight(Circle).AddLeft(null).AddRight(Circle),
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Square, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3).SetRepeats(2),
                        new EnemyPattern().AddRight(Rhombus, 2).AddLeft(Rhombus, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3).SetRepeats(2),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddRight(Rhombus, 3).AddLeft(Rhombus, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3).SetRepeats(2),
                        new EnemyPattern().AddRight(Rhombus, 2).AddLeft(Rhombus, 2).SetRepeats(2),
                        new EnemyPattern().AddRight(Circle).AddLeft(Circle),
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Square, 2).SetRepeats(2),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddRight(Square, 3).AddLeft(Square, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Square, 3).SetRepeats(2),
                        new EnemyPattern().AddRight(Circle).AddLeft(Circle),
                        new EnemyPattern().AddRight(Rhombus, 3).AddLeft(Rhombus, 3).SetRepeats(2),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddRight(Square, 3).AddLeft(Square, 3).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3).SetRepeats(2),
                        new EnemyPattern().AddRight(Circle, 2).AddLeft(Square, 2),
                        new EnemyPattern().AddRight(Rhombus, 3).AddLeft(Rhombus, 3).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3).SetRepeats(2),
                    },
                };
                var p = Pattern.Instance;
                p.SetPatterns(1);
                p.Reset();
                p.NextLevel(3);
                break;
            }
            case 4:
            {
                Level.Instance.TickAction += () =>
                {
                    if (Level.Ticks % 3 == 0 && !Level.GameOver)
                    {
                        BeatBGEffect.Create(1);
                    }
                };
                Level.Instance.GetComponent<AudioSource>().clip = Level.Instance.L2;
                Level.Instance.MusicStart = 0f;
                Level.Instance.MusicDelay = 2.5f;
                Level.TickTime = 60f / 135 / 3f;
                _nextLevel = 5;
                Distance = 5;
                
                _switchColors.Add(new Color(0.46f, 1f, 0.69f));
                _switchColors.Add(new Color(0.29f, 1f, 0.65f));
                _switchColors.Add(new Color(0.24f, 1f, 0.40f));
                _switchColors.Add(new Color(0f, 1f, 0.16f));


                _patterns = new List<List<EnemyPattern>>
                {
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Square).AddLeft(Square, 2).AddRight(Square),
                        new EnemyPattern().AddLeft(Rhombus, 3).AddRight(Square, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Rhombus, 2).AddLeft(Rhombus, 2).AddRight(Square, 2),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(DownTriangle).AddRight(DownTriangle),
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Square, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(null).SetRepeats(3),
                        new EnemyPattern().AddLeft(DownTriangle).AddRight(Square, 3).SetRepeats(2),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3).SetRepeats(2),
                        new EnemyPattern().AddLeft(Rhombus, 3).AddRight(Rhombus, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(DownTriangle).AddRight(Rhombus, 3).SetRepeats(2),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(Rhombus, 3).AddRight(null).AddLeft(Square, 3),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3),
                        new EnemyPattern().AddLeft(Rhombus, 3).AddRight(Rhombus, 3).SetRepeats(2),
                        new EnemyPattern().AddLeft(DownTriangle).AddRight(Square, 3),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(Rhombus, 2).AddRight(null).AddLeft(Square, 3),
                        new EnemyPattern().AddLeft(DownTriangle).AddRight(Triangle, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Rhombus, 2).AddRight(Square, 2).SetRepeats(2),
                    }
                };
                var p = Pattern.Instance;
                p.SetPatterns(2);
                p.Reset();
                p.NextLevel(3);
                break;
            }
            case 5:
            {
                Level.Instance.TickAction += () =>
                {
                    if (Level.Ticks % 3 == 0 && !Level.GameOver)
                    {
                        BeatBGEffect.Create(2);
                    }
                };
                if (!Level.NextLevelStart)
                {
                    Level.Instance.GetComponent<AudioSource>().clip = Level.Instance.L3;
                    Level.Instance.MusicStart = 0f;
                    Level.Instance.MusicDelay = 2.5f;
                    Level.TickTime = 60f / 150 / 3f;
                }
                _nextLevel = 6;
                Distance = 5;
                
                _switchColors.Add(new Color(0.81f, 0.78f, 1f));
                _switchColors.Add(new Color(0.77f, 0.64f, 1f));
                _switchColors.Add(new Color(0.73f, 0.47f, 1f));
                _switchColors.Add(new Color(0.55f, 0.25f, 0.79f));


                _patterns = new List<List<EnemyPattern>>
                {
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Square).AddLeft(Square, 2).AddRight(Square),
                        new EnemyPattern().AddLeft(Rhombus, 3).AddRight(Square, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Rhombus, 2).AddLeft(Rhombus, 2).AddRight(Square, 2),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(DownTriangle).AddRight(DownTriangle),
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Square, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(null).SetRepeats(3),
                        new EnemyPattern().AddLeft(DownTriangle).AddRight(Square, 3).SetRepeats(2),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3).SetRepeats(2),
                        new EnemyPattern().AddLeft(Rhombus, 3).AddRight(Rhombus, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(DownTriangle).AddRight(Rhombus, 3).SetRepeats(2),
                        new EnemyPattern().AddLeft(null).AddRight(Triangle, 2).AddLeft(null).AddRight(Square).SetRepeats(1),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(null).AddRight(Rhombus, 3).AddLeft(Square, 3),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3),
                        new EnemyPattern().AddLeft(Rhombus, 3).AddRight(Rhombus, 3).SetRepeats(2),
                        new EnemyPattern().AddLeft(DownTriangle).AddRight(Square, 3),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(null).AddLeft(Square, 3).AddRight(Square),
                        new EnemyPattern().AddLeft(DownTriangle).AddRight(Triangle, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square).AddRight(Triangle, 2).AddLeft(null).AddRight(Square).SetRepeats(1),
                    }
                };
                var p = Pattern.Instance;
                p.SetPatterns(3);
                p.Reset();
                p.NextLevel(3);
                break;
            }
            case 6:
            {
                Level.Instance.GetComponent<AudioSource>().clip = Level.Instance.L2;
                Level.Instance.MusicStart = 0f;
                Level.Instance.MusicDelay = 2.5f;
                Level.TickTime = 60f / 160 / 3f;
                _nextLevel = 6;
                Distance = 3;
                
                UnitedTint.Tint = Color.white;
                Camera.main.backgroundColor = Color.black;

                _patterns = new List<List<EnemyPattern>>
                {
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Rhombus, 3),
                        new EnemyPattern().AddLeft(DownTriangle, 1).AddRight(Square, 3),
                        new EnemyPattern().AddLeft(Circle, 2).AddRight(Square, 2),
                        new EnemyPattern().AddLeft(Rhombus, 3).AddRight(Rhombus, 2),
                    },
                };
                var p = Pattern.Instance;
                p.SetPatterns(7);
                p.Reset();
                Camera.main.orthographicSize /= 1.5f;
                CameraScript.Instance.SwitchFollow1 = null;
                CameraScript.Instance.SwitchFollow2 = null;
                CameraScript.Instance.SwitchProgress = 0f;
                break;
            }
        }
        if (_switchColors.Count > 0)
        {
            UnitedTint.Tint = _switchColors[0];
            CameraScript.ChangeColorTinted(UnitedTint.Tint);
            _switchColors.RemoveAt(0);
        }

        _curPattern = _patterns[_cl][_ci];
        var distTime = Distance * Level.TickTime * 3;
        Action a = () =>
        {
            Utils.InvokeDelayed(() =>
            {
                if (Level.GameOver) return;
                if (Level.CurrentLevel == 6) return;

                if (_cl >= _patterns.Count - 1)
                {
                    _spawning = false;
                }
                Action dieEvent = () => Utils.InvokeDelayed(() =>
                {
                    if (Level.GameOver) return;
                    if (CameraScript.Instance.SwitchFollow1 != null || CameraScript.Instance.SwitchFollow2 != null)
                    {
                        return;
                    }
                    var cs = CameraScript.Instance;
                    Utils.Animate(cs.SwitchProgress, 1f, 0.1f, (v) => cs.SwitchProgress = v, null, true);
                    var nextLevel = _cl >= _patterns.Count;
                    if (nextLevel)
                    {
                        ProgressLine.Instance.Updating = false;
                        Debug.LogWarning("STOP UPDATING");
                        Utils.InvokeDelayed(() =>
                        {                            
                            var p = Pattern.Instance;
                            p.Reset();
                            p.NextLevel(2);
                        }, Level.Instance.LevelBridge / 4);
                        Utils.InvokeDelayed(() =>
                        {
                            var p = Pattern.Instance;
                            p.Reset();
                            p.NextLevel(1);
                        }, Level.Instance.LevelBridge / 2);
                        Utils.InvokeDelayed(() =>
                        {    
                            var p = Pattern.Instance;
                            p.Reset();
                        }, Level.Instance.LevelBridge / 4 * 3);
                        Utils.InvokeDelayed(() =>
                        {
                            Level.CurrentLevel = _nextLevel;
                            Level.NextLevelStart = true;
                            Level.Instance.OnEnable();
                            ProgressLine.Instance.Reset();
                        }, Level.Instance.LevelBridge);
                    }
                    Utils.InvokeDelayed(() =>
                    {
                        Utils.Animate(1f, 0f, 0.2f, (v) => cs.SwitchProgress = v, null, true);
                        if (Level.CurrentLevel == 0)
                        {
                            if (_cl == 1)
                            {
                                GridMarks.Instance.RemoveTint(-1);
                                GridMarks.Instance.RemoveTint(1);
                                GridMarks.Instance.SetBorders(-1, 1);
                                GridMarks.Instance.DisplayBorders(true, false);
                            } else if (_cl == 2)
                            {
                                GridMarks.Instance.DisplayBorders(false);
                                GridMarks.Instance.LeftSolid = false;
                                GridMarks.Instance.RightSolid = false;
                                Action ad = () => Player.Instance.TakeDmg(Player.Instance, 999);
                                GridMarks.Instance.HandlerLeft = ad;
                                GridMarks.Instance.HandlerRight = ad;
                            }
                        }
//                        ProgressLine.Create(-1f + (-0.2f * _cl));
                        if (nextLevel)
                        {
                            return;
                        }
                        ProgressLine.Instance.StartNext();
                        if (_switchColors.Count > 0)
                        {
                            UnitedTint.Tint = _switchColors[0];
                            CameraScript.ChangeColorTinted(UnitedTint.Tint);
                            _switchColors.RemoveAt(0);
                        }
                        else
                        {
                            Debug.LogError("Switch colors empty! " + _cl + " " + _ci);
                        }
                        Pattern.Instance.NextLevel();
                    }, 0.2f);
                }, Level.TickTime * 2);
                if (_lastSpawned1 != null || _lastSpawned2 != null)
                {
                    CameraScript.Instance.SwitchFollow1 = _lastSpawned1;
                    CameraScript.Instance.SwitchFollow2 = _lastSpawned2;
                    _lastSpawned1.GetComponent<Unit>().DieEvent += dieEvent;
                    _lastSpawned2.GetComponent<Unit>().DieEvent += dieEvent;
                }

                _cl++;
                _ci = 0;
                if (_cl >= _patterns.Count)
                {
                    return;
                }
                _curPattern = _patterns[_cl][_ci];
                _spawning = false;
                Utils.InvokeDelayed(() => _spawning = true, distTime / 2);
            }, SublevelTime - distTime);
        };
        StartAction = () =>
        {
            ProgressLine.Instance.StartNext();
            Utils.InvokeDelayed(() =>
            {
                a();
            }, SublevelTime, null, true);
            a();
        };
    }
}