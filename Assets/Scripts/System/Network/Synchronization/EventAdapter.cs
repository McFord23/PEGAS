using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public enum EventKey
{
    Pause,
    Resume,
    Lose,
    Retry,
    Victory
}

public class EventAdapter : SingletonNetworkBehaviour<EventAdapter>
{
    public List<EventKey> keys;
    public List<UnityEvent> values;

    private Dictionary<EventKey, UnityEvent> eventItems = new();

    private void Start()
    {
        for (int i = 0; i != Mathf.Min(keys.Count, values.Count); i++)
            eventItems.Add(keys[i], values[i]);
    }

    public void Execute(EventKey eventKey)
    {
        switch (Settings.GameMode)
        {
            case GameMode.Single:
            case GameMode.LocalCoop:
                eventItems[eventKey].Invoke();
                break;
                
            case GameMode.Host:
            case GameMode.Client:
                RequestExecuteServerRpc(eventKey);
                break;
        }
    }

    [ServerRpc]
    private void RequestExecuteServerRpc(EventKey eventKey)
    {
        RequestExecuteClientRpc(eventKey);
    }

    [ClientRpc]
    private void RequestExecuteClientRpc(EventKey eventKey)
    {
        eventItems[eventKey].Invoke();
    }
}
