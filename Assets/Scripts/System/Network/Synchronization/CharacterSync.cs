using Enums;
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
    
    private NetworkVariable<Character> characterClient = new (Character.Luna, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        host = HostMonitoring.Instance;
        client = ClientMonitoring.Instance;

        host.OnCreatedEvent += OnHostUp;
        client.OnConnectedEvent += OnClientConnected;

        characterHost.OnValueChanged += OnHostCharacterChange;
        characterClient.OnValueChanged += OnClientCharacterChange;

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(CharacterChangeSync);
        }
    }

    public override void OnNetworkDespawn()
    {
        host.OnCreatedEvent -= OnHostUp;
        client.OnConnectedEvent -= OnClientConnected;

        characterHost.OnValueChanged -= OnHostCharacterChange;
        characterClient.OnValueChanged -= OnClientCharacterChange;

        foreach (Button button in buttons)
        {
            button.onClick.RemoveListener(CharacterChangeSync);
        }
    }

    private void OnHostUp()
    {
        characterHost.Value = Global.players[0].character;
        characterClient.Value = Global.players[1].character;

        playersMenu.ChangeHostCharacter(characterHost.Value);
        playersMenu.ChangeClientCharacter(characterClient.Value);
    }

    private void OnClientConnected()
    {
        Global.players[0].character = characterHost.Value;
        Global.players[1].character = characterClient.Value;

        playersMenu.ChangeHostCharacter(characterHost.Value);
        playersMenu.ChangeClientCharacter(characterClient.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestChangeCharacterServerRpc(Character host, Character client)
    {
        characterHost.Value = host;
        characterClient.Value = client;
    }

    private void CharacterChangeSync()
    {
        if (Global.gameMode == GameMode.Host)
        {
            characterHost.Value = Global.players[0].character;
            characterClient.Value = Global.players[1].character;
        }
        else if (Global.gameMode == GameMode.Client)
        {
            RequestChangeCharacterServerRpc(Global.players[0].character, Global.players[1].character);
        }
    }

    private void OnHostCharacterChange(Character oldCharacter, Character newCharacter)
    {
        Global.players[0].character = newCharacter;
        playersMenu.ChangeHostCharacter(newCharacter);
    }

    private void OnClientCharacterChange(Character oldCharacter, Character newCharacter)
    {
        Global.players[1].character = newCharacter;
        playersMenu.ChangeClientCharacter(newCharacter);
    }
}
