using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class ControlLayoutSync : NetworkBehaviour
{
    [SerializeField]
    private PlayersMenu playersMenu;

    [SerializeField]
    private List<Button> buttons = new List<Button>();

    private HostMonitoring host;
    private ClientMonitoring client;

    private NetworkVariable<ControlLayout> layoutHost = new (ControlLayout.Mouse, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    private NetworkVariable<ControlLayout> layoutClient = new (ControlLayout.WASD, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        host = HostMonitoring.Instance;
        client = ClientMonitoring.Instance;

        host.OnCreatedEvent += OnHostUp;
        client.OnConnectedEvent += OnClientConnected;

        layoutHost.OnValueChanged += OnLayoutChange;
        layoutClient.OnValueChanged += OnLayoutChange;

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(LayoutChangeSync);
        }
    }

    public override void OnNetworkDespawn()
    {
        host.OnCreatedEvent -= OnHostUp;
        client.OnConnectedEvent -= OnClientConnected;

        layoutHost.OnValueChanged -= OnLayoutChange;
        layoutClient.OnValueChanged -= OnLayoutChange;

        foreach (Button button in buttons)
        {
            button.onClick.RemoveListener(LayoutChangeSync);
        }
    }

    private void OnHostUp()
    {
        layoutHost.Value = PlayersSettings.Player1.ControlLayout;
        PlayersSettings.Player2.ControlLayout = layoutClient.Value;
        playersMenu.UpdatePlayersLayout();
    }

    private void OnClientConnected()
    {
        PlayersSettings.Player1.ControlLayout = layoutHost.Value;
        RequestChangeLayoutServerRpc(PlayersSettings.Player2.ControlLayout);

        playersMenu.UpdatePlayersLayout();
    }

    [ServerRpc]
    private void RequestChangeLayoutServerRpc(ControlLayout layout)
    {
        layoutClient.Value = layout;
    }

    private void LayoutChangeSync()
    {
        if (Settings.GameMode == GameMode.Host) layoutHost.Value = PlayersSettings.Player1.ControlLayout;
        else if (Settings.GameMode == GameMode.Client) RequestChangeLayoutServerRpc(PlayersSettings.Player2.ControlLayout);
    }

    private void OnLayoutChange(ControlLayout oldLayout = 0, ControlLayout newLayout = 0)
    {
        PlayersSettings.Player1.ControlLayout = layoutHost.Value;
        PlayersSettings.Player2.ControlLayout = layoutClient.Value;
        playersMenu.UpdatePlayersLayout();
    }
}
