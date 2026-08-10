using UnityEngine;
using Unity.Netcode;

public class Shooting : NetworkBehaviour
{
    [SerializeField] private GameObject fireball;
    [SerializeField] private int speed = 50;
    
    private PlayerBase player;

    private void Start()
    {
        player = GetComponentInParent<PlayerBase>();
    }

    private void FixedUpdate()
    {
        if (!player.IsInputAvailable()) return;
        if (player.AdditionalActionInput == 0) return;
        
        if (Settings.GameMode is GameMode.Single or GameMode.LocalCoop) SpawnFireball();
        else RequestSpawnFireballServerRpc();
    }

    private void SpawnFireball()
    {
        var selfTransform = transform;
        var newFireball = Instantiate(fireball, selfTransform.position, selfTransform.rotation);
        newFireball.GetComponent<Rigidbody2D>().AddForce(player.transform.right * (speed + player.Speed), ForceMode2D.Impulse);
    }
    
    [ServerRpc]
    private void RequestSpawnFireballServerRpc()
    {
        var selfTransform = transform;
        var newFireball = Instantiate(fireball, selfTransform.position, selfTransform.rotation);
        newFireball.GetComponent<Rigidbody2D>().AddForce(player.transform.right * (speed + player.Speed), ForceMode2D.Impulse);
        newFireball.name = $"Fireball {player.name}";
        newFireball.GetComponent<NetworkObject>().Spawn(true);
        
        RequestSpawnFireballClientRpc();
    }

    [ClientRpc]
    private void RequestSpawnFireballClientRpc()
    {
        if (Settings.GameMode != GameMode.Client) return;
        
        Rigidbody2D fireball = GameObject.Find("Fireball(Clone)").GetComponent<Rigidbody2D>();
        fireball.AddForce(player.transform.right * (speed + player.Speed), ForceMode2D.Impulse);
        fireball.name = $"Fireball {player.name}";
    }
}
