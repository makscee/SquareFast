using UnityEngine;
using UnityEngine.UI;

public class UnitedTint : MonoBehaviour
{
    public static Color Tint = Color.white;

    private SpriteRenderer _spriteRenderer;
    private Text _text;
    [SerializeField]
    private Color _color;
    private bool _initedColor;

    public Color Color
    {
        get { return _color; }
        set
        {
            _initedColor = true;
            _color = value;
        }
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _text = GetComponent<Text>();
        if (_initedColor) return;
        if (_spriteRenderer)
        {
            Color = _spriteRenderer.color;
        } else if (_text)
        {
            Color = _text.color;
        }
    }

    private void Update()
    {
        if (_spriteRenderer)
        {
            _spriteRenderer.color = Color * Tint;
        } else if (_text)
        {
            _text.color = Color * Tint;
        }
    }
}