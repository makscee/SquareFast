using UnityEngine;
using UnityEngine.UI;

public class UnitedTint : MonoBehaviour
{
    public static Color Tint = Color.white;
    public static Color Multiplier = Color.white;

    private SpriteRenderer _spriteRenderer;
    private Text _text;
    private RawImage _image;
    [SerializeField]
    private Color _color;
    private bool _initedColor;
    public Color OverrideColor = Color.black;

    public Color Color
    {
        get { return _color; }
        set
        {
            _initedColor = true;
            _color = value;
        }
    }

    public static void Pulse()
    {
        Multiplier = new Color(1.5f, 1.5f, 1.5f);
        Utils.Animate(Multiplier, Color.white, Level.TickTime * 1.5f, (v) =>
        {
            Multiplier = v;
        }, null, true);
    }
    
    public static void TickUpdate()
    {
        if (Level.Ticks % 3 != 0)
        {
            return;
        }
        Pulse();
    }

    private void Awake()
    {
        if (_initedColor) return;
        if (_spriteRenderer = GetComponent<SpriteRenderer>())
        {
            Color = _spriteRenderer.color;
            _spriteRenderer.color = Color * Tint * Multiplier;
        } else if (_text = GetComponent<Text>())
        {
            Color = _text.color;
            _text.color = Color * Tint * Multiplier;
        } else if (_image = GetComponent<RawImage>())
        {
            Color = _image.color;
            _image.color = Color * Tint * Multiplier;
        }
        _initedColor = true;
    }

    private void Update()
    {
        var c = OverrideColor == Color.black ? Color * Tint * Multiplier : OverrideColor;
        if (_spriteRenderer)
        {
            _spriteRenderer.color = c;
        } else if (_text)
        {
            _text.color = c;
        } else if (_image)
        {
            _image.color = c;
        }
    }
}