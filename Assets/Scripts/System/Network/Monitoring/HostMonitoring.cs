using System.Text;
using System.Collections;
using Unity.Netcode;
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

    public delegate void ClientConnectionStartEvent();
    public event ClientConnectionStartEvent OnClientConnectionStartEvent;
    
    public delegate void ClientConnectionFailedEvent();
    public event ClientConnectionFailedEvent OnClientConnectionFailedEvent;

    public delegate void ClientConnectedEvent();
    public event ClientConnectedEvent OnClientConnectedEvent;

    public delegate void ClientDisconnectedEvent();
    public event ClientDisconnectedEvent OnClientDisconnectedEvent;
    
    public delegate void ShutdownHostEvent();
    public event ShutdownHostEvent OnShutdownEvent;

    private string sessionPassword;
    private Coroutine waitConnection;

    private enum DisconnectReason
    {
        FullSession,
        PasswordWrong
    }

    private void Start()
    {
        NetworkManager.ConnectionApprovalCallback = ApprovalCheck;
    }
    
    public void SetPassword(string newPassword)
    {
        sessionPassword = newPassword;
    }
    
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

        Settings.ChangeGameMode(GameMode.Host);
        Application.logMessageReceived -= CheckCreatingFailure;
        
        NetworkManager.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnect;
        OnCreatedEvent?.Invoke();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (request.ClientNetworkId == NetworkManager.ServerClientId)
        {
            response.Reason = "";
            response.Approved = true;
            return;
        }

        if (NetworkManager.ConnectedClients.Count > 1)
        {
            response.Reason = DisconnectReason.FullSession.ToString();
            response.Approved = false;
            return;
        }

        var clientData = request.Payload;
        var clientPassword = Encoding.UTF8.GetString(clientData);
        
        if (clientPassword != sessionPassword)
        {
            response.Reason = DisconnectReason.PasswordWrong.ToString();
            response.Approved = false;
            return;
        }
        
        response.Reason = "";
        response.Approved = true;
        OnClientConnectionStartEvent?.Invoke();
        waitConnection = StartCoroutine(WaitingConnection());
    }
    
    private void OnClientConnected(ulong id = 1)
    {
        if (id != NetworkManager.LocalClientId)
        {
            if (waitConnection != null)
            {
                StopCoroutine(waitConnection);
                waitConnection = null;
            }
            
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
        if (waitConnection != null)
        {
            StopCoroutine(waitConnection);
            waitConnection = null;
        }
        
        NetworkManager.OnServerStarted -= OnServerUp;
        NetworkManager.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        NetworkManager.Shutdown();

        Settings.ChangeGameMode(GameMode.Single);
        Global.IsNetworkPlayerConnected = false;
        OnShutdownEvent?.Invoke();
    }
    
    private IEnumerator WaitingConnection()
    {
        yield return new WaitForSeconds(Settings.NETWORK_CONNECTING_TIMER);
        
        if (NetworkManager.ConnectedClients.Count < 2)
        {
            OnClientConnectionFailedEvent?.Invoke();
        }

        waitConnection = null;
    }
}
