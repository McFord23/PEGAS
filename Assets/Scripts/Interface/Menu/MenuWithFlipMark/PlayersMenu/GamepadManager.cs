using UnityEngine;
using UnityEngine.UI;

public class GamepadManager : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite[] gamepadSprites;
 
    [Header("Images")]
    [SerializeField] private Image player1GamepadImage;
    [SerializeField] private Image player2GamepadImage;
    [SerializeField] private Image sharedGamepadImage;

    [Header("Buttons")]
    [SerializeField] private GameObject showPlayer1GamepadSchemeButton;
    [SerializeField] private GameObject showPlayer2GamepadSchemeButton;
    [SerializeField] private GameObject shareFromPlayer1Button;
    [SerializeField] private GameObject shareFromPlayer2Button;

    private void Start()
    {
        Settings.OnChangeGameModeEvent += UpdateGamepadStatus;
        UpdateGamepadStatus();
    }

    private void OnDestroy()
    {
        Settings.OnChangeGameModeEvent -= UpdateGamepadStatus;
    }

    public void UpdateGamepadStatus()
    {
        switch (Settings.GameMode)
        {
            case GameMode.Single:
                if (PlayersSettings.Player1.Gamepad == null)
                {
                    if (PlayersSettings.Player2.Gamepad != null)
                    {
                        GiveGamepad(true);
                        showPlayer1GamepadSchemeButton.SetActive(true);
                        player1GamepadImage.sprite = gamepadSprites[0];
                    }
                    else
                    {
                        showPlayer1GamepadSchemeButton.SetActive(false);
                        player1GamepadImage.sprite = gamepadSprites[3];
                    }
                }
                else
                {
                    showPlayer1GamepadSchemeButton.SetActive(true);
                    player1GamepadImage.sprite = gamepadSprites[0];
                }
                break;

            case GameMode.LocalCoop:
                var isPlayer1HaveGamepad = PlayersSettings.Player1.Gamepad != null;
                showPlayer1GamepadSchemeButton.SetActive(isPlayer1HaveGamepad);
                if (isPlayer1HaveGamepad)
                {
                    player1GamepadImage.sprite = PlayersSettings.Player2.Gamepad == null
                        ? gamepadSprites[0]
                        : gamepadSprites[1];
                }
                else
                {
                    player1GamepadImage.sprite = gamepadSprites[3];
                }
                
                var isPlayer2HaveGamepad = PlayersSettings.Player2.Gamepad != null;
                showPlayer2GamepadSchemeButton.SetActive(isPlayer2HaveGamepad);
                if (isPlayer2HaveGamepad)
                {
                    player2GamepadImage.sprite = PlayersSettings.Player1.Gamepad == null
                        ? gamepadSprites[0]
                        : gamepadSprites[2];
                }
                else
                {
                    player2GamepadImage.sprite = gamepadSprites[3];
                }
                break;

            case GameMode.Host:
            case GameMode.Client:
                var isHostHaveGamepad = PlayersSettings.Player1.Gamepad != null || PlayersSettings.Player1.NetworkGamepad;
                showPlayer1GamepadSchemeButton.SetActive(isHostHaveGamepad);
                player1GamepadImage.sprite = isHostHaveGamepad
                    ? gamepadSprites[0] 
                    : gamepadSprites[3];
                
                var isClientHaveGamepad = PlayersSettings.Player2.Gamepad != null || PlayersSettings.Player2.NetworkGamepad;
                showPlayer2GamepadSchemeButton.SetActive(isClientHaveGamepad);
                player2GamepadImage.sprite = isClientHaveGamepad 
                    ? gamepadSprites[0] 
                    : gamepadSprites[3];
                break;
        }

        if (PlayersSettings.CanShareGamepad())
        {
            shareFromPlayer1Button.SetActive(PlayersSettings.Player1.Gamepad != null);
            shareFromPlayer2Button.SetActive(PlayersSettings.Player2.Gamepad != null);
        }
        else
        {
            shareFromPlayer1Button.SetActive(false);
            shareFromPlayer2Button.SetActive(false);
        }
    }

    public void ShareGamepad(bool isFromPlayer1)
    {
        if (!PlayersSettings.CanShareGamepad()) return;
        
        PlayersSettings.ShareGamepad(isFromPlayer1);
        
        player1GamepadImage.gameObject.SetActive(false);
        player2GamepadImage.gameObject.SetActive(false);
        
        shareFromPlayer1Button.SetActive(false);
        shareFromPlayer2Button.SetActive(false);
        
        sharedGamepadImage.gameObject.SetActive(true);
    }
    
    public void GiveGamepad(bool isToPlayer1)
    {
        if (!PlayersSettings.CanShareGamepad()) return;
        
        PlayersSettings.GiveGamepad(isToPlayer1);
        
        sharedGamepadImage.gameObject.SetActive(false);
        
        player1GamepadImage.gameObject.SetActive(true);
        player2GamepadImage.gameObject.SetActive(true);
        
        shareFromPlayer1Button.SetActive(isToPlayer1);
        shareFromPlayer2Button.SetActive(!isToPlayer1);
        
        UpdateGamepadStatus();
    }
}