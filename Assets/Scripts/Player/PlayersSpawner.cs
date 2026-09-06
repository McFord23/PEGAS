using System;
using UnityEngine;

public class PlayersSpawner : MonoBehaviour
{
    [Header("Celestia")]
    [SerializeField] private GameObject celestiaPrefab;
    [SerializeField] private Transform celestiaSpawnPoint;
    
    [Header("Luna")]
    [SerializeField] private GameObject lunaPrefab;
    [SerializeField] private Transform lunaSpawnPoint;
    
    public GameObject SpawnPlayer(PlayersSettings.Player player)
    {
        var objectToSpawn = GetObjectToSpawn(player.Character);
        var spawnPosition = GetSpawnPosition(player.Character);
        return Instantiate(objectToSpawn, spawnPosition, transform.rotation);
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
