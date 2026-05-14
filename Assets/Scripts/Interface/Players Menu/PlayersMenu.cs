using Enums;
using UnityEngine;
using UnityEngine.UI;

public class PlayersMenu : MonoBehaviour
{
    private MenuManager menu;
    private GameObject coopSubmenu;
    private GameObject networkSubmenu;

    private PlayerSubmenu player1Submenu;
    private Text p1;
    
    private RectTransform backMain;
    private GameObject backCoop;

    private PlayerSubmenu player2Submenu;
    private Text p2;
    private Text player2Title;
    
    private GameObject localBannishButton;
    private GameObject networkBannishButton;
    private GameObject quitButton;

    public Sprite[] controlLayoutSprites;
    public Sprite[] gamepadSprites;
    private Image player1GamepadImage;
    private Image player2GamepadImage;

    public void Initialize()
    {
        menu = GetComponentInParent<MenuManager>();

        player1Submenu = transform.Find("Player #1").GetComponent<PlayerSubmenu>();
        p1 = player1Submenu.transform.Find("P1Text").GetComponent<Text>();
        
        backMain = player1Submenu.transform.Find("Back Main").GetComponent<RectTransform>();
        backCoop = player1Submenu.transform.Find("Back Coop").gameObject;

        player2Submenu = transform.Find("Player #2").GetComponent<PlayerSubmenu>();
        player2Title = player2Submenu.transform.Find("Title").GetComponent<Text>();
        p2 = player2Submenu.transform.Find("P2Text").GetComponent<Text>();
        
        localBannishButton = player2Submenu.transform.Find("Local Bannish").gameObject;
        networkBannishButton = player2Submenu.transform.Find("Network Bannish").gameObject;
        quitButton = player2Submenu.transform.Find("Quit").gameObject;

        player1GamepadImage = transform.Find("Player #1/Gamepad").GetComponent<Image>();
        player2GamepadImage = transform.Find("Player #2/Gamepad").GetComponent<Image>();

        player1Submenu.Initialize();
        player2Submenu.Initialize();
        
        coopSubmenu = transform.Find("Coop Submenu").gameObject;
        networkSubmenu = transform.Find("Network Submenu").gameObject;
        
        switch (Global.gameMode)
        {            
            case GameMode.LocalCoop:
                LocalCoop();
                break;
            
            case GameMode.Host:
            case GameMode.Client:
                NetworkCoop();
                break;
        }

        networkSubmenu.GetComponent<NetworkSubmenu>().Initialize();

        player1Submenu.ChangeCharacter(Global.players[0].character);
        player2Submenu.ChangeCharacter(Global.players[1].character);
        
        UpdatePlayersLayout();
        UpdateGamepadStatus();
    }

    public void LocalCoop()
    {
        Global.gameMode = GameMode.LocalCoop;
        coopSubmenu.SetActive(false);

        if (Global.players[0].controlLayout == Global.players[1].controlLayout)
        {
            player2Submenu.NextLayout();
        }

        UpdateBackButtons(true);
        ShowPlayer2Submenu();
    }

    public void NetworkCoop()
    {
        coopSubmenu.SetActive(false);
        networkSubmenu.SetActive(true);
        player2Title.text = "Network Coop";

        UpdateBackButtons(false);
    }

    public void LocalBannish()
    {
        Global.gameMode = GameMode.Single;
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
        switch (Global.gameMode)
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
        switch (Global.gameMode)
        {
            case GameMode.LocalCoop:
                player2Title.text = "Local Coop";
                p1.text = "Player 1";
                p2.text = "Player 2";

                player2Submenu.ShowButton(true);
                player1Submenu.ShowButton(true);
                break;

            case GameMode.Host:
                player2Title.text = "Network Coop";
                p1.text = "You";
                p2.text = "Sister";

                player2Submenu.ShowButton(false);
                player1Submenu.ShowButton(true);;
                break;

            case GameMode.Client:
                player2Title.text = "Network Coop";
                p1.text = "Sister";
                p2.text = "You";

                player2Submenu.ShowButton(true);
                player1Submenu.ShowButton(false);
                break;
        }

        player2Submenu.gameObject.SetActive(true);

        UpdateKickButton();
        UpdateGamepadStatus();
        menu.UpdatePlayersIcon(true);
    }

    public void HidePlayer2Submenu()
    {
        player2Title.text = "";
        p1.text = "Player 1";
        p2.text = "Player 2";

        player1Submenu.ShowButton(true);
        player2Submenu.gameObject.SetActive(false);
        menu.UpdatePlayersIcon(false);
    }

    public void ChangeCharacter()
    {
        if (Global.players[0].character == Character.Celestia)
        {
            Global.players[0].character = Character.Luna;
            Global.players[1].character = Character.Celestia;

            player1Submenu.ChangeCharacter(Character.Luna);
            player2Submenu.ChangeCharacter(Character.Celestia);
        }
        else
        {
            Global.players[0].character = Character.Celestia;
            Global.players[1].character = Character.Luna;

            player1Submenu.ChangeCharacter(Character.Celestia);
            player2Submenu.ChangeCharacter(Character.Luna);
        }
    }

    public void ChangeHostCharacter(Character player)
    {
        player1Submenu.ChangeCharacter(player);
    }

    public void ChangeClientCharacter(Character player)
    {
        player2Submenu.ChangeCharacter(player);
    }
    
    public void UpdateGamepadStatus()
    {
        switch (Global.gameMode)
        {
            case GameMode.Single:
                if (Gamepad.gamepad1) player1GamepadImage.sprite = gamepadSprites[0];
                else player1GamepadImage.sprite = gamepadSprites[3];
                break;

            case GameMode.LocalCoop:
                if (Gamepad.gamepad1) player1GamepadImage.sprite = gamepadSprites[1];
                else player1GamepadImage.sprite = gamepadSprites[3];

                if (Gamepad.gamepad2) player2GamepadImage.sprite = gamepadSprites[2];
                else player2GamepadImage.sprite = gamepadSprites[3];
                break;

            case GameMode.Host:
            case GameMode.Client:
                if (Gamepad.gamepad1) player1GamepadImage.sprite = gamepadSprites[0];
                else player1GamepadImage.sprite = gamepadSprites[3];

                if (Gamepad.gamepad2) player2GamepadImage.sprite = gamepadSprites[0];
                else player2GamepadImage.sprite = gamepadSprites[3];
                break;
        }
    }

    public void ChangePlayer1Layout()
    {
        Global.players[0].controlLayout = player1Submenu.layout;
        player2Submenu.Block(player1Submenu.layout);
    }

    public void ChangePlayer2Layout()
    {
        Global.players[1].controlLayout = player2Submenu.layout;
        player1Submenu.Block(player2Submenu.layout);
    }

    public void UpdatePlayersLayout()
    {
        player1Submenu.SetLayout(Global.players[0].controlLayout);
        player2Submenu.SetLayout(Global.players[1].controlLayout);
    }
}
