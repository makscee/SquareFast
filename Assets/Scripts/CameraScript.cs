using System.ComponentModel;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CameraScript : MonoBehaviour
{
    public static CameraScript Instance;
    private float _followSpeed = 0.05f;
    public Text SavedTicks;
    public Material Material;
    public float Progress;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
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
        transform.position += dir * _followSpeed;
    }

    private void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        if (Progress == 0)
        {
            Graphics.Blit (source, destination);
            return;
        }
 
        Material.SetFloat("_Progress", Progress);
        Graphics.Blit (source, destination, Material);
    }
}