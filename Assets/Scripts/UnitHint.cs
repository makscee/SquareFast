using UnityEngine;
using UnityEngine.UI;

public class UnitHint : MonoBehaviour
{
    public Text Text;
    public UnitedTint Tint;
    private Unit _unit;
    
    public static readonly Prefab Prefab = new Prefab("UnitHint");
    
    private void Update()
    {
        if (_unit == null) return;
        Text.rectTransform.position = new Vector3(_unit.transform.position.x, Text.rectTransform.position.y, 0);
    }

    public static UnitHint CreateUnitText(string text, Unit unit, float smoothIn = 0.5f, float smoothOut = 0.4f, float scale = 0.6f, Color? colorOverride = null)
    {
        var go = Prefab.Instantiate();
        go.transform.SetParent(CameraScript.Instance.UnitHintCanvas.transform);
        go.transform.localScale = new Vector3(scale, scale);
        var uh = go.GetComponent<UnitHint>();
        uh._unit = unit;
        uh.Tint = go.GetComponent<UnitedTint>();
        go.GetComponent<Text>().text = text;
        Color c;
        if (colorOverride != null)
        {
            uh.Tint.OverrideColor = colorOverride.Value;
            c = uh.Tint.OverrideColor;
            c.a = 0;
            uh.Tint.OverrideColor = c;
            Utils.Animate(0f, 1f, smoothIn, (f) =>
            {
                c.a += f;
                uh.Tint.OverrideColor = c;
            });
        }
        else
        {
            c = uh.Tint.Color;
            c.a = 0;
            uh.Tint.Color = c;
            Utils.Animate(0f, 1f, smoothIn, (f) =>
            {
                c.a += f;
                uh.Tint.Color = c;
            });
        }
        uh._unit.DieEvent += () =>
        {
            Utils.Animate(1f, 0f, smoothOut, f =>
            {
                c.a += f;
                if (colorOverride != null)
                {
                    uh.Tint.OverrideColor= c;
                }
                else
                {
                    uh.Tint.Color = c;
                }
            });
            Utils.InvokeDelayed(() =>
            {
                Destroy(go);
            }, smoothOut);
        };
        return uh;
    }
}