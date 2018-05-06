using System;
using NUnit.Framework;
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
    public GameObject SwitchFollow1, SwitchFollow2;
    private float _zoom;
    public static float MenuZoomout = 0f;

    private void Awake()
    {
        _zoom = Camera.main.orthographicSize;
        Instance = this;
        ColorSwitch = new Material(ColorSwitch);
    }

    private float getSwitchProgress(Vector3 pos)
    {
        pos += pos.x > 0 ? new Vector3(1.5f, 0) : new Vector3(-1.5f, 0);
        if (pos.x > -1.5f && pos.x < 1.5f)
        {
            pos.x = Player.Instance.transform.position.x > 0 ? -1.5f : 1.5f;
        }
        var v = Camera.main.WorldToScreenPoint(pos).x;
        var w = (float) Camera.main.pixelWidth / 2;
        return (1 - Math.Abs(v - w) / w);
    }

    private float _still = 0, _before = 0;
    private void Update()
    {
        if (Player.Instance == null) return;
        if (SwitchFollow1 != null || SwitchFollow2 != null)
        {
            float pg1 = 1, pg2 = 1;
            if (SwitchFollow1 != null)
            {
                pg1 = getSwitchProgress(SwitchFollow1.transform.position);
            }
            if (SwitchFollow2 != null)
            {
                pg2 = getSwitchProgress(SwitchFollow2.transform.position);
            }
            SwitchProgress += (Math.Min(pg1, pg2) - SwitchProgress) / 2;
        }
        if (SwitchProgress <= 0)
        {
            _still = 0f;
            _before = 0f;
        }
        else
        {
            if (SwitchProgress == _before)
            {
                _still += Time.deltaTime;
            }
            else
            {
                _before = SwitchProgress;
                _still = 0f;
            }
        }
        if (_still >= 0.5f)
        {
            Debug.LogWarning("Switch progress still more than 0.5 sec. Returning.");
            Utils.Animate(SwitchProgress, 0f, 0.1f, (v) => SwitchProgress = v, null, true);
        }
        var needPos = Player.Instance.transform.position;
        if (MenuZoomout < 0) MenuZoomout = 0;
        if (Menu.Instance.isActiveAndEnabled)
        {
            needPos += Vector3.up * (3.5f - 3.5f / 3f * MenuZoomout);
//            Camera.main.orthographicSize = _zoom / (1 + MenuZoomout / 2);
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