using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PasswordButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private PasswordField passwordField;
    [SerializeField] private Sprite passwordSelectIcon;
    [SerializeField] private Sprite passwordHideIcon;
    [SerializeField] private Sprite passwordShowIcon;
    private Image icon;
    private bool isVisible;
    
    private void Start()
    {
        icon = GetComponent<Image>();
    }

    public void ChangePasswordVisibility()
    {
        isVisible = !isVisible;
        passwordField.SetVisiblePassword(isVisible);
        icon.sprite = isVisible ? passwordShowIcon : passwordHideIcon;
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        passwordField.ButtonHintPlaceholder();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        passwordField.FieldHintPlaceholder();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        icon.sprite = passwordSelectIcon;
        passwordField.ButtonHintPlaceholder();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        icon.sprite = isVisible ? passwordShowIcon : passwordHideIcon;
        passwordField.FieldHintPlaceholder();
    }
}