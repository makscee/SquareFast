using UnityEngine;
using UnityEngine.UI;

public class CounterScript : MonoBehaviour
{
    public static CounterScript Instance;
    private Text _text;
    private static string text = "1 - 1";

    private void Awake()
    {
        Instance = this;
        _text = GetComponent<Text>();
        _text.text = text;
    }

    public void Set(int x, int y)
    {
        _text.text = x + " - " + y;
        text = _text.text;
    }
}
