using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CameraScript : MonoBehaviour
{
    public static CameraScript Instance;
    private const float FollowSpeed = 0.05f;
    public Text SavedTicks;
    public Material Inverse;
    public Material ColorSwitch;
    public float InvProgress, SwitchProgress;
    public Color SwitchColor;
    public GameObject SwitchFollow;
    public List<Color> SwitchColors;

    private void Awake()
    {
        Instance = this;
        Inverse = new Material(Inverse);
        ColorSwitch = new Material(ColorSwitch);
    }

    private void Update()
    {
        if (SwitchFollow != null)
        {
            var pos = SwitchFollow.transform.position;
            pos += pos.x > 0 ? Vector3.right : Vector3.left;
            var v = Camera.main.WorldToScreenPoint(pos).x;
            var w = (float) Camera.main.pixelWidth / 2;
            SwitchProgress = 1 - Math.Abs(v - w) / w;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Level.Instance.Restart();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavedTicks.text += "\n" + Level.Ticks;
        }
        if (Player.Instance == null) return;
        var dir = Player.Instance.transform.position - transform.position;
        dir.z = 0;
        transform.position += dir * FollowSpeed;
    }

    private void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        RenderTexture rt = new CustomRenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight);
        ColorSwitch.SetFloat("_Progress", SwitchProgress);
        ColorSwitch.SetFloat("_InvProgress", InvProgress);
        ColorSwitch.SetColor("_Color", SwitchColor);
        Graphics.Blit (source, rt, ColorSwitch);
        if (InvProgress != 0)
        {
            RenderTexture rt2 = new CustomRenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight);
            Inverse.SetFloat("_Progress", InvProgress);
            Inverse.SetColor("_BG", Camera.main.backgroundColor);
            Graphics.Blit (rt, rt2, Inverse);
            rt.DiscardContents();
            rt = rt2;
        }
        Graphics.Blit(rt, destination);
    }
}