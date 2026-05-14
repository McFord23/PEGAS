using Enums;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ItemAdapter : NetworkBehaviour
{
    private CollectingItem item;

    private NetworkVariable<FixedString64Bytes> characterPath = new (
        "",
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner
    );

    private void Awake()
    {
        item = GetComponent<CollectingItem>();
    }

    public void PickUp(Transform character)
    {
        switch (Global.gameMode)
        {
            case GameMode.Single:
            case GameMode.LocalCoop:
                item.Pickup(character);
                break;
            
            case GameMode.Host:
                FixedString64Bytes path = Utilities.GetPath(character);
                RequestPickUpServerRpc(path);
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestPickUpServerRpc(FixedString64Bytes path)
    {
        characterPath.Value = path;
        RequestPickUpClientRpc(path);
    }

    [ClientRpc]
    private void RequestPickUpClientRpc(FixedString64Bytes path)
    {
        Transform character = GameObject.Find(path.ToString()).transform;
        item.Pickup(character);
    }

    public void Reset()
    {
        switch (Global.gameMode)
        {
            case GameMode.Single:
            case GameMode.LocalCoop:
                item.Reset();
                break;
            
            case GameMode.Host:
            case GameMode.Client:
                RequestResetServerRpc();
                break;
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RequestResetServerRpc()
    {
        RequestResetClientRpc();
    }

    [ClientRpc]
    private void RequestResetClientRpc()
    {
        item.Reset();
    }
    
    public void Drop()
    {
        switch (Global.gameMode)
        {
            case GameMode.Single:
            case GameMode.LocalCoop:
                item.Drop();
                break;
            
            case GameMode.Host:
            case GameMode.Client:
                RequestDropServerRpc();
                break;
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RequestDropServerRpc()
    {
        RequestDropClientRpc();
    }

    [ClientRpc]
    private void RequestDropClientRpc()
    {
        item.Drop();
    }
}
