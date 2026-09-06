using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MenuBaseWithImage
{
    [SerializeField] private Slider player1SensitivitySlider;
    [SerializeField] private Slider player2SensitivitySlider;
    [SerializeField] private LocalizationBase player1Label;
    [SerializeField] private LocalizationBase player2Label;

    public override void Initialize(MenuManager manager, MenuBackground background)
    {
        base.Initialize(manager, background);
        
        player1SensitivitySlider.value = PlayersSettings.Player1.Sensitivity;
        player2SensitivitySlider.value = PlayersSettings.Player2.Sensitivity;

        Settings.OnChangeGameModeEvent += OnChangeGameMode;
        PlayersSettings.OnSwapCharactersEvent += OnSwapCharacters;
        
        OnChangeGameMode();
        OnSwapCharacters();
    }

    private void OnDestroy()
    {
        Settings.OnChangeGameModeEvent -= OnChangeGameMode;
        PlayersSettings.OnSwapCharactersEvent -= OnSwapCharacters;
    }

    public void SetPlayer1Sensitivity(float value)
    {
        PlayersSettings.Player1.Sensitivity = value;
    }

    public void SetPlayer2Sensitivity(float value)
    {
        PlayersSettings.Player2.Sensitivity = value;
    }

    private void OnChangeGameMode()
    {
        switch (Settings.GameMode)
        {
            case GameMode.Single:
            case GameMode.Host:
                player1Label.gameObject.SetActive(true);
                player1SensitivitySlider.gameObject.SetActive(true);
                
                player2Label.gameObject.SetActive(false);
                player2SensitivitySlider.gameObject.SetActive(false);
                break;
            
            case GameMode.Client:
                player1Label.gameObject.SetActive(false);
                player1SensitivitySlider.gameObject.SetActive(false);
                
                player2Label.gameObject.SetActive(true);
                player2SensitivitySlider.gameObject.SetActive(true);
                break;
            
            case GameMode.LocalCoop:
                player1Label.gameObject.SetActive(true);
                player1SensitivitySlider.gameObject.SetActive(true);
                
                player2Label.gameObject.SetActive(true);
                player2SensitivitySlider.gameObject.SetActive(true);
                break;
        }
    }

    private void OnSwapCharacters()
    {
        var character1 = Utilities.ToCamelCase(PlayersSettings.Player1.Character.ToString());
        var character2 = Utilities.ToCamelCase(PlayersSettings.Player2.Character.ToString());
        
        player1Label.UpdatePhrase("Interface", character1);
        player2Label.UpdatePhrase("Interface", character2);
    }
}
