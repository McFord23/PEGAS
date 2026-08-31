using System.Collections;
using System.Text;
using UnityEngine;

public class ClientMonitoring : SingletonNetworkBehaviour<ClientMonitoring>
{
    private IEnumerator waitingConnection;

    public delegate void ConnectionStartEvent();
    public event ConnectionStartEvent OnConnectionStartEvent;

    public delegate void ConnectionFailedEvent();
    public event ConnectionFailedEvent OnConnectionFailedEvent;

    public delegate void ConnectedEvent();
    public event ConnectedEvent OnConnectedEvent;
    
    public delegate void DisconnectedEvent();
    public event DisconnectedEvent OnDisconnectedEvent;

    private byte[] password;

    public void SetPassword(string newPassword)
    {
       password = Encoding.UTF8.GetBytes(newPassword);
    }
    
    public void StartConnection()
    {
        NetworkManager.NetworkConfig.ConnectionData = password;
        NetworkManager.StartClient();
        OnConnectionStartEvent?.Invoke();

        NetworkManager.OnClientConnectedCallback += OnConnected;
        NetworkManager.OnClientDisconnectCallback += OnFailedConnection;

        waitingConnection = WaitingConnection();
        StartCoroutine(waitingConnection);
    }

    public void CancelConnection()
    {
        NetworkManager.OnClientConnectedCallback -= OnConnected;
        NetworkManager.OnClientDisconnectCallback -= OnFailedConnection;
        StopCoroutine(waitingConnection);
        NetworkManager.Shutdown();

        OnDisconnectedEvent?.Invoke();
    }

    private void OnConnected(ulong id)
    {
        if (NetworkManager.IsHost) return;

        StopCoroutine(waitingConnection);
        NetworkManager.OnClientDisconnectCallback -= OnFailedConnection;
        NetworkManager.OnClientDisconnectCallback += StopClient;

        Settings.ChangeGameMode(GameMode.Client);
        Global.IsNetworkPlayerConnected = true;
        OnConnectedEvent?.Invoke();
    }

    public void Disconnect()
    {
        StopClient();
    }

    private void OnFailedConnection(ulong id)
    {
        if (waitingConnection != null)
        {
            StopCoroutine(waitingConnection);
            waitingConnection = null;
        }
        
        OnConnectionFailedEvent?.Invoke();
    }

    private void StopClient(ulong id = 1)
    {
        if (Settings.GameMode != GameMode.Client) return;

        NetworkManager.OnClientConnectedCallback -= OnConnected;
        NetworkManager.OnClientDisconnectCallback -= StopClient;
        StopCoroutine(waitingConnection);
        NetworkManager.Shutdown();

        Settings.ChangeGameMode(GameMode.Single);
        Global.IsNetworkPlayerConnected = false;
        OnDisconnectedEvent?.Invoke();
    }
    
    private IEnumerator WaitingConnection()
    {
        yield return new WaitForSeconds(Settings.NETWORK_CONNECTING_TIMER);

        waitingConnection = null;
        
        if (!NetworkManager.IsConnectedClient)
        {
            NetworkManager.Shutdown();
            OnConnectionFailedEvent?.Invoke();
        }
    }
}
