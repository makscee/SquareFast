using System;
using UnityEngine;
using UnityEngine.UI;

public class CounterScript : MonoBehaviour
{
    private Text _text;

    private void Start()
    {
        _text = GetComponent<Text>();
        Level.Instance.StartAction += () => _started = true;
    }

    private float _t = 0;
    private bool _started;
    private void Update()
    {
        if (!_started || Level.GameOver)
        {
            _started = false;
            return;
        }
        _t += Time.deltaTime;
        _text.text = string.Format("{0:F2}", Math.Round(_t, 2));
    }
}
