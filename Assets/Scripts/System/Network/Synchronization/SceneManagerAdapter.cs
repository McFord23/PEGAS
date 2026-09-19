using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Абстрагирует локальный и сетевой переход между сценами 
 */
public class SceneManagerAdapter : SingletonNetworkBehaviour<SceneManagerAdapter>
{
    [SerializeField] private GameObject loadScreen;

    public static bool IsMenuScene()
    {
        return SceneManager.GetActiveScene().name == Level.MainMenu.ToString();
    }
    
    public static bool IsGameScene()
    {
        return !IsMenuScene() && SceneManager.GetActiveScene().name != Level.Credits.ToString();
    }
    
    public static Level GetActiveScene()
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
    
    public void LoadScene(Level level)
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