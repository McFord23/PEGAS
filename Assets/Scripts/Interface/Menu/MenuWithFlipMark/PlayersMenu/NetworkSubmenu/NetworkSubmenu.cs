using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkSubmenu : NetworkBehaviour
{
    [SerializeField] private AddressFieldManager ipFieldManager;
    [SerializeField] private PasswordField passwordField;
    [SerializeField] private CoopStatus coopStatus;
    [SerializeField] private Button createButton;
    [SerializeField] private Button connectButton;
    [SerializeField] private CloseCoopButton closeCoopButton;

    private PlayersMenu playersMenu;
    
    private HostMonitoring hostMonitoring;
    private ClientMonitoring clientMonitoring;

    public void Initialize(PlayersMenu menu)
    {
        playersMenu = menu;
        
        hostMonitoring = HostMonitoring.Instance;
        clientMonitoring = ClientMonitoring.Instance;

        switch (Settings.GameMode)
        {
            case GameMode.Client:
                OnClientConnected();
                break;

            case GameMode.Host:
                if (Global.IsNetworkPlayerConnected)
                {
                    OnClientConnected();
                }
                else OnHostCreated();
                break;
        }
    }

    public void HostSubscribe()
    {
        hostMonitoring.OnStartCreationEvent += OnHostStartCreating;
        hostMonitoring.OnCreatingFailureEvent += OnHostCreatingFailure;
        hostMonitoring.OnCreatedEvent += OnHostCreated;
        hostMonitoring.OnClientConnectionStartEvent += OnClientStartConnecting;
        hostMonitoring.OnClientConnectionFailedEvent += OnClientFailedConnecting;
        hostMonitoring.OnClientConnectedEvent += OnClientConnected;
        hostMonitoring.OnClientDisconnectedEvent += OnClientDisconnected;
        hostMonitoring.OnShutdownEvent += OnHostShutdown;
    }

    private void OnHostStartCreating()
    {
        SetButtonsActive(false);
        closeCoopButton.ChangeMode(CloseCoopButton.Mode.ShutDown);
    }

    private void OnHostCreatingFailure(string log)
    {
        HostUnsubscribe();
        
        coopStatus.SetMode(CoopStatus.Mode.CreationError);
        
        SetButtonsActive(true);
        closeCoopButton.ChangeMode(CloseCoopButton.Mode.Back);
    }

    private void OnHostCreated()
    {
        coopStatus.SetMode(CoopStatus.Mode.Created);
    }

    private void OnHostShutdown()
    {
        HostUnsubscribe();
        
        coopStatus.SetMode(CoopStatus.Mode.None);
        
        SetButtonsActive(true);
        closeCoopButton.ChangeMode(CloseCoopButton.Mode.Back);
    }

    private void HostUnsubscribe()
    {
        hostMonitoring.OnStartCreationEvent -= OnHostStartCreating;
        hostMonitoring.OnCreatingFailureEvent -= OnHostCreatingFailure;
        hostMonitoring.OnCreatedEvent -= OnHostCreated;
        hostMonitoring.OnClientConnectionStartEvent -= OnClientStartConnecting;
        hostMonitoring.OnClientConnectionFailedEvent -= OnClientFailedConnecting;
        hostMonitoring.OnClientConnectedEvent -= OnClientConnected;
        hostMonitoring.OnClientDisconnectedEvent -= OnClientDisconnected;
        hostMonitoring.OnShutdownEvent -= OnHostShutdown;
    }

    public void ClientSubscribe()
    {
        clientMonitoring.OnConnectionStartEvent += OnClientStartConnecting;
        clientMonitoring.OnConnectionFailedEvent += OnClientFailedConnecting;
        clientMonitoring.OnConnectedEvent += OnClientConnected;
        clientMonitoring.OnDisconnectedEvent += OnClientDisconnected;
    }

    private void OnClientStartConnecting()
    {
        if (Settings.GameMode == GameMode.Host)
        {
            coopStatus.SetMode(CoopStatus.Mode.PlayerConnecting);
        }
        else
        {
            coopStatus.SetMode(CoopStatus.Mode.Connecting);
        
            SetButtonsActive(false);
            closeCoopButton.ChangeMode(CloseCoopButton.Mode.Cancel);
        }
    }

    private void OnClientFailedConnecting()
    {
        var reason = NetworkManager.DisconnectReason;
        
        if (Settings.GameMode is GameMode.Host)
        {
            if (string.IsNullOrEmpty(reason))
            {
                coopStatus.SetMode(CoopStatus.Mode.ConnectionError);
            }
        }
        else
        {
            var mode = Enum.TryParse<CoopStatus.Mode>(reason, out var result)
                ? result
                : CoopStatus.Mode.ConnectionError;
            
            coopStatus.SetMode(mode);
        
            SetButtonsActive(true);
            closeCoopButton.ChangeMode(CloseCoopButton.Mode.Back);

            ClientUnsubscribe();
        }
    }

    private void OnClientConnected()
    {
        gameObject.SetActive(false);
        playersMenu.ShowPlayer2Submenu();
    }

    private void OnClientDisconnected()
    {
        if (Settings.GameMode is GameMode.Host)
        {
            coopStatus.SetMode(CoopStatus.Mode.Created);
        }
        else
        {
            coopStatus.SetMode(CoopStatus.Mode.None);

            SetButtonsActive(true);
            closeCoopButton.ChangeMode(CloseCoopButton.Mode.Back);
            ClientUnsubscribe();
        }

        playersMenu.HidePlayer2Submenu();
        gameObject.SetActive(true);
    }

    private void ClientUnsubscribe()
    {
        clientMonitoring.OnConnectionStartEvent -= OnClientStartConnecting;
        clientMonitoring.OnConnectionFailedEvent -= OnClientFailedConnecting;
        clientMonitoring.OnConnectedEvent -= OnClientConnected;
        clientMonitoring.OnDisconnectedEvent -= OnClientDisconnected;
    }

    public void InvalidIP()
    {
        coopStatus.SetMode(CoopStatus.Mode.InvalidIP);
        createButton.interactable = false;
        connectButton.interactable = false;
    }

    public void ValidIP()
    {
        coopStatus.SetMode(CoopStatus.Mode.None);
        createButton.interactable = true;
        connectButton.interactable = true;
    }
    
    private void SetButtonsActive(bool value)
    {
        createButton.gameObject.SetActive(value);
        connectButton.gameObject.SetActive(value);
        ipFieldManager.Block(!value);
        passwordField.Block(!value);
    }
}
