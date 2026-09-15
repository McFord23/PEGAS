using UnityEngine;

public class WallToWallPlayerSpawner : PlayerSpawner
{
    [Header("Camera")]
    [SerializeField] private GameObject strategicCamera;

    public override GameObject SpawnPlayer(PlayersSettings.Player player)
    {
        var playerObject = base.SpawnPlayer(player);
        playerObject.GetComponent<WallToWallPlayer>().SetStrategicCamera(strategicCamera);
        return playerObject;
    }
}