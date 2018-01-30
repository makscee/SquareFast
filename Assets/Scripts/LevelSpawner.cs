using System;
using System.Collections.Generic;
using System.Linq;
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
        if (_produced % 2 != 0)
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
    private GameObject _lastSpawned;
    public void TickUpdate()
    {
        if (Level.Ticks % 3 != 0 || !_spawning) return;
        var next = _curPattern.GetNext();
        if (next != null)
        {
            _lastSpawned = next;
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
                Level.Instance.MusicStart = 9.7f;
                Level.Instance.MusicDelay = 2f;
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
                        new EnemyPattern().AddLeft(Square).AddRight(Square).AddLeft(Square).AddRight(Square)
                            .AddLeft(Square, 2).AddRight(Square, 2),
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
                Level.Instance.MusicStart = 16.3f;
                Level.Instance.MusicDelay = 2.5f;
                Level.TickTime = 60f / 135 / 3f;
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
                        new EnemyPattern().AddRight(Triangle, 2).AddLeft(null).SetRepeats(3),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3).SetRepeats(2),
                        new EnemyPattern().AddLeft(Rhombus, 2).AddRight(Rhombus, 2).SetRepeats(2),
                        new EnemyPattern().AddRight(Triangle, 2).AddLeft(null).SetRepeats(2),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(Rhombus, 2).AddRight(null).AddLeft(Square, 3),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 2).AddRight(Square, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3),
                        new EnemyPattern().AddRight(Rhombus, 3).AddLeft(Rhombus, 3).SetRepeats(2),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(Rhombus, 2).AddRight(null).AddLeft(Square, 3),
                        new EnemyPattern().AddRight(Triangle, 2).AddLeft(null).SetRepeats(2),
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
                Level.Instance.MusicStart = 9.7f;
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
                        new EnemyPattern().AddRight(Circle).AddLeft(null).AddLeft(Circle).AddRight(null),
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
                Level.Instance.MusicStart = 16.3f;
                Level.Instance.MusicDelay = 2.5f;
                Level.TickTime = 60f / 135 / 3f;
                _nextLevel = 4;
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
                        new EnemyPattern().AddRight(Triangle, 2).AddLeft(null).SetRepeats(3),
                        new EnemyPattern().AddLeft(DownTriangle).AddRight(Square, 3).SetRepeats(2),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3).SetRepeats(2),
                        new EnemyPattern().AddLeft(Rhombus, 3).AddRight(Rhombus, 2).SetRepeats(2),
                        new EnemyPattern().AddLeft(DownTriangle).AddRight(Rhombus, 3).SetRepeats(2),
                        new EnemyPattern().AddRight(Triangle, 2).AddLeft(null).SetRepeats(2),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(Rhombus, 3).AddRight(null).AddLeft(Square, 3),
                    },
                    new List<EnemyPattern>
                    {
                        new EnemyPattern().AddLeft(Square, 3).AddRight(Square, 3),
                        new EnemyPattern().AddRight(Rhombus, 3).AddLeft(Rhombus, 3).SetRepeats(2),
                        new EnemyPattern().AddLeft(DownTriangle).AddRight(Square, 3),
                        new EnemyPattern().AddLeft(Triangle, 2).AddRight(Rhombus, 2).AddRight(null).AddLeft(Square, 3),
                        new EnemyPattern().AddLeft(DownTriangle).AddRight(Triangle, 2).SetRepeats(2),
                        new EnemyPattern().AddRight(Triangle, 2).AddLeft(null).SetRepeats(2),
                    }
                };
                var p = Pattern.Instance;
                p.SetPatterns(2);
                p.Reset();
                p.NextLevel(3);
                break;
            }
            default: break;
        }
        UnitedTint.Tint = _switchColors[0];
        CameraScript.ChangeColorTinted(UnitedTint.Tint);
        _switchColors.RemoveAt(0);
        
        _curPattern = _patterns[_cl][_ci];
        var distTime = Distance * Level.TickTime * 3;
        const float sublevelTime = 15f;
        Action a = () =>
        {
            Utils.InvokeDelayed(() =>
            {
                if (Level.GameOver) return;

                if (_cl >= _patterns.Count - 1)
                {
                    _spawning = false;
                }
                
                if (_lastSpawned != null)
                {
                    CameraScript.Instance.SwitchFollow = _lastSpawned;
                    var u = _lastSpawned.GetComponent<Unit>();
                    u.DieEvent += () =>
                    {
                        Utils.InvokeDelayed(() =>
                        {
                            if (Level.GameOver) return;
                            var cs = CameraScript.Instance;
                            Utils.Animate(cs.SwitchProgress, 1f, 0.1f, (v) => cs.SwitchProgress = v, null, true);
                            Utils.InvokeDelayed(() =>
                            {
                                Utils.Animate(1f, 0f, 0.2f, (v) => cs.SwitchProgress = v, null, true);
                                if (_cl >= _patterns.Count)
                                {
                                    Level.CurrentLevel = _nextLevel;
                                    Level.NextLevelStart = true;
                                    Level.Instance.OnEnable();
                                    return;
                                }
                                UnitedTint.Tint = _switchColors[0];
                                CameraScript.ChangeColorTinted(UnitedTint.Tint);
                                _switchColors.RemoveAt(0);
                                Pattern.Instance.NextLevel();
                            }, 0.2f);
                        }, Level.TickTime * 2);
                    };
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
            }, sublevelTime - distTime);
        };
        StartAction = () =>
        {
            Utils.InvokeDelayed(() =>
            {
                a();
            }, sublevelTime, null, true);
            a();
        };
    }
}