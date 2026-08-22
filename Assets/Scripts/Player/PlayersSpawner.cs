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

    private PlayersManager playersManager;
    
    private void Awake()
    {
        playersManager = GetComponent<PlayersManager>();
        Spawn();
    }
    
    public void Spawn()
    {
        switch (Settings.GameMode)
        {
            case GameMode.Single:
                if (playersManager.players[0] == null) SpawnPlayer(PlayersSettings.Player1);
                break;
            
            case GameMode.LocalCoop:
                if (playersManager.players[0] == null) SpawnPlayer(PlayersSettings.Player1);
                if (playersManager.players[1] == null) SpawnPlayer(PlayersSettings.Player2);
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
            
            if (playersManager.players[playerNum] == null)
            {
                SpawnPlayer(player).GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            }
            
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
    
    private GameObject GetObjectToSpawn(PlayerCharacter character)
    {
        return character switch
        {
            PlayerCharacter.Celestia => celestiaPrefab,
            PlayerCharacter.Luna => lunaPrefab,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private Vector2 GetSpawnPosition(PlayerCharacter character)
    {
        return character switch
        {
            PlayerCharacter.Celestia => celestiaSpawnPoint.position,
            PlayerCharacter.Luna => lunaSpawnPoint.position,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
