using Enums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject prefab;
    
    private void Awake()
    {
        if (Global.gameMode == GameMode.Single)
        {
            SpawnSingleItem();
            return;
        }

        if (NetworkManager.IsHost)
        {
            NetworkManager.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
        }
    }

    private void SpawnSingleItem()
    {
        var selfTransform = transform;
        Instantiate(prefab, selfTransform.position, selfTransform.rotation);
    }
    
    private void OnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        var selfTransform = transform;
        var instanceTransform = Instantiate(prefab, selfTransform.position, selfTransform.rotation);
        instanceTransform.GetComponent<NetworkObject>().Spawn(true);

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
    }
}