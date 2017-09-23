using System;
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
        if (Player.Instance == null) return;
        var needPos = Player.Instance.transform.position;
        if (Menu.Instance.isActiveAndEnabled)
        {
            needPos += Vector3.up * 4;
        }
        var dir =  needPos - transform.position;
        dir.z = 0;
        transform.position += dir * FollowSpeed;
    }

    private const float SwitchTime = 0.2f;
    public void SwitchScene(Action a = null)
    {
        Utils.Animate(0, 1, SwitchTime, (v) => SwitchProgress = v, null, true);
        Utils.InvokeDelayed(() =>
        {
            InvProgress = 0f;
            Utils.Animate(1, 0, SwitchTime, (v) => SwitchProgress = v, null, true);
            transform.position = new Vector3(0, 0, transform.position.z);
            if (a != null) a();
        }, SwitchTime);
    }

    public static void ChangeColorTinted(Color c)
    {
        Camera.main.backgroundColor = c * new Color(0.21f, 0.21f, 0.21f);
    }

    private void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        ColorSwitch.SetFloat("_Progress", SwitchProgress);
        ColorSwitch.SetFloat("_InvProgress", InvProgress);
        ColorSwitch.SetColor("_Color", SwitchColor);
        Graphics.Blit (source, destination, ColorSwitch);
    }
}