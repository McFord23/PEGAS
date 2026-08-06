using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Абстрагирует мультиплеерный и одиночный переход между сценами 
 */
public class SceneManagerAdapter : SingletonNetworkBehaviour<SceneManagerAdapter>
{
    [SerializeField] private GameObject loadScreen;

    /*private void Start()
    {
        ClientMonitoring.Instance.OnConnectedEvent += LoadHostScene;
    }

    private void LoadHostScene()
    {
        if (SceneManager.GetActiveScene().name != NetworkManager.SceneManager.)
        {
            if (loadScreen) loadScreen.SetActive(true);
        }
    }*/
    
    public void LoadScene(string sceneName)
    {
        switch (Settings.GameMode)
        {
            case GameMode.Single:
            case GameMode.LocalCoop:
                SceneManager.LoadScene(sceneName);
                break;

            case GameMode.Host:
            case GameMode.Client:
                if (loadScreen) loadScreen.SetActive(true);
                RequestLoadSceneServerRpc(new FixedString32Bytes(sceneName));
                break;
        }
    }

    [ServerRpc]
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
            NetworkManager.SceneManager.LoadScene(sceneName.ToString(), LoadSceneMode.Single);
        }
    }
}