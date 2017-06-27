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
            SceneManager.LoadScene(0);
            Level.Updating = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CounterScript.Instance.IncreaseCounter();
            SceneManager.LoadScene(0);
            Level.Updating = true;
            Level.TickTime /= 1.5f;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            CounterScript.Instance.IncreaseCounter();
        }
        
        if (Player.Instance == null) return;
        var dir = Player.Instance.transform.position - transform.position;
        dir.z = 0;
        transform.position += dir * _followSpeed;
    }
}