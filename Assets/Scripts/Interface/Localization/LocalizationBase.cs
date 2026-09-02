using UnityEngine;

public class LocalizationBase : MonoBehaviour
{
    [SerializeField] protected string file;
    [SerializeField] protected string phrase;
    private bool wasStarted;

    protected bool IsPathEmpty => string.IsNullOrEmpty(file) || string.IsNullOrEmpty(phrase);
    
    protected virtual void Start()
    {
        wasStarted = true;

        if (IsPathEmpty) return;
        
        LocalizationManager.Instance.LanguageChangeEvent.AddListener(Localize);
        Localize();
    }
    
    public void UpdatePhrase(string newFile, string newPhrase, bool apply = true)
    {
        if (IsPathEmpty)
        {
            LocalizationManager.Instance.LanguageChangeEvent.AddListener(Localize);
        }
        
        file = newFile;
        phrase = newPhrase;
        
        if (apply) Localize();
    }

    protected virtual void Localize()
    {
        if (!wasStarted) Start();
    }
}
