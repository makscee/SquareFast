using UnityEngine;
using UnityEngine.UI;

public class CounterScript : MonoBehaviour
{
    public static CounterScript Instance;
    private Text text;
    private static int count = 1;

    private void Awake()
    {
        Instance = this;
        text = GetComponent<Text>();
        text.text = count.ToString();
    }

    public void IncreaseCounter()
    {
        text.text = (++count).ToString();
    }
}
