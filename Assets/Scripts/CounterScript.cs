using System;
using UnityEngine;
using UnityEngine.UI;

public class CounterScript : MonoBehaviour
{
    private Text _text;

    private void Start()
    {
        _text = GetComponent<Text>();
        Level.Instance.StartAction += () =>
        {
            _updating = true;
            _t = 0;
        };
        Level.Instance.GameOverStartAction += () =>
        {
            _updating = false;
        };
        Menu.Instance.Enter += () =>
        {
            _t = 0;
            _text.text = "0.00";
        };
        Level.Instance.TickActionPerm += () =>
        {
            if (Level.Ticks % 3 != 0 || Level.GameOver) return;
            Utils.Animate(new Vector3(1.3f, 1.3f), Vector3.one, 0.15f, (v) =>
            {
                _text.transform.localScale = v;
            }, null, true);
        };
    }

    private float _t = 0;
    private bool _updating;
    
    private void Update()
    {
        if (!_updating)
        {
            return;
        }
        _t += Time.deltaTime;
        _text.text = string.Format("{0:F2}", Math.Round(_t, 2));
    }
}
