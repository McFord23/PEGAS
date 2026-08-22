using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WindigoSpawner : SingletonNetworkBehaviour<WindigoSpawner>
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector3 spawnPos = new Vector3(25, 23, 0);

    private NetworkVariable<uint> id = new(
        0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner
    );

    protected override void Awake()
    {
        base.Awake();
        
        switch (Settings.GameMode)
        {
            case GameMode.Single:
            case GameMode.LocalCoop:
                Spawn();
                break;
            
            case GameMode.Host:
                NetworkManager.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
                break;
        }
    }

    private void OnLoadEventCompleted(string sceneName, LoadSceneMode loadsceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if ((Global.IsNetworkPlayerConnected && clientsCompleted.Count > 1) || !Global.IsNetworkPlayerConnected)
        {
            Spawn();
            NetworkManager.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
        }
    }

    public void Spawn()
    {
        switch (Settings.GameMode)
        {
            case GameMode.Single:
            case GameMode.LocalCoop:
                Windigo windigo;
                windigo = Instantiate(prefab, spawnPos, Quaternion.Euler(Vector3.zero)).GetComponent<Windigo>();
                WindigosManager.Instance.AddWindigo(windigo);
                break;
            
            case GameMode.Host:
                RequestSpawnWindigoServerRpc();
                break;
        }
    }

    [ServerRpc]
    private void RequestSpawnWindigoServerRpc()
    {
        Windigo windigo;
        windigo = Instantiate(prefab, spawnPos, Quaternion.Euler(Vector3.zero)).GetComponent<Windigo>();
        windigo.transform.name = $"Windigo {id.Value}";
        windigo.GetComponent<NetworkObject>().Spawn(true);
        WindigosManager.Instance.AddWindigo(windigo);
        
        RequestSpawnWindigoClientRpc();
        
        id.Value += 1;
    }

    [ClientRpc]
    private void RequestSpawnWindigoClientRpc()
    {
        if (Settings.GameMode != GameMode.Client) return;
        
        Windigo windigo;
        windigo = GameObject.Find("Windigo(Clone)").GetComponent<Windigo>();
        windigo.transform.name = $"Windigo {id.Value}";
        WindigosManager.Instance.AddWindigo(windigo);
    }
}
