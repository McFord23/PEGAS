using Enums;
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

        Global.gameMode = GameMode.Host;
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
            Global.fullParty = true;
            OnClientConnectedEvent?.Invoke();
        }
    }
    
    public void DisconnectClient()
    {
        NetworkManager.DisconnectClient(clientID);
        OnClientDisconnect();
    }

    private void OnClientDisconnect(ulong id = 1)
    {
        Global.fullParty = false;
        OnClientDisconnectedEvent?.Invoke();
    }

    public void ShutdownHost()
    {
        NetworkManager.OnServerStarted -= OnServerUp;
        NetworkManager.OnClientDisconnectCallback -= OnClientConnect;
        NetworkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        NetworkManager.Shutdown();

        Global.gameMode = GameMode.Single;
        Global.fullParty = false;
        OnShutdownEvent?.Invoke();
    }
}
