using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayersMenu : MonoBehaviour
{
    [SerializeField] private RectTransform backMain;
    [SerializeField] private Text modeTitle;
    
    [Header("Player 1")]
    [SerializeField] private PlayerSubmenu player1Submenu;
    [SerializeField] private Text p1;
    [SerializeField] private Image player1GamepadImage;

    [Header("Player 2")]
    [SerializeField] private PlayerSubmenu player2Submenu;
    [SerializeField] private Text p2;
    [SerializeField] private Image player2GamepadImage;
    
    [Header("Local Coop")]
    [SerializeField] private GameObject coopSubmenu;
    [SerializeField] private GameObject localBannishButton;
    [SerializeField] private GameObject backCoop;
    
    [Header("Network Coop")]
    [SerializeField] private GameObject networkSubmenu;
    [SerializeField] private GameObject networkBannishButton;
    [SerializeField] private GameObject quitButton;
    
    [FormerlySerializedAs("controlLayoutSprites")] [Header("Controls")]
    public Sprite[] controlSchemeSprites;
    public Sprite[] gamepadSprites;

    public void Initialize()
    {
        player1Submenu.Initialize(controlSchemeSprites);
        player2Submenu.Initialize(controlSchemeSprites);
        
        switch (Settings.GameMode)
        {            
            case GameMode.LocalCoop:
                LocalCoop();
                break;
            
            case GameMode.Host:
            case GameMode.Client:
                NetworkCoop();
                break;
        }

        networkSubmenu.GetComponent<NetworkSubmenu>().Initialize(this);

        player1Submenu.ChangeCharacter(PlayersSettings.Player1.Character);
        player2Submenu.ChangeCharacter(PlayersSettings.Player2.Character);
        
        UpdatePlayersSchemes();
        UpdateGamepadStatus();
    }

    public void LocalCoop()
    {
        Settings.GameMode = GameMode.LocalCoop;
        coopSubmenu.SetActive(false);

        if (PlayersSettings.Player1.ControlScheme == PlayersSettings.Player2.ControlScheme)
        {
            player2Submenu.NextScheme();
        }

        UpdateBackButtons(true);
        ShowPlayer2Submenu();
    }

    public void NetworkCoop()
    {
        coopSubmenu.SetActive(false);
        networkSubmenu.SetActive(true);
        modeTitle.text = "Network Coop";

        UpdateBackButtons(false);
    }

    public void LocalBannish()
    {
        Settings.GameMode = GameMode.Single;
        coopSubmenu.SetActive(true);
        HidePlayer2Submenu();
    }

    public void BackCoop()
    {
        networkSubmenu.SetActive(false);
        coopSubmenu.SetActive(true);

        UpdateBackButtons(true);
    }

    public void UpdateBackButtons(bool toOne)
    {
        if (toOne)
        {
            backMain.anchoredPosition = new Vector2(0, -311);
            backCoop.SetActive(false);
        }
        else
        {
            backMain.anchoredPosition = new Vector2(0, -287);
            backCoop.SetActive(true);
        }
    }

    private void UpdateKickButton()
    {
        switch (Settings.GameMode)
        {
            case GameMode.LocalCoop:
                quitButton.SetActive(false);
                networkBannishButton.SetActive(false);
                localBannishButton.SetActive(true);
                break;

            case GameMode.Host:
                quitButton.SetActive(false);
                localBannishButton.SetActive(false);
                networkBannishButton.SetActive(true);
                break;
                
            case GameMode.Client:
                localBannishButton.SetActive(false);
                networkBannishButton.SetActive(false);
                quitButton.SetActive(true);
                break;
        }
    }

    public void ShowPlayer2Submenu()
    {
        switch (Settings.GameMode)
        {
            case GameMode.LocalCoop:
                modeTitle.text = "Local Coop";
                p1.text = "Player 1";
                p2.text = "Player 2";

                player2Submenu.ShowButton(true);
                player1Submenu.ShowButton(true);
                break;

            case GameMode.Host:
                modeTitle.text = "Network Coop";
                p1.text = "You";
                p2.text = "Sister";

                player2Submenu.ShowButton(false);
                player1Submenu.ShowButton(true);;
                break;

            case GameMode.Client:
                modeTitle.text = "Network Coop";
                p1.text = "Sister";
                p2.text = "You";

                player2Submenu.ShowButton(true);
                player1Submenu.ShowButton(false);
                break;
        }

        player2Submenu.gameObject.SetActive(true);

        UpdateKickButton();
        UpdateGamepadStatus();
        MenuManager.Instance.UpdatePlayersIcon(true);
    }

    public void HidePlayer2Submenu()
    {
        modeTitle.text = "";
        p1.text = "Player 1";
        p2.text = "Player 2";

        player1Submenu.ShowButton(true);
        player2Submenu.gameObject.SetActive(false);
        UpdateGamepadStatus();
        MenuManager.Instance.UpdatePlayersIcon(false);
    }

    public void ChangeCharacter()
    {
        PlayersSettings.SwapCharacters();
        player1Submenu.ChangeCharacter(PlayersSettings.Player1.Character);
        player2Submenu.ChangeCharacter(PlayersSettings.Player2.Character);
    }
    
    public void UpdateGamepadStatus()
    {
        switch (Settings.GameMode)
        {
            case GameMode.Single:
                player1GamepadImage.sprite = PlayersSettings.Player1.Gamepad ? gamepadSprites[0] : gamepadSprites[3];
                break;

            case GameMode.LocalCoop:
                player1GamepadImage.sprite = PlayersSettings.Player1.Gamepad ? gamepadSprites[1] : gamepadSprites[3];
                player2GamepadImage.sprite = PlayersSettings.Player2.Gamepad ? gamepadSprites[2] : gamepadSprites[3];
                break;

            case GameMode.Host:
            case GameMode.Client:
                player1GamepadImage.sprite = PlayersSettings.Player1.Gamepad ? gamepadSprites[0] : gamepadSprites[3];
                player2GamepadImage.sprite = PlayersSettings.Player2.Gamepad ? gamepadSprites[0] : gamepadSprites[3];
                break;
        }
    }

    public void ChangePlayer1Scheme()
    {
        PlayersSettings.Player1.ControlScheme = player1Submenu.Scheme;
        player2Submenu.Block(player1Submenu.Scheme);
    }

    public void ChangePlayer2Scheme()
    {
        PlayersSettings.Player2.ControlScheme = player2Submenu.Scheme;
        player1Submenu.Block(player2Submenu.Scheme);
    }

    public void UpdatePlayersSchemes()
    {
        player1Submenu.SetScheme(PlayersSettings.Player1.ControlScheme);
        player2Submenu.SetScheme(PlayersSettings.Player2.ControlScheme);
    }
}
