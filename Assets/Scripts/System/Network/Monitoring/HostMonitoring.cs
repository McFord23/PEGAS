using UnityEngine;

public class HostMonitoring : SingletonNetworkBehaviour<HostMonitoring>
{
    private ulong clientID;

    public delegate void StartCreationEvent();
    public event StartCreationEvent OnStartCreationEvent;
    
    public delegate void CreatingFailureEvent(string value);
    public event CreatingFailureEvent OnCreatingFailureEvent;

    public delegate void CreatedEvent();
    public event CreatedEvent OnCreatedEvent;

    public delegate void ClientConnectedEvent();
    public event ClientConnectedEvent OnClientConnectedEvent;

    public delegate void ClientDisconnectedEvent();
    public event ClientDisconnectedEvent OnClientDisconnectedEvent;
    
    public delegate void ShutdownHostEvent();
    public event ShutdownHostEvent OnShutdownEvent;

    public void CreateHost()
    {
        OnStartCreationEvent?.Invoke();

        NetworkManager.OnServerStarted += OnServerUp;
        Application.logMessageReceived += CheckCreatingFailure;

        NetworkManager.StartHost();
    }

    private void CheckCreatingFailure(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error && stackTrace.Contains("Netcode"))
        {
            Application.logMessageReceived -= CheckCreatingFailure;
            OnCreatingFailureEvent?.Invoke(logString);
        }
    }

    private void OnServerUp()
    {
        if (!NetworkManager.IsHost) return;

        Settings.GameMode = GameMode.Host;
        Application.logMessageReceived -= CheckCreatingFailure;
        NetworkManager.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnect;
        OnCreatedEvent?.Invoke();
    }

    private void OnClientConnect(ulong id = 1)
    {
        if (id != NetworkManager.LocalClientId)
        {
            clientID = id;
            Global.IsNetworkPlayerConnected = true;
            OnClientConnectedEvent?.Invoke();
        }
    }
    
    public void DisconnectClient()
    {
        if (clientID != NetworkManager.LocalClientId)
        {
            NetworkManager.DisconnectClient(clientID);
            OnClientDisconnect();
        }
    }

    private void OnClientDisconnect(ulong id = 1)
    {
        Global.IsNetworkPlayerConnected = false;
        OnClientDisconnectedEvent?.Invoke();
    }

    public void ShutdownHost()
    {
        NetworkManager.OnServerStarted -= OnServerUp;
        NetworkManager.OnClientDisconnectCallback -= OnClientConnect;
        NetworkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        NetworkManager.Shutdown();

        Settings.GameMode = GameMode.Single;
        Global.IsNetworkPlayerConnected = false;
        OnShutdownEvent?.Invoke();
    }
}
