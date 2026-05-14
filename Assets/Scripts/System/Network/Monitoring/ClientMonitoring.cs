using Enums;
using System.Collections;
using UnityEngine;

public class ClientMonitoring : SingletonNetworkBehaviour<ClientMonitoring>
{
    private IEnumerator waitingConnection;

    public delegate void ConnectionStartEvent();
    public event ConnectionStartEvent OnConnectionStartEvent;

    public delegate void ConnectionFailureEvent();
    public event ConnectionFailureEvent OnConnectionFailureEvent;

    public delegate void ConnectedEvent();
    public event ConnectedEvent OnConnectedEvent;
    
    public delegate void DisconnectedEvent();
    public event DisconnectedEvent OnDisconnectedEvent;

    public void StartConnection()
    {
        NetworkManager.StartClient();
        OnConnectionStartEvent?.Invoke();

        NetworkManager.OnClientConnectedCallback += OnConnected;

        waitingConnection = WaitingConnection();
        StartCoroutine(waitingConnection);
    }

    public void CancelConnection()
    {
        NetworkManager.OnClientConnectedCallback -= OnConnected;
        StopCoroutine(waitingConnection);
        NetworkManager.Shutdown();

        OnDisconnectedEvent?.Invoke();
    }

    private void OnConnected(ulong id)
    {
        if (NetworkManager.IsHost) return;

        StopCoroutine(waitingConnection);
        NetworkManager.OnClientDisconnectCallback += StopClient;

        Global.gameMode = GameMode.Client;
        Global.fullParty = true;
        OnConnectedEvent?.Invoke();
    }

    public void Disconnect()
    {
        StopClient();
    }

    private void StopClient(ulong id = 1)
    {
        if (Global.gameMode != GameMode.Client) return;

        NetworkManager.OnClientConnectedCallback -= OnConnected;
        NetworkManager.OnClientDisconnectCallback -= StopClient;
        StopCoroutine(waitingConnection);
        NetworkManager.Shutdown();

        Global.gameMode = GameMode.Single;
        Global.fullParty = false;
        OnDisconnectedEvent?.Invoke();
    }
    
    private IEnumerator WaitingConnection()
    {
        yield return new WaitForSeconds(15);

        if (!NetworkManager.IsConnectedClient)
        {
            NetworkManager.Shutdown();
            OnConnectionFailureEvent?.Invoke();
        }
    }
}
