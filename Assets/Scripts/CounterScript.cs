using System;
using UnityEngine;
using UnityEngine.UI;

public class CounterScript : MonoBehaviour
{
    public Text Text;
    private Vector3 _scale;
    public static CounterScript Instance;

    private void Start()
    {
        Instance = this;
        _scale = transform.localScale;
        Text = GetComponent<Text>();
        Level.Instance.StartAction += () =>
        {
            Updating = true;
            _t = 0;
        };
        Level.Instance.GameOverStartAction += () =>
        {
            Updating = false;
        };
        Menu.Instance.Enter += () =>
        {
            _t = 0;
            Text.text = "0.00";
        };
        Level.Instance.TickActionPerm += () =>
        {
            if (Level.Ticks % 3 != 0 || Level.GameOver) return;
            Utils.Animate(_scale + new Vector3(0.3f, 0.3f), _scale, 0.15f, (v) =>
            {
                Text.transform.localScale = v;
            }, null, true);
        };
    }

    private float _t = 0;
    public bool Updating;
    
    private void Update()
    {
        if (!Updating)
        {
            return;
        }
        _t += Time.deltaTime;
        Text.text = string.Format("{0:F2}", Math.Round(_t, 2));
    }
}
