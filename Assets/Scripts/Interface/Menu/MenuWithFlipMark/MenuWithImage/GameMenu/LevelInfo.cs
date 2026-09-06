using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    [SerializeField] private LocalizationBase title;
    [SerializeField] private LocalizationBase description;
    
    public void Show(Level level)
    {
        var file = $"{level.ToString()}/Info";
        title.UpdatePhrase(file, "title");
        description.UpdatePhrase(file, "description");
        description.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        description.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}