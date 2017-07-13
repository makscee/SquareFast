using UnityEngine;
using UnityEngine.UI;

public class CounterScript : MonoBehaviour
{
    public static CounterScript Instance;
    private Text _text;
    private static string text = "0";

    private void Awake()
    {
        Instance = this;
        _text = GetComponent<Text>();
        _text.text = text;
    }

    public void Set(int x)
    {
        _text.text = x.ToString();
        text = _text.text;
    }
}
