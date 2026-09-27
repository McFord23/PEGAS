using UnityEngine;

public class ControlHint : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private bool isPlayer1;
    [SerializeField] private LocalizationBase character;
    private PlayersSettings.Player player;
    
    [Header("Control")]
    [SerializeField] private GameObject wasdIcon;
    [SerializeField] private GameObject arrowsIcon;
    [SerializeField] private GameObject mouseIcon;
    [SerializeField] private GameObject gamepadIcon;
    [SerializeField] private GameObject sharedGamepadIcon;
    private GameObject currentIcon;
    
    private void Start()
    {
        player = isPlayer1 ? PlayersSettings.Player1 : PlayersSettings.Player2;
        Settings.OnChangeGameModeEvent += OnGameModeChanged;
        PlayersSettings.OnChangeControlEvent += OnControlSchemeChanged;
        PlayersSettings.OnSwapCharactersEvent += OnCharacterSwapped;
        PlayersSettings.OnChangeShareGamepadEvent += OnControlSchemeChanged;

        currentIcon = wasdIcon;
        OnControlSchemeChanged();
        OnCharacterSwapped();
        OnGameModeChanged();
    }

    private void OnGameModeChanged()
    {
        var value = isPlayer1 || Settings.GameMode is not GameMode.Single;
        gameObject.SetActive(value);
    }
    
    private void OnControlSchemeChanged()
    {
        currentIcon.SetActive(false);

        if (player.Gamepad != null)
        {
            currentIcon = PlayersSettings.IsSharedGamepad ? sharedGamepadIcon : gamepadIcon;
        }
        else
        {
            currentIcon = player.ControlScheme switch
            {
                ControlScheme.WASD => wasdIcon,
                ControlScheme.Arrows => arrowsIcon,
                ControlScheme.Mouse => mouseIcon,
                _ => currentIcon
            };
        }

        currentIcon.SetActive(true);
    }
    
    private void OnCharacterSwapped()
    {
        character.UpdatePhrase("Interface", Utilities.ToCamelCase(player.Character.ToString()));
    }
}