using System.ComponentModel;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraScript : MonoBehaviour
{
    public static CameraScript Instance;
    private float _followSpeed = 0.05f;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Level.Instance.Restart();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Level.Instance.NextLevel();
        }
        
        if (Player.Instance == null) return;
        var dir = Player.Instance.transform.position - transform.position;
        dir.z = 0;
        transform.position += dir * _followSpeed;
    }
}