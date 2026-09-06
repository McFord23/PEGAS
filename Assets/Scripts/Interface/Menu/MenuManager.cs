using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject block;
    private GameObject currentPopup;
    
    [SerializeField] private List<MenuBase> menus;
    private MenuBase currentMenu;
    private MenuBase mainMenu;
    private MenuBase gameMenu;

    public UnityEvent MenuEnabledEvent;
    public UnityEvent MenuDisabledEvent;
    public UnityEvent ChangeMenuEvent;

    private bool isSelectedState;

    private void Start()
    {
        var menuBackground = GetComponent<MenuBackground>();
        menuBackground.Initialize();
        
        foreach (var menu in menus)
        {
            menu.Initialize(this, menuBackground);
        }
        
        if (SceneManagerAdapter.IsMenuScene())
        {
            mainMenu = menus.Find(menu => menu is MainMenu);
            currentMenu = mainMenu;
            mainMenu.SetActive(true);
            Enable();
        }
        else
        {
            gameMenu = menus.Find(menu => menu is GameMenu);
            currentMenu = gameMenu;
            gameMenu.SetActive(true);
            Disable();
        }
    }

    private void Update()
    {
        if (Controls.Pause & !isSelectedState)
        {
            Back();
        }
    }

    public void UpdateCurrentMenu(MenuBase newMenuBase)
    {
        currentMenu.SetActive(false);
        currentMenu = newMenuBase;
        
        var newMenuIndex = menus.IndexOf(newMenuBase);

        // закладки выше налево
        for (var i = 0; i < newMenuIndex; i++)
        {
            menus[i].UpdateMark(false);
        }
        
        // закладки ниже направо
        for (var i = menus.Count - 1; i > newMenuIndex; i--)
        {
            menus[i].UpdateMark(true);
        }
        
        ChangeMenuEvent.Invoke();
    }

    public void ShowPopup(GameObject popup)
    {
        if (currentPopup)
        {
            currentPopup.SetActive(false);
            currentPopup = null;
        }

        currentPopup = popup;
        popup.SetActive(true);
        block.SetActive(true);
    }

    public void HidePopup()
    {
        currentPopup.SetActive(false);
        currentPopup = null;
        block.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        MenuEnabledEvent.Invoke();
    }
    
    public void Disable()
    {
        gameObject.SetActive(false);
        MenuDisabledEvent.Invoke();
    }

    public void Back()
    {
        if (currentPopup)
        {
            currentPopup.SetActive(false);
            currentPopup = null;
            block.SetActive(false);
            return;
        }
        
        if (SceneManagerAdapter.IsMenuScene())
        {
            if (currentMenu == mainMenu) return;
            mainMenu.SetActive(true);
            ChangeMenuEvent.Invoke();
        }
        else
        {
            if (currentMenu == gameMenu) return;
            gameMenu.SetActive(true);
            ChangeMenuEvent.Invoke();
        }
    }

    public void SelectedState(bool var)
    {
        isSelectedState = var;
    }
}
