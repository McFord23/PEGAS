using UnityEngine;

public class WallToWallPlayerSpawner : PlayerSpawner
{
    [Header("Wall To Wall")] 
    [SerializeField] private float height = 1;
    [SerializeField] private GameObject strategicCamera;

    public override GameObject SpawnPlayer(PlayersSettings.Player player)
    {
        var playerObject = base.SpawnPlayer(player);
        playerObject.GetComponent<WallToWallPlayer>().SetStrategicCamera(strategicCamera);
        return playerObject;
    }

    protected override Vector3 GetSpawnPosition(PlayerCharacter character)
    {
        Transform spawnPoint;
        
        if (character is PlayerCharacter.Celestia)
        {
            spawnPoint = IsPlayerOnPoint(celestiaSpawnPoint) ? lunaSpawnPoint : celestiaSpawnPoint;
        }
        else
        {
            spawnPoint = IsPlayerOnPoint(lunaSpawnPoint) ? celestiaSpawnPoint : lunaSpawnPoint;
        }
        
        return spawnPoint.position;
    }

    private bool IsPlayerOnPoint(Transform point)
    {
        var origin = point.position + Vector3.up * height;
        return Physics.Raycast(origin, Vector3.down, height, LayerMask.GetMask("Player"));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(celestiaSpawnPoint.position + Vector3.up * height, Vector3.down * height);
        Gizmos.DrawRay(lunaSpawnPoint.position + Vector3.up * height, Vector3.down * height);
    }
}