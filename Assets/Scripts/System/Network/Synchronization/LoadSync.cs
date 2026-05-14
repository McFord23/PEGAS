using Enums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class LoadSync : NetworkBehaviour
{
    [SerializeField] private GameObject loadScreen;
    [SerializeField] private bool loadFromStart = false;

    private void Awake()
    {
        if (Global.gameMode == GameMode.Host || Global.gameMode == GameMode.Client)
        {
            if (Global.fullParty && loadFromStart)
            {
                loadScreen.SetActive(true);
                NetworkManager.SceneManager.OnLoadEventCompleted += LoadCompleted;
            }
        }
    }

    private void LoadCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        if (clientscompleted.Count > 1)
        {
            loadScreen.SetActive(false);
            NetworkManager.SceneManager.OnLoadEventCompleted -= LoadCompleted;
        }
    }
}
