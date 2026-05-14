using Enums;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;

public class NetworkSubmenu : NetworkBehaviour
{
    private AddressFieldManager ipFieldManager;
    private PlayersMenu playersMenu;

    private Text status;
    private Color brown;
    private Color red;
    private IEnumerator hideErrorConnection;

    private Button createButton;
    private Button connectButton;
    private GameObject shutdownButton;
    private GameObject cancelButton;

    private HostMonitoring hostMonitoring;
    private ClientMonitoring clientMonitoring;

    public void Initialize()
    {
        ipFieldManager = GetComponentInChildren<AddressFieldManager>();
        playersMenu = GetComponentInParent<PlayersMenu>(true);

        status = transform.Find("Status").GetComponent<Text>();
        brown = status.color;
        red = new Color(0.45f, 0.2f, 0.15f);
        hideErrorConnection = HideErrorConnection();

        createButton = transform.Find("Create").GetComponent<Button>();
        connectButton = transform.Find("Connect").GetComponent<Button>();
        shutdownButton = transform.Find("Shutdown").gameObject;
        cancelButton = transform.Find("Cancel").gameObject;

        hostMonitoring = HostMonitoring.Instance;
        clientMonitoring = ClientMonitoring.Instance;

        switch (Global.gameMode)
        {
            case GameMode.Client:
                playersMenu.UpdateBackButtons(true);
                OnClientConnected();
                break;

            case GameMode.Host:
                if (Global.fullParty)
                {
                    playersMenu.UpdateBackButtons(true);
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

    public void OnHostStartCreating()
    {
        ShowStatus("creating...");
        ipFieldManager.Block(true);

        createButton.interactable = false;
        connectButton.interactable = false;
    }

    private void OnHostCreatingFailure(string log)
    {
        ShowError(log);
        createButton.interactable = true;
        connectButton.interactable = true;

        HostUnsubscribe();
    }

    private void OnHostCreated()
    {
        ShowStatus("no player");

        createButton.interactable = true;
        connectButton.interactable = true;
        createButton.gameObject.SetActive(false);
        connectButton.gameObject.SetActive(false);

        shutdownButton.SetActive(true);
        playersMenu.UpdateBackButtons(true);
    }

    private void OnHostShutdown()
    {
        HostUnsubscribe();
        status.gameObject.SetActive(false);

        ipFieldManager.Block(false);

        shutdownButton.SetActive(false);
        cancelButton.SetActive(false);
        createButton.gameObject.SetActive(true);
        connectButton.gameObject.SetActive(true);

        playersMenu.UpdateBackButtons(false);
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

    public void OnClientStartConnecting()
    {
        ShowStatus("connecting...");

        ipFieldManager.Block(true);

        createButton.gameObject.SetActive(false);
        connectButton.gameObject.SetActive(false);
        cancelButton.SetActive(true);

        playersMenu.UpdateBackButtons(true);
    }

    private void OnClientFailureConnecting()
    {
        ShowError("connection error");

        ipFieldManager.Block(false);

        cancelButton.SetActive(false);
        createButton.gameObject.SetActive(true);
        connectButton.gameObject.SetActive(true);

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
        if (Global.gameMode == GameMode.Host)
        {
            ShowStatus("no player");
        }
        else
        {
            HideStatus();

            ipFieldManager.Block(false);

            cancelButton.SetActive(false);
            createButton.gameObject.SetActive(true);
            connectButton.gameObject.SetActive(true);

            playersMenu.UpdateBackButtons(false);

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
    
    public void ShowError(string error)
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
        Color color = status.color;

        yield return new WaitForSeconds(3);

        while (color.a > 0)
        {
            color.a -= 0.001f;
            status.color = color;
            yield return null;
        }
    }
}
