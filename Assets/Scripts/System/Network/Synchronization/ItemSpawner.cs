using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject prefab;
    
    private void Awake()
    {
        if (Settings.GameMode is GameMode.Single or GameMode.LocalCoop)
        {
            SpawnItem();
            return;
        }

        if (NetworkManager.IsHost)
        {
            NetworkManager.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
        }
    }

    private void SpawnItem()
    {
        Instantiate(prefab, transform.position, transform.rotation);
    }
    
    private void OnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        var instanceTransform = Instantiate(prefab, transform.position, transform.rotation);
        instanceTransform.GetComponent<NetworkObject>().Spawn(true);

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
    }
}