using UnityEngine;

public class LanguageButton : MonoBehaviour
{
    [SerializeField] private LocalizationManager.Language language;
    [SerializeField] private GameObject outline;

    private void Start()
    {
        OnLanguageChange();
        LocalizationManager.OnChangeLanguageEvent += OnLanguageChange;
    }

    private void OnLanguageChange()
    {
        outline.SetActive(language == LocalizationManager.CurrentLanguage);
    }
    
    public void ChangeLanguage()
    {
        LocalizationManager.SetLanguage(language);
    }
}