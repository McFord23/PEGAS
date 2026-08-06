using UnityEngine;
using UnityEngine.UI;

public class PlayerSubmenu : MonoBehaviour
{
    [Header("Character")]
    public GameObject celestia;
    public GameObject luna;
    public Text text;
    [SerializeField] private GameObject characterButton;
    
    [Header("Controls")]
    public ControlLayout Layout { private set; get; }
    [SerializeField] private GameObject nextControlButton;
    [SerializeField] private GameObject previousControlButton;
    [SerializeField] private Image layoutSprite;
    private Sprite[] layoutSprites;
    private int indexBlocked;
    private int index;

    public void Initialize(Sprite[] initialLayoutSprites)
    {
        layoutSprites = initialLayoutSprites;
        index = (int)Layout;
        layoutSprite.sprite = layoutSprites[index];
    }

    public void SetLayout(ControlLayout controlLayout)
    {
        Layout = controlLayout;
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

        if (Settings.GameMode == GameMode.LocalCoop)
        {
            if (index == indexBlocked && indexBlocked == layoutSprites.Length - 1) index = 0;
            else if (index == indexBlocked) index++;
        }
        
        layoutSprite.sprite = layoutSprites[index];
        Layout = (ControlLayout)index;

    }

    public void PerviousLayout()
    {
        if (index > 0) index--;
        else index = layoutSprites.Length - 1;

        if (Settings.GameMode == GameMode.LocalCoop)
        {
            if (index == indexBlocked && indexBlocked == 0) index = layoutSprites.Length - 1;
            else if (index == indexBlocked) index--;
        }

        layoutSprite.sprite = layoutSprites[index];
        Layout = (ControlLayout)index;
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
        nextControlButton.SetActive(value);
        previousControlButton.SetActive(value);
    }
}
