using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlyEffect : MonoBehaviour
{
    public GameObject Image, Text;
    private static Prefab _prefab = new Prefab("ScreenFlyEffect"); 
    
    public static void Create(string text)
    {
        var sf = _prefab.Instantiate().GetComponent<ScreenFlyEffect>();
        sf.transform.SetParent(CameraScript.Instance.ScreenCanvas.transform);
        sf.GetComponent<RectTransform>().localPosition = Vector3.zero;
        sf.Text.GetComponent<Text>().text = text;
        var textRt = sf.Text.GetComponent<RectTransform>();
        const float p1 = 0.3f, p2 = 1f, p3 = 0.3f;
        Utils.Animate(0f, 1f, 0.1f, (v) => sf.Image.transform.localScale = new Vector3(1f, v, 1f), null, true);
        Utils.Animate(4000f, 50f, p1, (v) => textRt.localPosition = new Vector3(v, 0f, 0f), null, true);
        Utils.Animate(50f, 0f, p2, (v) => textRt.localPosition = new Vector3(v, 0f, 0f), null, true, p1);
        Utils.Animate(0f, -4000f, p3, (v) => textRt.localPosition = new Vector3(v, 0f, 0f), null, true, p1 + p2);
        Utils.Animate(1f, 0f, 0.1f, (v) => sf.Image.transform.localScale = new Vector3(1f, v, 1f), null, true, p1 + p2 + p3 + 0.2f);
        Destroy(sf.gameObject, 3f);
    }
}