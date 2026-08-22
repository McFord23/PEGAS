using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;

public class NetworkSubmenu : NetworkBehaviour
{
    [SerializeField] private AddressFieldManager ipFieldManager;
    private PlayersMenu playersMenu;

    [SerializeField] private Text status;
    private Color brown;
    private readonly Color red = new (0.45f, 0.2f, 0.15f);
    private IEnumerator hideErrorConnection;

    [SerializeField] private Button createButton;
    [SerializeField] private Button connectButton;
    [SerializeField] private CloseCoopButton closeCoopButton;

    private HostMonitoring hostMonitoring;
    private ClientMonitoring clientMonitoring;

    public void Initialize(PlayersMenu menu)
    {
        playersMenu = menu;
        brown = status.color;
        hideErrorConnection = HideErrorConnection();
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
        hostMonitoring.OnClientConnectedEvent += OnClientConnected;
        hostMonitoring.OnClientDisconnectedEvent += OnClientDisconnected;
        hostMonitoring.OnShutdownEvent += OnHostShutdown;
    }

    private void OnHostStartCreating()
    {
        ShowStatus("creating...");
        ipFieldManager.Block(true);

        createButton.interactable = false;
        connectButton.interactable = false;
        closeCoopButton.ChangeMode(CloseCoopButton.Mode.ShutDown);
    }

    private void OnHostCreatingFailure(string log)
    {
        ShowError(log);
        createButton.interactable = true;
        connectButton.interactable = true;

        HostUnsubscribe();
        closeCoopButton.ChangeMode(CloseCoopButton.Mode.Back);
    }

    private void OnHostCreated()
    {
        ShowStatus("no player");

        createButton.interactable = true;
        connectButton.interactable = true;
        createButton.gameObject.SetActive(false);
        connectButton.gameObject.SetActive(false);
    }

    private void OnHostShutdown()
    {
        HostUnsubscribe();
        status.gameObject.SetActive(false);

        ipFieldManager.Block(false);
        
        createButton.gameObject.SetActive(true);
        connectButton.gameObject.SetActive(true);
        closeCoopButton.ChangeMode(CloseCoopButton.Mode.Back);
    }

    private void HostUnsubscribe()
    {
        hostMonitoring.OnStartCreationEvent -= OnHostStartCreating;
        hostMonitoring.OnCreatingFailureEvent -= OnHostCreatingFailure;
        hostMonitoring.OnCreatedEvent -= OnHostCreated;
        hostMonitoring.OnClientConnectedEvent -= OnClientConnected;
        hostMonitoring.OnClientDisconnectedEvent -= OnClientDisconnected;
        hostMonitoring.OnShutdownEvent -= OnHostShutdown;
    }

    public void ClientSubscribe()
    {
        clientMonitoring.OnConnectionStartEvent += OnClientStartConnecting;
        clientMonitoring.OnConnectionFailureEvent += OnClientFailureConnecting;
        clientMonitoring.OnConnectedEvent += OnClientConnected;
        clientMonitoring.OnDisconnectedEvent += OnClientDisconnected;
    }

    private void OnClientStartConnecting()
    {
        ShowStatus("connecting...");

        ipFieldManager.Block(true);

        createButton.gameObject.SetActive(false);
        connectButton.gameObject.SetActive(false);
        closeCoopButton.ChangeMode(CloseCoopButton.Mode.Cancel);
    }

    private void OnClientFailureConnecting()
    {
        ShowError("connection error");

        ipFieldManager.Block(false);
        
        createButton.gameObject.SetActive(true);
        connectButton.gameObject.SetActive(true);
        closeCoopButton.ChangeMode(CloseCoopButton.Mode.Back);

        ClientUnsubscribe();
    }

    private void OnClientConnected()
    {
        HideStatus();
        gameObject.SetActive(false);
        playersMenu.ShowPlayer2Submenu();
    }

    private void OnClientDisconnected()
    {
        if (Settings.GameMode == GameMode.Host)
        {
            ShowStatus("no player");
        }
        else
        {
            HideStatus();

            ipFieldManager.Block(false);

            closeCoopButton.ChangeMode(CloseCoopButton.Mode.Back);
            createButton.gameObject.SetActive(true);
            connectButton.gameObject.SetActive(true);

            ClientUnsubscribe();
        }

        playersMenu.HidePlayer2Submenu();
        gameObject.SetActive(true);
    }

    private void ClientUnsubscribe()
    {
        clientMonitoring.OnConnectionStartEvent -= OnClientStartConnecting;
        clientMonitoring.OnConnectionFailureEvent -= OnClientFailureConnecting;
        clientMonitoring.OnConnectedEvent -= OnClientConnected;
        clientMonitoring.OnDisconnectedEvent -= OnClientDisconnected;
    }

    private void ShowStatus(string text)
    {
        StopCoroutine(hideErrorConnection);

        status.text = text;
        status.color = brown;
        status.gameObject.SetActive(true);
    }

    private void ShowError(string error)
    {
        StopCoroutine(hideErrorConnection);

        status.text = error;
        status.color = red;
        status.gameObject.SetActive(true);

        StartCoroutine(hideErrorConnection);
    }

    public void ShowError(string error, bool blockButton)
    {
        StopCoroutine(hideErrorConnection);

        status.text = error;
        status.color = red;
        status.gameObject.SetActive(true);

        StartCoroutine(hideErrorConnection);

        if (blockButton)
        {
            createButton.interactable = false;
            connectButton.interactable = false;
        }
    }

    public void HideStatus()
    {
        StopCoroutine(hideErrorConnection);

        status.gameObject.SetActive(false);
        createButton.interactable = true;
        connectButton.interactable = true;
    }

    private IEnumerator HideErrorConnection()
    {
        var color = status.color;

        yield return new WaitForSeconds(3);

        while (color.a > 0)
        {
            color.a -= 0.001f;
            status.color = color;
            yield return null;
        }
    }
}
