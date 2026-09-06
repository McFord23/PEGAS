using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class LoadSync : NetworkBehaviour
{
    [SerializeField] private GameObject loadScreen;
    [SerializeField] private bool loadFromStart;

    private void Awake()
    {
        if (Settings.GameMode == GameMode.Host || Settings.GameMode == GameMode.Client)
        {
            if (Global.IsNetworkPlayerConnected && loadFromStart)
            {
                loadScreen.SetActive(true);
                NetworkManager.SceneManager.OnLoadEventCompleted += LoadCompleted;
            }
        }
    }

    private void LoadCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (clientsCompleted.Count > 1)
        {
            loadScreen.SetActive(false);
            NetworkManager.SceneManager.OnLoadEventCompleted -= LoadCompleted;
        }
    }
}
