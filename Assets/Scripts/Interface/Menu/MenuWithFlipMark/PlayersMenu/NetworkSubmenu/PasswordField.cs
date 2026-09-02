using UnityEngine;
using TMPro;

public class PasswordField : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private LocalizationBase placeholder;

    private void Start()
    {
        inputField.onEndEdit.Invoke("");
    }

    public void Block(bool value)
    {
        inputField.interactable = !value;
    }
    
    public void SetVisiblePassword(bool value)
    {
        inputField.contentType = value ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        inputField.ForceLabelUpdate();
    }

    public void ButtonHintPlaceholder()
    {
        placeholder.UpdatePhrase("Interface", "passwordButtonHint");
    }

    public void FieldHintPlaceholder()
    {
        placeholder.UpdatePhrase("Interface", "passwordFieldHint");
    }
}