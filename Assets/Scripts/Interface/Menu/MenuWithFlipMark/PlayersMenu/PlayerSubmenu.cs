using System;
using UnityEngine;

public class PlayerSubmenu : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] private GameObject celestia;
    [SerializeField] private GameObject luna;
    [SerializeField] private LocalizationBase text;
    [SerializeField] private GameObject characterButton;
    
    [Header("Controls")]
    [SerializeField] private GameObject nextSchemeButton;
    [SerializeField] private GameObject previousSchemeButton;
    [SerializeField] private GameObject[] controlSchemes;
    private int indexBlocked;
    private int index;

    public void Initialize(ControlScheme controlScheme)
    {
        index = (int)controlScheme;

        for (var i = 0; i < controlSchemes.Length; i++)
        {
            controlSchemes[i].SetActive(i == index);
        }
    }

    public void SetScheme(ControlScheme controlScheme)
    {
        controlSchemes[index].SetActive(false);
        index = (int)controlScheme;
        controlSchemes[index].SetActive(true);
    }

    public ControlScheme GetScheme()
    {
        return (ControlScheme)Enum.GetValues(typeof(ControlScheme)).GetValue(index);
    }

    public void Block(ControlScheme indexAnotherPlayer)
    {
        indexBlocked = (int)indexAnotherPlayer;
    }

    public void NextScheme()
    {
        controlSchemes[index].SetActive(false);
        
        if (index < controlSchemes.Length - 1) index++;
        else index = 0;

        if (Settings.GameMode == GameMode.LocalCoop)
        {
            if (index == indexBlocked && indexBlocked == controlSchemes.Length - 1) index = 0;
            else if (index == indexBlocked) index++;
        }
        
        controlSchemes[index].SetActive(true);
    }

    public void PreviousScheme()
    {
        controlSchemes[index].SetActive(false);
        
        if (index > 0) index--;
        else index = controlSchemes.Length - 1;

        if (Settings.GameMode == GameMode.LocalCoop)
        {
            if (index == indexBlocked && indexBlocked == 0) index = controlSchemes.Length - 1;
            else if (index == indexBlocked) index--;
        }
        
        controlSchemes[index].SetActive(true);
    }

    public void ChangeCharacter(PlayerCharacter playerCharacter)
    {
        switch (playerCharacter)
        {
            case PlayerCharacter.Celestia:
                luna.SetActive(false);
                celestia.SetActive(true);
                text.UpdatePhrase("Interface", "celestia");
                break;
            
            case PlayerCharacter.Luna:
                celestia.SetActive(false);
                luna.SetActive(true);
                text.UpdatePhrase("Interface", "luna");
                break;
        }
    }

    public void ShowButton(bool value)
    {
        characterButton.SetActive(value);
        nextSchemeButton.SetActive(value);
        previousSchemeButton.SetActive(value);
    }
}
