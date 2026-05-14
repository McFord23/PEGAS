using System;
using System.Collections.Generic;
using Enums;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Спавнит игроков в режиме мультиплеера и одного игрока в одиночном режиме
 */
public class PlayersSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject celestiaPrefab;
    [SerializeField] private GameObject lunaPrefab;
    
    private void Awake()
    {
        if (Global.gameMode == GameMode.Single)
        {
            SpawnSinglePlayer();
            return;
        }

        if (NetworkManager.IsHost)
        {
            NetworkManager.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
        }
    }

    private void SceneManagerOnOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        var playerNum = 0;
        foreach (ulong clientId in clientscompleted)
        {
            SpawnCharacter(playerNum, clientId);
            playerNum++;
        }

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManagerOnOnLoadEventCompleted;
    }

    private void SpawnSinglePlayer()
    {
        var objectToSpawn = GetObjectToSpawn(Global.players[0].character);
        var selfTransform = transform;
        Instantiate(objectToSpawn, selfTransform.position, selfTransform.rotation);
    }
    
    private void SpawnCharacter(int playerNum, ulong clientId)
    {
        if (!IsHost) return;

        var objectToSpawn = GetObjectToSpawn(Global.players[playerNum].character);
        var selfTransform = transform;
        var instanceTransform = Instantiate(objectToSpawn, selfTransform.position, selfTransform.rotation);

        instanceTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
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
}
