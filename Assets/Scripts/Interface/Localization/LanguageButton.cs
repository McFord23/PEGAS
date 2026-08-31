using UnityEngine;

public class LanguageButton : MonoBehaviour
{
    [SerializeField] private LocalizationManager.Language language;
    [SerializeField] private GameObject outline;

    private void Start()
    {
        OnLanguageChange();
        LocalizationManager.Instance.LanguageChangeEvent.AddListener(OnLanguageChange);
    }

    //private void OnDestroy()
    //{
    //    LocalizationManager.LanguageChange -= OnLanguageChange;
    //}

    private void OnLanguageChange()
    {
        outline.SetActive(language == LocalizationManager.CurrentLanguage);
    }
    
    public void ChangeLanguage()
    {
        LocalizationManager.Instance.SetLanguage(language);
    }
}