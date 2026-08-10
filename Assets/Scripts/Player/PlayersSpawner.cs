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
                SpawnPlayer(PlayersSettings.Player1.Character);
                break;
            
            case GameMode.LocalCoop:
                SpawnPlayer(PlayersSettings.Player1.Character);
                SpawnPlayer(PlayersSettings.Player2.Character);
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
            SpawnPlayer(player.Character).GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            playerNum++;
        }

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManagerOnOnLoadEventCompleted;
    }
    
    private GameObject SpawnPlayer(Character character)
    {
        var objectToSpawn = GetObjectToSpawn(character);
        var spawnPosition = GetSpawnPosition(character);
        return Instantiate(objectToSpawn, spawnPosition, transform.rotation);
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
