using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WindigosManager : SingletonNetworkBehaviour<WindigosManager>
{
    private List<Windigo> windigoList = new List<Windigo>();

    public void AddWindigo(Windigo windigo)
    {
        windigoList.Add(windigo);
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

    [ServerRpc]
    private void RequestResetServerRpc()
    {
        Reset();
    }

    public void Pause()
    {
        foreach (Windigo windigo in windigoList)
        {
            windigo.Pause();
        }
    }

    public void Resume()
    {
        foreach (Windigo windigo in windigoList)
        {
            windigo.Resume();
        }
    }

    public void Remove(Windigo windigo)
    {
        windigoList.Remove(windigo);
    }
}
