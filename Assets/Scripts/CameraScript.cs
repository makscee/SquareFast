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
        ColorSwitch = new Material(ColorSwitch);
    }

    private void Update()
    {
        if (SwitchFollow != null)
        {
            var pos = SwitchFollow.transform.position;
            pos += pos.x > 0 ? new Vector3(1.5f, 0) : new Vector3(-1.5f, 0);
            pos.x = Math.Max(2f, Math.Abs(pos.x));
            var v = Camera.main.WorldToScreenPoint(pos).x;
            var w = (float) Camera.main.pixelWidth / 2;
            var needSP = (1 - Math.Abs(v - w) / w) - SwitchProgress;
            SwitchProgress += needSP / 2;
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
        ColorSwitch.SetFloat("_Progress", SwitchProgress);
        ColorSwitch.SetFloat("_InvProgress", InvProgress);
        ColorSwitch.SetColor("_Color", SwitchColor);
        Graphics.Blit (source, destination, ColorSwitch);
    }
}