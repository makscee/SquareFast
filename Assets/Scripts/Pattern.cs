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
    public List<PatternImages> Patterns;
}

public class Pattern : MonoBehaviour
{
    public static Pattern Instance;
    public List<PatternsImages> Images;
    private int _curLevel = 0;
    private int _patterns = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void NextLevel(int skip = 1)
    {
        if (Level.GameOver)
        {
            return;
        }
        _curLevel += skip;
        var c = 1;
        foreach (var images in Images[_patterns].Patterns)
        {
            if (c > _curLevel) break;
            c++;
            foreach (var image in images.Images)
            {
                if (!image.gameObject.activeSelf)
                {
                    var from = _patterns == 1 ? new Vector3(0, 1, 1) : Vector3.zero;
                    image.rectTransform.localScale = from;
                    Utils.Animate(from, Vector3.one, Level.TickTime / 2, (v) => image.rectTransform.localScale += v);
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
        _curLevel = 0;
        foreach (var patterns in Images)
        {
            foreach (var images in patterns.Patterns)
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
        if (Level.Ticks % 3 != 0)
        {
            return;
        }
        var c = 1;
        foreach (var images in Images[_patterns].Patterns)
        {
            if (c > _curLevel) break;
            c++;
            foreach (var image in images.Images)
            {
                var t = 1.15f + c * 0.05f;
                var from = new Vector3(t, t, t);
                var to = Vector3.one;
                var tImage = image;
                tImage.rectTransform.localScale = from;
                Utils.Animate(from, to, Level.TickTime * 2, (v) => tImage.rectTransform.localScale += v);
            }
        }
    }
}