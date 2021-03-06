using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct PatternImages
{
    public List<RawImage> Images;
}
[Serializable]
public struct PatternsImages
{
    public List<PatternImages> Levels;
}

public class Pattern : MonoBehaviour
{
    public static Pattern Instance;
    public List<PatternsImages> Patterns;
    private int _curLevel = 0;
    private int _patterns = 0;
    private Vector3 _scale = Vector3.one;

    private void Awake()
    {
        Instance = this;
        var t = Screen.width * 1f / 1600;
        _scale = new Vector3(t, t, t);
    }

    public void NextLevel(int skip = 1)
    {
        if (_patterns == 6)
        {
            return;
        }
        if (Level.GameOver)
        {
            return;
        }
        _curLevel += skip;
        var c = 1;
        foreach (var images in Patterns[_patterns].Levels)
        {
            if (c > _curLevel) break;
            c++;
            foreach (var image in images.Images)
            {
                if (!image.gameObject.activeSelf)
                {
                    var from = _patterns == 1 ? new Vector3(0, 1, 1) : Vector3.zero;
                    image.rectTransform.localScale = from;
                    Utils.Animate(from, _scale, Level.TickTime / 2, (v) => image.rectTransform.localScale += v);
                }
                image.gameObject.SetActive(true);
            }
        }
    }

    public void SetPatterns(int l)
    {
        _patterns = l - 1;
    }

    public void Reset()
    {
        Camera.main.orthographicSize = 5.87f;
        _curLevel = 0;
        foreach (var patterns in Patterns)
        {
            foreach (var images in patterns.Levels)
            {
                foreach (var image in images.Images)
                {
                    image.gameObject.SetActive(false);
                }
            }
        }
    }

    public void TickUpdate()
    {
        if (_patterns == 6)
        {
            return;
        }
        if (Level.Ticks % 3 != 0)
        {
            return;
        }
        var c = 1;
        foreach (var images in Patterns[_patterns].Levels)
        {
            if (c > _curLevel) break;
            c++;
            foreach (var image in images.Images)
            {
                var t = _scale.x * 1.15f + c * 0.05f;
                var from = new Vector3(t, t, t);
                var to = _scale;
                var tImage = image;
                tImage.rectTransform.localScale = from;
                Utils.Animate(from, to, Level.TickTime * 2, (v) => tImage.rectTransform.localScale += v);
            }
        }
    }
}