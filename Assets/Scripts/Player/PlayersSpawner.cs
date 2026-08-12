using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayersSpawner : NetworkBehaviour
{
    [Header("Celestia")]
    [SerializeField] private GameObject celestiaPrefab;
    [SerializeField] private Transform celestiaSpawnPoint;
    
    [Header("Luna")]
    [SerializeField] private GameObject lunaPrefab;
    [SerializeField] private Transform lunaSpawnPoint;
    
    private void Awake()
    {
        switch (Settings.GameMode)
        {
            case GameMode.Single:
                SpawnPlayer(PlayersSettings.Player1);
                break;
            
            case GameMode.LocalCoop:
                SpawnPlayer(PlayersSettings.Player1);
                SpawnPlayer(PlayersSettings.Player2);
                break;
            
            case GameMode.Host:
                NetworkManager.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
                break;
        }
    }

    private void SceneManagerOnOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        if (!IsHost) return;
        
        var playerNum = 0;
        foreach (ulong clientId in clientscompleted)
        {
            var player = playerNum > 0 ? PlayersSettings.Player1 : PlayersSettings.Player2;
            SpawnPlayer(player).GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            playerNum++;
        }

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManagerOnOnLoadEventCompleted;
    }
    
    private GameObject SpawnPlayer(PlayersSettings.Player player)
    {
        var objectToSpawn = GetObjectToSpawn(player.Character);
        var spawnPosition = GetSpawnPosition(player.Character);
        var playerObject = Instantiate(objectToSpawn, spawnPosition, transform.rotation);
        playerObject.GetComponent<PlayerBase>().Initialize(player);
        return playerObject;
    }
    
    private GameObject GetObjectToSpawn(Character character)
    {
        return character switch
        {
            Character.Celestia => celestiaPrefab,
            Character.Luna => lunaPrefab,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private Vector2 GetSpawnPosition(Character character)
    {
        return character switch
        {
            Character.Celestia => celestiaSpawnPoint.position,
            Character.Luna => lunaSpawnPoint.position,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
