using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class ControlSchemeSync : NetworkBehaviour
{
    [SerializeField] private PlayersMenu playersMenu;

    [SerializeField] private List<Button> buttons = new ();

    private HostMonitoring host;
    private ClientMonitoring client;

    private readonly NetworkVariable<ControlScheme> hostScheme = new (ControlScheme.Mouse, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    private readonly NetworkVariable<ControlScheme> clientScheme = new (ControlScheme.WASD, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        host = HostMonitoring.Instance;
        client = ClientMonitoring.Instance;

        host.OnCreatedEvent += OnHostUp;
        client.OnConnectedEvent += OnClientConnected;

        hostScheme.OnValueChanged += OnSchemeChange;
        clientScheme.OnValueChanged += OnSchemeChange;

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(SchemeChangeSync);
        }
    }

    public override void OnNetworkDespawn()
    {
        host.OnCreatedEvent -= OnHostUp;
        client.OnConnectedEvent -= OnClientConnected;

        hostScheme.OnValueChanged -= OnSchemeChange;
        clientScheme.OnValueChanged -= OnSchemeChange;

        foreach (Button button in buttons)
        {
            button.onClick.RemoveListener(SchemeChangeSync);
        }
    }

    private void OnHostUp()
    {
        hostScheme.Value = PlayersSettings.Player1.ControlScheme;
        clientScheme.Value = PlayersSettings.Player2.ControlScheme;
        playersMenu.UpdatePlayersSchemes();
    }

    private void OnClientConnected()
    {
        PlayersSettings.Player1.ControlScheme = hostScheme.Value;
        RequestChangeSchemeServerRpc(PlayersSettings.Player2.ControlScheme);

        playersMenu.UpdatePlayersSchemes();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestChangeSchemeServerRpc(ControlScheme scheme)
    {
        clientScheme.Value = scheme;
    }

    private void SchemeChangeSync()
    {
        switch (Settings.GameMode)
        {
            case GameMode.Host:
                hostScheme.Value = PlayersSettings.Player1.ControlScheme;
                break;
            
            case GameMode.Client:
                RequestChangeSchemeServerRpc(PlayersSettings.Player2.ControlScheme);
                break;
        }
    }

    private void OnSchemeChange(ControlScheme oldScheme = 0, ControlScheme newScheme = 0)
    {
        PlayersSettings.Player1.ControlScheme = hostScheme.Value;
        PlayersSettings.Player2.ControlScheme = clientScheme.Value;
        if (SceneManagerAdapter.IsGameScene()) PlayersManager.Instance.UpdatePlayersControlScheme();
        playersMenu.UpdatePlayersSchemes();
    }
}
