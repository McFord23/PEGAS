using Enums;
using UnityEngine;
using Unity.Netcode;

public class Shooting : NetworkBehaviour
{
    [SerializeField] private GameObject fireball;
    private int speed = 50;
    
    private Player player;
    private PlayerController controller;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        controller = player.GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (player.moveState is MoveState.Paused or MoveState.Stunned or MoveState.Dead or MoveState.Winner) return;

        if (controller.GetShootInput() > 0)
        {
            if (Global.gameMode is GameMode.Single or GameMode.LocalCoop) SpawnFireball();
            else RequestSpawnFireballServerRpc();
        }
    }

    private void SpawnFireball()
    {
        var selfTransform = transform;
        var newFireball = Instantiate(fireball, selfTransform.position, selfTransform.rotation);
        newFireball.GetComponent<Rigidbody2D>().AddForce(player.transform.right * (speed + player.speed), ForceMode2D.Impulse);
    }
    
    [ServerRpc]
    private void RequestSpawnFireballServerRpc()
    {
        var selfTransform = transform;
        var newFireball = Instantiate(fireball, selfTransform.position, selfTransform.rotation);
        newFireball.GetComponent<Rigidbody2D>().AddForce(player.transform.right * (speed + player.speed), ForceMode2D.Impulse);
        newFireball.name = $"Fireball {player.name}";
        newFireball.GetComponent<NetworkObject>().Spawn(true);
        
        RequestSpawnFireballClientRpc();
    }

    [ClientRpc]
    private void RequestSpawnFireballClientRpc()
    {
        //print("Rotation: " + player.transform.rotation.z);
        
        if (Global.gameMode != GameMode.Client) return;
        
        Rigidbody2D fireball = GameObject.Find("Fireball(Clone)").GetComponent<Rigidbody2D>();
        fireball.AddForce(player.transform.right * (speed + player.speed), ForceMode2D.Impulse);
        fireball.name = $"Fireball {player.name}";
    }
}
