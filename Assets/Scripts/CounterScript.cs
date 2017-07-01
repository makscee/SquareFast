using UnityEngine;
using UnityEngine.UI;

public class CounterScript : MonoBehaviour
{
    public static CounterScript Instance;
    private Text _text;
    private static int _count = 1;

    private void Awake()
    {
        Instance = this;
        _text = GetComponent<Text>();
        _text.text = _count.ToString();
    }

    public void IncreaseCounter()
    {
        _text.text = (++_count).ToString();
    }
}
