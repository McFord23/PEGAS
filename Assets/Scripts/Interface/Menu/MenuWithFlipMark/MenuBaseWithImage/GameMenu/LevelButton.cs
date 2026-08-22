using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Level level;
    private LevelsSubmenu levelsSubmenu;
    private bool isSelected;

    public void Initialize(LevelsSubmenu submenu)
    {
        levelsSubmenu = submenu;
        GetComponent<Button>().onClick.AddListener(Press);
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        Select();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Select();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Deselect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Deselect();
    }
    
    private void Press()
    {
        levelsSubmenu.Play();
    }

    private void Select()
    {
        if (isSelected) return;

        isSelected = true;
        levelsSubmenu.Select(level);
    }

    private void Deselect()
    {
        if (!isSelected) return;

        isSelected = false;
        levelsSubmenu.Deselect(/*level*/);
    }
}