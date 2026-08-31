using UnityEngine;
using UnityEngine.UI;

public class PlayerSubmenu : MonoBehaviour
{
    [Header("Character")]
    public GameObject celestia;
    public GameObject luna;
    public LocalizationBase text;
    [SerializeField] private GameObject characterButton;
    
    [Header("Controls")]
    public ControlScheme Scheme { private set; get; }
    [SerializeField] private GameObject nextSchemeButton;
    [SerializeField] private GameObject previousSchemeButton;
    [SerializeField] private Image schemeSprite;
    private Sprite[] schemesSprites;
    private int indexBlocked;
    private int index;

    public void Initialize(Sprite[] initialLayoutSprites)
    {
        schemesSprites = initialLayoutSprites;
        index = (int)Scheme;
        schemeSprite.sprite = schemesSprites[index];
    }

    public void SetScheme(ControlScheme controlScheme)
    {
        Scheme = controlScheme;
        index = (int)controlScheme;
        schemeSprite.sprite = schemesSprites[index];
    }

    public void Block(ControlScheme indexAnotherPlayer)
    {
        indexBlocked = (int)indexAnotherPlayer;
    }

    public void NextScheme()
    {
        if (index < schemesSprites.Length - 1) index++;
        else index = 0;

        if (Settings.GameMode == GameMode.LocalCoop)
        {
            if (index == indexBlocked && indexBlocked == schemesSprites.Length - 1) index = 0;
            else if (index == indexBlocked) index++;
        }
        
        schemeSprite.sprite = schemesSprites[index];
        Scheme = (ControlScheme)index;

    }

    public void PreviousScheme()
    {
        if (index > 0) index--;
        else index = schemesSprites.Length - 1;

        if (Settings.GameMode == GameMode.LocalCoop)
        {
            if (index == indexBlocked && indexBlocked == 0) index = schemesSprites.Length - 1;
            else if (index == indexBlocked) index--;
        }

        schemeSprite.sprite = schemesSprites[index];
        Scheme = (ControlScheme)index;
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
