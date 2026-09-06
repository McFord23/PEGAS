using UnityEngine;

public class MenuBaseWithFlipMark : MenuBase
{
    [SerializeField] private BookMark mark;

    public override void Initialize(MenuManager manager, MenuBackground background)
    {
        mark.Initialize();
        base.Initialize(manager, background);
    }
    
    public override void SetActive(bool value)
    {
        base.SetActive(value);
        mark.SetActive(value);
    }
    
    public override void UpdateMark(bool value)
    {
        mark.Flip(value);
    }
}