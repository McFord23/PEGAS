using UnityEngine;

public class FlyingPlayerSpawner : PlayerSpawner
{
    [Header("Flying")]
    [SerializeField] private float crashSpeed = 80f;

    public override GameObject SpawnPlayer(PlayersSettings.Player player)
    {
        var playerObject = base.SpawnPlayer(player);
        playerObject.GetComponent<FlyingPlayer>().SetCrashSpeed(crashSpeed);
        return playerObject;
    }
}