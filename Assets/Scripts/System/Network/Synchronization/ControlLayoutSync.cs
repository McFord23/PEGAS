using Enums;
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
        layoutHost.Value = Global.players[0].controlLayout;
        Global.players[1].controlLayout = layoutClient.Value;

        playersMenu.UpdatePlayersLayout();
    }

    private void OnClientConnected()
    {
        Global.players[0].controlLayout = layoutHost.Value;
        RequestChangeLayoutServerRpc(Global.players[1].controlLayout);

        playersMenu.UpdatePlayersLayout();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestChangeLayoutServerRpc(ControlLayout layout)
    {
        layoutClient.Value = layout;
    }

    private void LayoutChangeSync()
    {
        if (Global.gameMode == GameMode.Host) layoutHost.Value = Global.players[0].controlLayout;
        else if (Global.gameMode == GameMode.Client) RequestChangeLayoutServerRpc(Global.players[1].controlLayout);
    }

    private void OnLayoutChange(ControlLayout oldLayout = 0, ControlLayout newLayout = 0)
    {
        Global.players[0].controlLayout = layoutHost.Value;
        Global.players[1].controlLayout = layoutClient.Value;
        playersMenu.UpdatePlayersLayout();
    }
}
