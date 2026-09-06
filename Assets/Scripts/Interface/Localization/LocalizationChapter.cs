using UnityEngine;

public class LocalizationChapter : LocalizationText
{
    [SerializeField] private string number;
    [SerializeField] private Level level;
    
    protected override string GetPhrase()
    {
        var chapter = base.GetPhrase();
        var indent = number.Contains(".") ? "   " : "";
        var chapterName = LocalizationManager.GetPhrase($"{level.ToString()}/Info", "title");
        return $"{indent}{chapter} {number}. {chapterName}";
    }
}