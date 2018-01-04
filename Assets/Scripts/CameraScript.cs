using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CameraScript : MonoBehaviour
{
    public static CameraScript Instance;
    private const float FollowSpeed = 3f;
    public Text SavedTicks;
    public Material Inverse;
    public Material ColorSwitch;
    public float InvProgress, SwitchProgress;
    public Color SwitchColor;
    public GameObject SwitchFollow;
    private float _zoom;
    public static float MenuZoomout = 3f;

    private void Awake()
    {
        _zoom = Camera.main.orthographicSize;
        Instance = this;
        ColorSwitch = new Material(ColorSwitch);
    }

    private void Update()
    {
        if (SwitchFollow != null)
        {
            var pos = SwitchFollow.transform.position;
            if (Math.Abs(pos.x) < Math.Abs(Player.Instance.Position.x))
            {
                pos = Player.Instance.Position;
            }
            pos += pos.x > 0 ? new Vector3(1.5f, 0) : new Vector3(-1.5f, 0);
            pos.x = Math.Max(2f, Math.Abs(pos.x));
            var v = Camera.main.WorldToScreenPoint(pos).x;
            var w = (float) Camera.main.pixelWidth / 2;
            var needSP = (1 - Math.Abs(v - w) / w) - SwitchProgress;
            SwitchProgress += needSP / 2;
        }
        if (Player.Instance == null) return;
        var needPos = Player.Instance.transform.position;
        if (MenuZoomout < 0) MenuZoomout = 0;
        if (Menu.Instance.isActiveAndEnabled)
        {
            needPos += Vector3.up * (3.5f - 3.5f / 3f * MenuZoomout);
            Camera.main.orthographicSize = _zoom / (1 + MenuZoomout / 2);
        }
        var dir =  needPos - transform.position;
        dir.z = 0;
        transform.position += dir * FollowSpeed * Time.deltaTime;
    }

    private const float SwitchTime = 0.2f;
    public void SwitchScene(Action a = null)
    {
        Menu.Instance.PlaySelect();
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