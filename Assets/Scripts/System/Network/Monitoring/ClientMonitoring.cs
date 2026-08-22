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

        Settings.GameMode = GameMode.Client;
        Global.IsNetworkPlayerConnected = true;
        OnConnectedEvent?.Invoke();
    }

    public void Disconnect()
    {
        StopClient();
    }

    private void StopClient(ulong id = 1)
    {
        if (Settings.GameMode != GameMode.Client) return;

        NetworkManager.OnClientConnectedCallback -= OnConnected;
        NetworkManager.OnClientDisconnectCallback -= StopClient;
        StopCoroutine(waitingConnection);
        NetworkManager.Shutdown();

        Settings.GameMode = GameMode.Single;
        Global.IsNetworkPlayerConnected = false;
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
