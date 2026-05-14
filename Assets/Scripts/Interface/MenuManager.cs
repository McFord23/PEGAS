using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : SingletonMonoBehaviour<MenuManager>
{
    private RectTransform menuTransform;
    private Vector2 monoPageTarget = new Vector2(-310, 0);
    private Vector2 dualPageTarget = Vector2.zero;
    private float speed = 8f;

    [Header("Menu Background")]
    public Sprite holePage;
    public Sprite holeArchPage;
    private Image background;

    private GameObject menu;
    private GameObject mainMenu;
    
    private GameObject settingsMenu;
    private Image settingsPage;
    private GameObject engraving;
    private GameObject backButton;

    private GameObject deathSubmenu;
    private GameObject pauseSubmenu;
    private GameObject victorySubmenu;

    private GameObject playersMenu;

    private string resumeMenu;
    //private MenuNavigation navigation;

    private RectTransform settingsMarkRect;
    private Toggle settingsMark;
    private Toggle playersMark;

    [Header("Icon")]
    public Sprite soloIcon;
    public Sprite togetherIcon;
    private Image playersIcon;

    public UnityEvent MenuEnabledEvent;
    public UnityEvent MenuDisabledEvent;
    public UnityEvent ChangeMenuEvent;

    private bool isSelectedState = false;

    private Mode mode;

    private enum Mode
    {
        Main,
        Game
    }

    private string scene;

    private void Start()
    {
        base.Awake();
        
        menuTransform = GetComponent<RectTransform>();

        background = transform.Find("Background").GetComponent<Image>();

        settingsMarkRect = transform.Find("Settings Mark").GetComponent<RectTransform>();
        settingsMark = settingsMarkRect.GetComponent<Toggle>();
        playersMark = transform.Find("Players Mark").GetComponent<Toggle>();

        playersIcon = transform.Find("Players Mark/Players Icon").GetComponent<Image>();

        menu = gameObject;
        settingsMenu = transform.Find("Settings Menu").gameObject;
        settingsPage = settingsMenu.GetComponent<Image>();
        engraving = transform.Find("Engraving").gameObject;
        backButton = settingsMenu.transform.Find("Back").gameObject;
        settingsMenu.SetActive(false);
        
        mainMenu = transform.Find("Main Menu").gameObject;

        playersMenu = transform.Find("Players Menu").gameObject;
        playersMenu.SetActive(false);

        playersMenu.GetComponent<PlayersMenu>().Initialize();
        
        //navigation = GetComponent<MenuNavigation>();

        scene = SceneManager.GetActiveScene().name;

        switch (scene)
        {
            case "Main Menu":
                mode = Mode.Main;
                SetBackgroundMask(true);
                
                MainMenu();
                menuTransform.anchoredPosition = monoPageTarget;
                MenuEnabledEvent.Invoke();
                break;
            
            case "Game":
                mode = Mode.Game;
                SetBackgroundMask(false);
                deathSubmenu = settingsMenu.transform.Find("Death Submenu").gameObject;
                pauseSubmenu = settingsMenu.transform.Find("Pause Submenu").gameObject;
                victorySubmenu = settingsMenu.transform.Find("Victory Submenu").gameObject;
                
                backButton.SetActive(false);
                deathSubmenu.SetActive(false);
                pauseSubmenu.SetActive(false);
                victorySubmenu.SetActive(false);
                
                DisableMenu();
                break;
        }
    }

    private void Update()
    {
        if ((bool)mainMenu)
        {
            if (mainMenu.activeSelf) menuTransform.anchoredPosition = Vector2.Lerp(menuTransform.anchoredPosition, monoPageTarget, Time.deltaTime * speed);
            else menuTransform.anchoredPosition = Vector2.Lerp(menuTransform.anchoredPosition, dualPageTarget, Time.deltaTime * speed);

            if (isSelectedState) return;
            if (Input.GetButtonDown("Cancel"))
            {
                Back();
            }
        }
    }

    public void DisableMenu()
    {
        gameObject.SetActive(false);
        MenuDisabledEvent.Invoke();
    }

    public void Back()
    {
        if (playersMenu.activeSelf) playersMenu.SetActive(false);
        
        switch (resumeMenu)
        {
            case "main menu":
                settingsMark.isOn = false;
                DisableSettingsMenu();
                mainMenu.SetActive(true);
                //navigation.StartSelect("Main Menu");
                break;

            case "dead":
                settingsMark.isOn = true;
                EnabledSettingsMenu();
                deathSubmenu.SetActive(true);
                //navigation.StartSelect("Dead");
                break;

            case "pause":
                settingsMark.isOn = true;
                EnabledSettingsMenu();
                pauseSubmenu.SetActive(true);
                //navigation.StartSelect("Pause");
                break;

            case "victory":
                settingsMark.isOn = true;
                EnabledSettingsMenu();
                victorySubmenu.SetActive(true);
                //navigation.StartSelect("Victory");
                break;
        }       

        if (playersMark.isOn) playersMark.isOn = false;
        FlipSettingMark(false);

        ChangeMenuEvent.Invoke();
    }

    public void EnabledSettingsMenu()
    {
        switch (mode)
        {
            case Mode.Main:
                settingsPage.sprite = holePage;
                engraving.SetActive(true);

                if (mainMenu.activeSelf) mainMenu.SetActive(false);
                break;

            case Mode.Game:
                settingsPage.sprite = holeArchPage;
                break;
        }

        if (playersMenu.activeSelf) playersMenu.SetActive(false);
        settingsMenu.SetActive(true);

        if (playersMark.isOn) playersMark.isOn = false;

        settingsMark.isOn = true;
        FlipSettingMark(false);

        //navigation.SetMarkNavigation("Settings");
        ChangeMenuEvent.Invoke();
    }

    private void DisableSettingsMenu()
    {
        if (engraving.activeSelf) engraving.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void EnabledPlayersMenu()
    {
        if (mainMenu.activeSelf) mainMenu.SetActive(false);
        if (settingsMenu.activeSelf) DisableSettingsMenu();
        playersMenu.SetActive(true);

        if (settingsMark.isOn) settingsMark.isOn = false;
        FlipSettingMark(true);

        playersMark.isOn = true;
        //navigation.SetMarkNavigation("Players");
        ChangeMenuEvent.Invoke();
    }

    public void UpdatePlayersIcon(bool togetherMode)
    {
        playersIcon.sprite = togetherMode 
            ? togetherIcon 
            : soloIcon;
    }

    public void MainMenu()
    {
        mainMenu.SetActive(true);

        resumeMenu = "main menu";
        //navigation.StartSelect("Main Menu");
    }

    public void Pause()
    {
        menu.SetActive(true);
        MenuEnabledEvent.Invoke();
        EnabledSettingsMenu();
        pauseSubmenu.SetActive(true);
        resumeMenu = "pause";
        //navigation.StartSelect("Pause");
    }

    public void Dead()
    {
        if (pauseSubmenu.activeSelf)
        {
            pauseSubmenu.SetActive(false);
        }
        else
        {
            menu.SetActive(true);
            EnabledSettingsMenu();
            MenuEnabledEvent.Invoke();
        }
        deathSubmenu.SetActive(true);

        resumeMenu = "dead";
        //navigation.StartSelect("Dead");
    }

    public void Victory()
    {
        if (deathSubmenu.activeSelf)
        {
            deathSubmenu.SetActive(false);
        }
        else if (pauseSubmenu.activeSelf)
        {
            pauseSubmenu.SetActive(false);
        }
        else
        {
            menu.SetActive(true);
            EnabledSettingsMenu();
            MenuEnabledEvent.Invoke();
        }
        victorySubmenu.SetActive(true);

        resumeMenu = "victory";
        //navigation.StartSelect("Victory");
    }

    public void SelectedState(bool var)
    {
        isSelectedState = var;
    }

    private void FlipSettingMark(bool checkPositive)
    {
        if (checkPositive)
        {
            if (settingsMarkRect.localScale.x < 0) return;
        }
        else
        {
            if (settingsMarkRect.localScale.x > 0) return;
        }

        Vector3 pos = settingsMarkRect.localPosition;
        pos.x = -pos.x;
        settingsMarkRect.localPosition = pos;
        
        Vector3 scale = settingsMarkRect.localScale;
        scale.x = -scale.x;
        settingsMarkRect.localScale = scale;
    }

    private void SetBackgroundMask(bool isOn)
    {
        Color color = background.color;
        color.a = isOn ? 0 : 1;
        background.color = color;
    }
}
