using System.Collections.Generic;
using UnityEngine.Events;
using Unity.Netcode;

public class WindigosManager : SingletonNetworkBehaviour<WindigosManager>
{
    public UnityEvent windigoDeathEvent;
    private List<Windigo> windigoList = new();

    public void AddWindigo(Windigo windigo)
    {
        windigoList.Add(windigo);
        windigo.deathEvent.AddListener(OnWindigoDeath);
    }
    
    public void Reset()
    {
        if (Settings.GameMode == GameMode.Client)
        {
            RequestResetServerRpc();
            return;
        }
        
        for (int i = 0; i < windigoList.Count; i++)
        {
            windigoList[i].Destroy(true);
        }

        WindigoSpawner.Instance.Spawn();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestResetServerRpc()
    {
        Reset();
    }

    public void Pause()
    {
        foreach (var windigo in windigoList)
        {
            windigo.Pause();
        }
    }

    public void Resume()
    {
        foreach (var windigo in windigoList)
        {
            windigo.Resume();
        }
    }

    public void Remove(Windigo windigo)
    {
        windigoList.Remove(windigo);
    }

    private void OnWindigoDeath()
    {
        windigoDeathEvent?.Invoke();
    }
}
