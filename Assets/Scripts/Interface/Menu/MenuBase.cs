using UnityEngine;
using UnityEngine.UI;

public class MenuBase : MonoBehaviour
{
    protected MenuManager Manager { get; private set; }
    protected MenuBackground Background { get; private set; }
    protected Image Page { get; private set;  }

    public virtual void Initialize(MenuManager manager, MenuBackground background)
    {
        Page = GetComponent<Image>();
        Manager = manager;
        Background = background;
    }

    public virtual void SetActive(bool value)
    {
        if (value)
        {
            Background.OnChangeMenu(this);
            Manager.UpdateCurrentMenu(this);
        }
        
        gameObject.SetActive(value);
        UpdateMark(value);
    }
    
    public virtual void UpdateMark(bool value) {}

    public bool IsMonoPage()
    {
        return Page.sprite.name == "Title";
    }
}