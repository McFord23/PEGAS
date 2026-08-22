using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MenuBaseWithImage
{
    [SerializeField] private Slider player1SensitivitySlider;
    [SerializeField] private Slider player2SensitivitySlider;
    [SerializeField] private GameObject player2Text;

    public override void Initialize(MenuManager manager, MenuBackground background)
    {
        base.Initialize(manager, background);
        
        player1SensitivitySlider.value = PlayersSettings.Player1.Sensitivity;
        player2SensitivitySlider.value = PlayersSettings.Player2.Sensitivity;
    }

    public override void SetActive(bool value)
    {
        base.SetActive(value);

        if (!value) return;
        
        var isSingle = Settings.GameMode is GameMode.Single; 
        player2Text.SetActive(!isSingle);
        player2SensitivitySlider.gameObject.SetActive(!isSingle);
    }
    
    public void SetPlayer1Sensitivity(float value)
    {
        PlayersSettings.Player1.Sensitivity = value;
    }

    public void SetPlayer2Sensitivity(float value)
    {
        PlayersSettings.Player2.Sensitivity = value;
    }
}
