using System;
using UnityEngine;

//[ExecuteInEditMode]
public class InversePostFX : MonoBehaviour
{
    public Material Inverse;

    private void Awake()
    {
        Inverse = new Material(Inverse);
    }

    private void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        var invProgress = CameraScript.Instance.InvProgress;
        
        if (invProgress > 0.001)
        {
            Inverse.SetFloat("_Progress", invProgress);
            Inverse.SetColor("_BG", Camera.main.backgroundColor);
            Graphics.Blit (source, destination, Inverse);
            return;
        }
        Graphics.Blit(source, destination);
    }
}
