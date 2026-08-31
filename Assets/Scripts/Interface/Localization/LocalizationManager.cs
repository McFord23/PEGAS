using UnityEngine.Events;

public class LocalizationManager : SingletonMonoBehaviour<LocalizationManager>
{
    public UnityEvent LanguageChangeEvent;
    public static Language CurrentLanguage { get; private set; } = Language.English;

    public enum Language
    {
        English,
        Russian
    }

    public string GetPhrase(string file, string phrase)
    {
        var path = $"Languages/{CurrentLanguage}/{file}";
        var jsonData = JsonReader<string, string>.LoadFile(path);
        return jsonData[phrase];
    }

    public void SetLanguage(Language newLanguage)
    {
        CurrentLanguage = newLanguage;
        LanguageChangeEvent?.Invoke();
    }
}
