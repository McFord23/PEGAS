using UnityEngine;

public class UnavailableWarning : MonoBehaviour
{
    private void Start()
    {
        Settings.OnChangeGameModeEvent += OnGameModeChanged;
        OnGameModeChanged();
    }

    private void OnGameModeChanged()
    {
        gameObject.SetActive(Settings.GameMode is GameMode.Single);
    }
}