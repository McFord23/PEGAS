using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSync : NetworkBehaviour
{
    [SerializeField]
    private PlayersMenu playersMenu;

    [SerializeField]
    private List<Button> buttons = new List<Button>();

    private HostMonitoring host;
    private ClientMonitoring client;

    private NetworkVariable<Character> characterHost = new (Character.Celestia, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        host = HostMonitoring.Instance;
        client = ClientMonitoring.Instance;

        host.OnCreatedEvent += OnHostUp;
        client.OnConnectedEvent += OnClientConnected;

        characterHost.OnValueChanged += OnCharacterChange;

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(CharacterChangeSync);
        }
    }

    public override void OnNetworkDespawn()
    {
        host.OnCreatedEvent -= OnHostUp;
        client.OnConnectedEvent -= OnClientConnected;

        characterHost.OnValueChanged -= OnCharacterChange;

        foreach (Button button in buttons)
        {
            button.onClick.RemoveListener(CharacterChangeSync);
        }
    }

    private void OnHostUp()
    {
        characterHost.Value = PlayersSettings.Player1.Character;
    }

    private void OnClientConnected()
    {
        if (PlayersSettings.Player2.Character == characterHost.Value)
        {
            playersMenu.ChangeCharacter();
        }
    }

    [ServerRpc]
    private void RequestChangeCharacterServerRpc(Character host)
    {
        characterHost.Value = host;
    }

    private void CharacterChangeSync()
    {
        if (Settings.GameMode == GameMode.Host)
        {
            characterHost.Value = PlayersSettings.Player1.Character;
        }
        else if (Settings.GameMode == GameMode.Client)
        {
            RequestChangeCharacterServerRpc(PlayersSettings.Player1.Character);
        }
    }

    private void OnCharacterChange(Character oldCharacter, Character newCharacter)
    {
        playersMenu.ChangeCharacter();
    }
}
