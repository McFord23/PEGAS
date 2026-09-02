using UnityEngine;
using UnityEngine.UI;

public class PlayersMenu : MenuBaseWithFlipMark
{
    [SerializeField] private RectTransform backMain;
    [SerializeField] private CoopStatus coopStatus;
    
    [Header("Icon")]
    [SerializeField] private Image playersMarkIcon;
    [SerializeField] private Sprite soloIcon;
    [SerializeField] private Sprite coopIcon;
    
    [Header("Player 1")]
    [SerializeField] private PlayerSubmenu player1Submenu;
    [SerializeField] private LocalizationBase player1Label;

    [Header("Player 2")]
    [SerializeField] private PlayerSubmenu player2Submenu;
    [SerializeField] private LocalizationBase player2Label;

    [Header("Coop")] 
    [SerializeField] private GameObject coopSubmenu;
    [SerializeField] private GameObject networkSubmenu;
    [SerializeField] private CloseCoopButton closeCoopButton;
    
    [Header("Controls")]
    [SerializeField] private Sprite[] controlSchemeSprites;
    
    public override void Initialize(MenuManager manager, MenuBackground background)
    {
        base.Initialize(manager, background);
        
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
    }

    public void LocalCoop()
    {
        Settings.ChangeGameMode(GameMode.LocalCoop);
        SetActiveCoopSubmenu(false);

        if (PlayersSettings.Player1.ControlScheme == PlayersSettings.Player2.ControlScheme)
        {
            player2Submenu.NextScheme();
        }
        
        ShowPlayer2Submenu();
        closeCoopButton.ChangeMode(CloseCoopButton.Mode.LocalKick);
    }

    public void NetworkCoop()
    {
        SetActiveCoopSubmenu(false);
        networkSubmenu.SetActive(true);
        closeCoopButton.ChangeMode(CloseCoopButton.Mode.Back);
    }

    public void LocalKick()
    {
        Settings.ChangeGameMode(GameMode.Single);
        SetActiveCoopSubmenu(true);
        HidePlayer2Submenu();
    }

    public void BackCoop()
    {
        networkSubmenu.SetActive(false);
        SetActiveCoopSubmenu(true);
    }

    public void ShowPlayer2Submenu()
    {
        switch (Settings.GameMode)
        {
            case GameMode.LocalCoop:
                coopStatus.SetMode(CoopStatus.Mode.Local);
                player1Label.UpdatePhrase("Interface", "player1");
                player2Label.UpdatePhrase("Interface", "player2");

                player2Submenu.ShowButton(true);
                player1Submenu.ShowButton(true);
                break;

            case GameMode.Host:
                coopStatus.SetMode(CoopStatus.Mode.Network);
                player1Label.UpdatePhrase("Interface", "you");
                player2Label.UpdatePhrase("Interface", "sister");

                player2Submenu.ShowButton(false);
                player1Submenu.ShowButton(true);
                break;

            case GameMode.Client:
                coopStatus.SetMode(CoopStatus.Mode.Network);
                player1Label.UpdatePhrase("Interface", "sister");
                player2Label.UpdatePhrase("Interface", "you");

                player2Submenu.ShowButton(true);
                player1Submenu.ShowButton(false);
                break;
        }

        player2Submenu.gameObject.SetActive(true);

        UpdateCloseButton();
        UpdatePlayersIcon();
    }

    public void HidePlayer2Submenu()
    {
        player1Label.UpdatePhrase("Interface", "player1");
        player2Label.UpdatePhrase("Interface", "player2");

        player1Submenu.ShowButton(true);
        player2Submenu.gameObject.SetActive(false);
        UpdatePlayersIcon();
    }

    public void ChangeCharacter()
    {
        PlayersSettings.SwapCharacters();
        player1Submenu.ChangeCharacter(PlayersSettings.Player1.Character);
        player2Submenu.ChangeCharacter(PlayersSettings.Player2.Character);
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
    
    private void UpdatePlayersIcon()
    {
        playersMarkIcon.sprite = Settings.GameMode is GameMode.Single 
            ? soloIcon 
            : coopIcon;
    }
    
    private void UpdateCloseButton()
    {
        switch (Settings.GameMode)
        {
            case GameMode.LocalCoop:
                closeCoopButton.ChangeMode(CloseCoopButton.Mode.LocalKick);
                break;

            case GameMode.Host:
                closeCoopButton.ChangeMode(CloseCoopButton.Mode.ShutDown);
                break;
                
            case GameMode.Client:
                closeCoopButton.ChangeMode(CloseCoopButton.Mode.Quick);
                break;
        }
    }

    private void SetActiveCoopSubmenu(bool value)
    {
        coopSubmenu.SetActive(value);
        closeCoopButton.gameObject.SetActive(!value);
        if (value) coopStatus.SetMode(CoopStatus.Mode.None);
    }
}
