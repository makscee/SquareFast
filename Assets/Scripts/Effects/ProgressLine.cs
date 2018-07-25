using UnityEngine;

public class ProgressLine : MonoBehaviour
{
    public GameObject FrontLine, BackLine;
    private UnitedTint _frontTint, _backTint;
    public static ProgressLine Instance;
 
    private void Awake()
    {
        Instance = this;
        _frontTint = FrontLine.GetComponent<UnitedTint>();
        _backTint = BackLine.GetComponent<UnitedTint>();
    }

    private float _curTint = 0.4f, _stepTint = 0.15f;
    public void StartNext()
    {
        Updating = true;
        FrontLine.transform.localScale = new Vector3(0f, FrontLine.transform.localScale.y);
        if (BackLine.activeSelf)
        {
            var tint = _curTint - 0.15f;
            _backTint.Color = new Color(tint, tint, tint);
            BackLine.transform.localScale = new Vector3(LevelSpawner.Distance * 2 + 1, BackLine.transform.localScale.y);
        }
        else
        {
            BackLine.SetActive(true);
            FrontLine.SetActive(true);
        }
        _curTint += _stepTint;
        _frontTint.Color = new Color(_curTint, _curTint, _curTint);
        _t = 0f;
    }

    public bool Updating;
    private float _t;
    private void Update()
    {
        if (!Updating)
        {
            return;
        }
        _t += Time.deltaTime;
        var st = Utils.Interpolate(0f, LevelSpawner.Distance * 2 + 1,
            LevelSpawner.SublevelTime, _t);
        FrontLine.transform.localScale += new Vector3(st, 0);
    }

    public void Reset()
    {
        FrontLine.SetActive(false);
        BackLine.SetActive(false);
        BackLine.transform.localScale = new Vector3(0f, BackLine.transform.localScale.y);
        _curTint = 1f - _stepTint * 4;
    }
}