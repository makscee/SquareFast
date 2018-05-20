using System;
using UnityEngine;
using UnityEngine.UI;

public class CounterScript : MonoBehaviour
{
    private Text _text;
    private Vector3 _scale;

    private void Start()
    {
        _scale = transform.localScale;
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
            Utils.Animate(_scale + new Vector3(0.3f, 0.3f), _scale, 0.15f, (v) =>
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
