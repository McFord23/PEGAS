using UnityEngine.Events;

public static class LocalizationManager
{
    public enum Language
    {
        English,
        Russian
    }
    
    public static Language CurrentLanguage { get; private set; } = Language.English;
    
    public delegate void ChangeLanguageEvent();
    public static event ChangeLanguageEvent OnChangeLanguageEvent;
    
    public static void ClearSubscribers()
    {
        OnChangeLanguageEvent = null;
    }

    public static string GetPhrase(string file, string phrase)
    {
        var path = $"Languages/{CurrentLanguage}/{file}";
        var jsonData = JsonReader<string, string>.LoadFile(path);
        return jsonData[phrase];
    }

    public static void SetLanguage(Language newLanguage)
    {
        CurrentLanguage = newLanguage;
        OnChangeLanguageEvent?.Invoke();
    }
}
