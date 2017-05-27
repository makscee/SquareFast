using System.ComponentModel;
using System.Net.NetworkInformation;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static CameraScript Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetSAColor()
    {
        var cam = GetComponent<Camera>();
        cam.backgroundColor = new Color(0.3f, 0.1f, 0f);
    }

    public void SetRegularColor()
    {
        var cam = GetComponent<Camera>();
        cam.backgroundColor = Color.black;
    }
}