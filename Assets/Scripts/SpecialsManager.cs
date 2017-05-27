using UnityEngine;

public class SpecialsManager
{
    private bool _started;
    private readonly SpecialsTree _tree = new SpecialsTree();
    
    public void Start()
    {
        _started = true;
        CameraScript.Instance.SetSAColor();
    }

    public void Reset()
    {
        _tree.Reset();
        _started = false;
        CameraScript.Instance.SetRegularColor();
    }

    public bool IsStarted()
    {
        return _started;
    }

    public ISpecialAbility Add(int dir)
    {
        if (!_started)
        {
            return null;
        }
        ISpecialAbility ability = null;
        switch (dir)
        {
            case 1:
                ability = _tree.GoRight();
                break;
            case -1:
                ability = _tree.GoLeft();
                break;
        }
        return ability;
    }
}

internal class SpecialsTree
{
    private readonly SpecialsNode _root;
    private SpecialsNode _cur;
    
    public SpecialsTree()
    {
        _root = new SpecialsNode();
        _root.AddLeft().AddLeft().AddLeft(new DashL());
        _root.AddRight().AddRight().AddRight(new DashR());
        _cur = _root;
    }

    public void Reset()
    {
        _cur = _root;
    }

    public ISpecialAbility GoLeft()
    {
        if (_cur.Left == null)
        {
            Debug.Log("reached null");
            return null;
        }
        _cur = _cur.Left;
        return _cur.Ability;
    }

    public ISpecialAbility GoRight()
    {
        if (_cur.Right == null)
        {
            Debug.Log("reached null");
            return null;
        }
        _cur = _cur.Right;
        return _cur.Ability;
    }
    
    private class SpecialsNode
    {
        public SpecialsNode Left, Right;
        public ISpecialAbility Ability;

        public SpecialsNode AddLeft(ISpecialAbility ability = null)
        {
            Left = new SpecialsNode();
            Left.Ability = ability;
            return Left;
        }
        
        public SpecialsNode AddRight(ISpecialAbility ability = null)
        {
            Right = new SpecialsNode();
            Right.Ability = ability;
            return Right;
        }
    }
}