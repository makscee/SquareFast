using UnityEngine;
using UnityEngine.UI;

public class AvengeText : MonoBehaviour
{
    public Text Text;
    public UnitedTint Tint;
    private Unit _killer;
    private void Start()
    {
        Level.Instance.GameOverAction += () =>
        {
            if (Level.Instance.Killer == null) return;
            _killer = Level.Instance.KillerUnit;
            var c = Tint.Color;
            c.a = 0;
            Tint.Color = c;
            Utils.Animate(0f, 1f, 0.5f, (f) =>
            {
                c.a += f;
                Tint.Color = c;
            });
            _killer.DieEvent += () => Utils.Animate(1f, 0f, 0.4f, f =>
            {
                c.a += f;
                Tint.Color = c;
            });
        };
    }

    private void Update()
    {
        if (_killer == null) return;
        Text.rectTransform.position = new Vector3(_killer.transform.position.x, Text.rectTransform.position.y, 0);
    }
}