using UnityEngine;

public class GameMenu : MenuBaseWithImage
{
    [SerializeField] private GameObject levelsSubmenu;
    [SerializeField] private GameObject missionSubmenu;
    [SerializeField] private GameObject pauseSubmenu;
    [SerializeField] private GameObject loseSubmenu;
    [SerializeField] private GameObject victorySubmenu;
    private GameObject currentSubmenu;

    public override void Initialize(MenuManager manager, MenuBackground background)
    {
        base.Initialize(manager, background);

        if (SceneManagerAdapter.IsGameScene())
        {
            FlipImage(true);
            ChangePage(MenuBackground.ImagePageType.Arch);
            
            currentSubmenu = pauseSubmenu;
            pauseSubmenu.SetActive(true);
            missionSubmenu.SetActive(true);
        }
        else
        {
            FlipImage(false);
            ChangePage(MenuBackground.ImagePageType.Empty);
            
            currentSubmenu = levelsSubmenu;
            levelsSubmenu.GetComponent<LevelsSubmenu>().Initialize(this);
            levelsSubmenu.SetActive(true);
        }
    }
    
    private void Update()
    {
        if (Global.IsPause && pauseSubmenu.activeSelf && Controls.Pause)
        {
            ExecuteResume();
        }
    }

    public void SetInfoPageVisible(bool value)
    {
        var newPage = value ? MenuBackground.ImagePageType.Info : MenuBackground.ImagePageType.Empty;
        ChangePage(newPage);
        image.SetActive(!value);
    }
    
    public void Pause()
    {
        ChangeSubmenu(pauseSubmenu);
        TryEnableMenu();
    }

    public void ExecuteResume()
    {
        EventAdapter.Instance.Execute(EventKey.Resume);
    }

    public void Resume()
    {
        Manager.Disable();
    }
    
    public void Lose()
    {
        ChangeSubmenu(loseSubmenu);
        TryEnableMenu();
    }

    public void ExecuteRetry()
    {
        EventAdapter.Instance.Execute(EventKey.Retry);
    }

    public void Retry()
    {
        if (Manager.gameObject.activeSelf)
        {
            Manager.Disable();
        }
    }
    
    public void Victory()
    {
        ChangeSubmenu(victorySubmenu);
        TryEnableMenu();
    }

    public void Continue()
    {
        SceneManagerAdapter.Instance.LoadScene(Level.Credits);
    }
    
    public void Exit()
    {
        SceneManagerAdapter.Instance.LoadScene(Level.MainMenu);
    }

    private void TryEnableMenu()
    {
        if (!Manager.gameObject.activeSelf)
        {
            Manager.Enable();
        }
    }
    
    private void ChangeSubmenu(GameObject newSubmenu)
    {
        currentSubmenu.SetActive(false);
        newSubmenu.SetActive(true);
        currentSubmenu = newSubmenu;
    }
}