public class Grid
{
    readonly Unit[] _units = new Unit[100];
    private readonly int _gridOffset = 50;

    public void SetOrReplace(int pos, Unit unit)
    {
        var prevPos = unit.Position + _gridOffset;
        pos += _gridOffset;
        if (_units[pos] != null && _units[pos] != unit)
        {
            _units[pos].Kill();
        }
        _units[prevPos] = null;
        _units[pos] = unit;
        unit.Move(pos - _gridOffset);
    }

    public Unit Get(int pos)
    {
        pos += _gridOffset;
        return _units[pos];
    }
}
