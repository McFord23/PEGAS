using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    List<Button> buttons = new List<Button>();
    public Button[] markButtons;
    public Button[] mainMenuButtons;
    public Button[] pauseButtons;
    public Button[] deadButtons;
    public Button[] victoryButtons;
    public Button[] settingsButtons;
    public List<Button> soloButtons = new List<Button>();
    public List<Button> togetherButtons = new List<Button>();

    int index;
    bool selectNone;

    void Awake()
    {
        buttons.AddRange(markButtons);
        buttons.AddRange(mainMenuButtons);
        buttons.AddRange(pauseButtons);
        buttons.AddRange(deadButtons);
        buttons.AddRange(victoryButtons);
        buttons.AddRange(settingsButtons);
        buttons.AddRange(soloButtons);
        buttons.AddRange(togetherButtons);
    }

    void Start()
    {
        if (Global.gameMode != GameMode.Single) SetTogetherMenuNavigation();
        else SetSoloMenuNavigation();
    }

    void Update()
    {
        if (selectNone)
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                buttons[index].Select();
                selectNone = false;
            }
        }
    }

    public void UpdateSelectIndex()
    {
        selectNone = true;
    }

    public void UpdateSelectIndex(Button button)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (button == buttons[i])
            {
                index = i;
                break;
            }
        }
    }

    public void StartSelect(string menu)
    {
        switch (menu)
        {
            case "Main Menu":
                UpdateSelectIndex(mainMenuButtons[0]);
                break;
            case "Pause":
                UpdateSelectIndex(pauseButtons[0]);
                break;
            case "Dead":
                UpdateSelectIndex(deadButtons[0]);
                break;
            case "Victory":
                UpdateSelectIndex(victoryButtons[0]);
                break;
        }
        selectNone = true;
        SetMarkNavigation(menu);
    }

    public void SetMarkNavigation(string menu)
    {
        switch (menu)
        {
            case "Main Menu":
                SetMarkNavigation(mainMenuButtons[0]);
                break;
            case "Pause":
                SetMarkNavigation(pauseButtons[0]);
                break;
            case "Dead":
                SetMarkNavigation(deadButtons[0]);
                break;
            case "Victory":
                SetMarkNavigation(victoryButtons[0]);
                break;
            case "Settings":
                SetMarkNavigation(settingsButtons[0]);
                break;
            case "Players":
                SetMarkNavigation(Global.gameMode != GameMode.Single ? togetherButtons[0] : soloButtons[0]);
                break;
        }
    }

    void SetMarkNavigation(Button startButton)
    {
        foreach (Button button in markButtons)
        {
            Navigation navigation = button.navigation;
            navigation.selectOnLeft = startButton;
            button.navigation = navigation;
        }
    }

    public void SetSoloMenuNavigation()
    {
        Navigation navigation;
        navigation = soloButtons[1].navigation;
        navigation.selectOnRight = soloButtons[0];
        soloButtons[1].navigation = navigation;

        navigation = soloButtons[2].navigation;
        navigation.selectOnRight = soloButtons[0];
        soloButtons[2].navigation = navigation;

        navigation = soloButtons[3].navigation;
        navigation.selectOnRight = soloButtons[0];
        soloButtons[3].navigation = navigation;

        SetMarkNavigation(soloButtons[0]);
        soloButtons[0].Select();
    }

    public void SetTogetherMenuNavigation()
    {
        Navigation navigation;
        navigation = soloButtons[1].navigation;
        navigation.selectOnRight = togetherButtons[1];
        soloButtons[1].navigation = navigation;

        navigation = soloButtons[2].navigation;
        navigation.selectOnRight = togetherButtons[2];
        soloButtons[2].navigation = navigation;

        navigation = soloButtons[3].navigation;
        navigation.selectOnRight = togetherButtons[3];
        soloButtons[3].navigation = navigation;

        SetMarkNavigation(togetherButtons[0]);
        togetherButtons[0].Select();
    }
}
