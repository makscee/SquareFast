using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct PatternImages
{
    public List<RawImage> Images;
}

public class Pattern : MonoBehaviour
{
    public static Pattern Instance;
    public List<PatternImages> Images;
    private int _curLevel = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void NextLevel()
    {
        _curLevel++;
        var c = 1;
        foreach (var images in Images)
        {
            if (c > _curLevel) break;
            c++;
            foreach (var image in images.Images)
            {
                if (!image.gameObject.activeSelf)
                {
                    image.rectTransform.localScale = Vector3.zero;
                    Utils.Animate(Vector3.zero, Vector3.one, Level.TickTime / 2, (v) => image.rectTransform.localScale += v);
                }
                image.gameObject.SetActive(true);
            }
        }
    }

    public void Reset()
    {
        _curLevel = 0;
        foreach (var images in Images)
        {
            foreach (var image in images.Images)
            {
                image.gameObject.SetActive(false);
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
        foreach (var images in Images)
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