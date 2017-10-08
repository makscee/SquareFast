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
        Level.Instance.ExitGameOverAction += () =>
        {
            _t = 0;
            _started = true;
        };
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
        var d = Level.TickTime * 3;
        var t = Math.Ceiling(_t / d);
        _t += Time.deltaTime;
        if (t < Math.Ceiling(_t / d))
        {
            Utils.Animate(new Vector3(1.3f, 1.3f), Vector3.one, 0.2f, (v) =>
            {
                _text.transform.localScale = v;
            }, null, true);
        }
        _text.text = string.Format("{0:F2}", Math.Round(_t, 2));
    }
}
