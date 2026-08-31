using UnityEngine;

public class LocalizationBase : MonoBehaviour
{
    [SerializeField] protected string file;
    [SerializeField] protected string phrase;
    private bool isStarted;

    protected bool IsPathEmpty => string.IsNullOrEmpty(file) || string.IsNullOrEmpty(phrase);
    
    protected virtual void Start()
    {
        isStarted = true;

        if (IsPathEmpty) return;
        
        LocalizationManager.Instance.LanguageChangeEvent.AddListener(Localize);
        Localize();
    }
    
    //protected void OnDestroy()
    //{
    //    LocalizationManager.LanguageChange -= Localize;
    //}
    
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
        if (!isStarted) Start();
    }
}
