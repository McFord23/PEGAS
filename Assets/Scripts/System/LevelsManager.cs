using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Подготавливает игру к переходу на другую сцену (сбрасывает временные статусы и подписки, а в сетевом коопе так же
 * отправлет запрос загрузки уровня на сервер). Выдаёт информацию по текущему уровню.
 */
public class LevelsManager : SingletonNetworkBehaviour<LevelsManager>
{
    [SerializeField] private GameObject loadScreen;

    public static bool IsMenuLevel()
    {
        return SceneManager.GetActiveScene().name == Level.MainMenu.ToString();
    }
    
    public static bool IsGameLevel()
    {
        return !IsMenuLevel() && SceneManager.GetActiveScene().name != Level.Credits.ToString();
    }
    
    public static Level GetActiveLevel()
    {
        return SceneManager.GetActiveScene().name switch
        {
            "MainMenu" => Level.MainMenu,
            "Credits" => Level.Credits,
            "DisciplinaryCleanup" => Level.DisciplinaryCleanup,
            "SantaSisters" => Level.SantaSisters,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public static bool IsFlyingLevel(Level level)
    {
        Level[] flyingLevels = 
        {
            Level.SantaSisters
        };

        return Array.Exists(flyingLevels, levelFromMassive => level == levelFromMassive);
    }

    public static bool IsLevelRequiresCoop(Level level)
    {
        Level[] levelsRequiresCoop = 
        {
            Level.DisciplinaryCleanup
        };

        return Array.Exists(levelsRequiresCoop, levelFromMassive => level == levelFromMassive);
    }
    
    public void LoadLevel(Level level)
    {
        var sceneName = level.ToString();
        switch (Settings.GameMode)
        {
            case GameMode.Single:
            case GameMode.LocalCoop:
                Global.Reset();
                ClearSubscribers();
                SceneManager.LoadScene(sceneName);
                break;

            case GameMode.Host:
            case GameMode.Client:
                loadScreen.SetActive(true);
                RequestLoadSceneServerRpc(new FixedString32Bytes(sceneName));
                break;
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestLoadSceneServerRpc(FixedString32Bytes sceneName)
    {
        RequestLoadSceneClientRpc(sceneName);
    }

    [ClientRpc]
    private void RequestLoadSceneClientRpc(FixedString32Bytes sceneName)
    {
        if (loadScreen) loadScreen.SetActive(true);

        if (Settings.GameMode == GameMode.Host)
        {
            Global.Reset();
            ClearSubscribers();
            NetworkManager.SceneManager.LoadScene(sceneName.ToString(), LoadSceneMode.Single);
        }
    }

    private void ClearSubscribers()
    {
        Settings.ClearSubscribers();
        PlayersSettings.ClearSubscribers();
        LocalizationManager.ClearSubscribers();
    }
}