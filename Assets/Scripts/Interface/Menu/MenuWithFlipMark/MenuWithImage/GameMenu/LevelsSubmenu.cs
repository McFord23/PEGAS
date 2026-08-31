using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelsSubmenu : MonoBehaviour
{
    [Serializable]
    private class LevelDTO
    {
        public SceneAsset scene;
        public Sprite preview;
    }
    
    [SerializeField] private Image levelPreview;
    [SerializeField] private LevelInfo levelInfo;
    [SerializeField] private LevelButton[] levelButtons;
    [SerializeField] private LevelDTO[] levels;
    private Level selectedLevel;
    private bool hasSelectedLevel;
    
    private GameMenu gameMenu;

    public void Initialize(GameMenu menu)
    {
        gameMenu = menu;

        foreach (var levelButton in levelButtons)
        {
            levelButton.Initialize(this);
        }
    }
    
    public void Select(Level level)
    {
        selectedLevel = level;
        hasSelectedLevel = true;
        
        levelPreview.sprite = levels[(int)level - 2].preview;
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
            SceneManagerAdapter.Instance.LoadScene(selectedLevel);
        }
    }
}