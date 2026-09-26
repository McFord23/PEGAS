using UnityEngine;
using UnityEngine.UI;

public class LevelsSubmenu : MonoBehaviour
{
    [SerializeField] private Image levelPreview;
    [SerializeField] private LevelInfo levelInfo;
    [SerializeField] private LevelButton[] levelButtons;
    [SerializeField] private Sprite[] levelsPreviews;
    [SerializeField] private GameObject requireCoopPopup;
    private Level selectedLevel;
    private bool hasSelectedLevel;
    
    private GameMenu gameMenu;
    private MenuManager menuManager;

    public void Initialize(GameMenu menu, MenuManager manager)
    {
        gameMenu = menu;
        menuManager = manager;

        foreach (var levelButton in levelButtons)
        {
            levelButton.Initialize(this);
        }
    }
    
    public void Select(Level level)
    {
        selectedLevel = level;
        hasSelectedLevel = true;
        
        levelPreview.sprite = levelsPreviews[(int)level - 2];
        levelPreview.gameObject.SetActive(true);
        
        levelInfo.Show(level);
        
        gameMenu.SetInfoPageVisible(true);
    }

    public void Deselect()
    {
        hasSelectedLevel = false;
        levelPreview.gameObject.SetActive(false);
        levelInfo.Hide();
        gameMenu.SetInfoPageVisible(false);
    }
    
    public void Play()
    {
        if (hasSelectedLevel)
        {
            if (LevelManager.IsLevelRequiresCoop(selectedLevel) && Settings.GameMode is GameMode.Single)
            {
                menuManager.ShowPopup(requireCoopPopup);
            }
            else
            {
                LevelManager.Instance.LoadScene(selectedLevel);
            }
        }
    }
}