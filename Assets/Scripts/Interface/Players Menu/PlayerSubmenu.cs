using Enums;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSubmenu : MonoBehaviour
{
    private PlayersMenu players;
    private Sprite[] layoutSprites;
    private Image layoutSprite;

    public ControlLayout layout { private set; get; }
    private int index;
    private int indexBlocked;

    public GameObject celestia;
    public GameObject luna;
    public Text text;

    private GameObject characterButton;
    private GameObject nextButton;
    private GameObject perviousButton;

    public void Initialize()
    {
        players = transform.parent.GetComponent<PlayersMenu>();
        layoutSprites = players.controlLayoutSprites;
        layoutSprite = transform.Find("Control Layout").GetComponent<Image>();

        characterButton = transform.Find("Character/Change Character Icon").gameObject;
        nextButton = transform.Find("Control Layout/Next Layout").gameObject;
        perviousButton = transform.Find("Control Layout/Pervious Layout").gameObject;
        
        index = (int)layout;
        layoutSprite.sprite = layoutSprites[index];
    }

    public void SetLayout(ControlLayout controlLayout)
    {
        layout = controlLayout;
        index = (int)controlLayout;
        layoutSprite.sprite = layoutSprites[index];
    }

    public void Block(ControlLayout indexAnotherPlayer)
    {
        indexBlocked = (int)indexAnotherPlayer;
    }

    public void NextLayout()
    {
        if (index < layoutSprites.Length - 1) index++;
        else index = 0;

        if (Global.gameMode == GameMode.LocalCoop)
        {
            if (index == indexBlocked && indexBlocked == layoutSprites.Length - 1) index = 0;
            else if (index == indexBlocked) index++;
        }
        
        layoutSprite.sprite = layoutSprites[index];
        layout = (ControlLayout)index;

    }

    public void PerviousLayout()
    {
        if (index > 0) index--;
        else index = layoutSprites.Length - 1;

        if (Global.gameMode == GameMode.LocalCoop)
        {
            if (index == indexBlocked && indexBlocked == 0) index = layoutSprites.Length - 1;
            else if (index == indexBlocked) index--;
        }

        layoutSprite.sprite = layoutSprites[index];
        layout = (ControlLayout)index;
    }

    public void ChangeCharacter(Character character)
    {
        switch (character)
        {
            case Character.Celestia:
                luna.SetActive(false);
                celestia.SetActive(true);
                text.text = "Celestia";
                break;
            
            case Character.Luna:
                celestia.SetActive(false);
                luna.SetActive(true);
                text.text = "Luna";
                break;
        }
    }

    public void ShowButton(bool value)
    {
        characterButton.SetActive(value);
        nextButton.SetActive(value);
        perviousButton.SetActive(value);
    }
}
