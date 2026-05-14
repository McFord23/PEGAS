using Enums;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

public class WindigoDestructor : SingletonNetworkBehaviour<WindigoDestructor>
{
    private NetworkVariable<bool> hostDontSee = new(
        false,
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner
    );
    
    private NetworkVariable<bool> clientDontSee = new(
        false,
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner
    );
    
    public void UpdateSeeStatus()
    {
        if (IsHost) hostDontSee.Value = false;
        else RequestUpdateClientSeeServerRpc(false);
    }
    
    public void Destruct(Windigo windigo, bool ignoreSee)
    {
        if (Global.gameMode is GameMode.Single or GameMode.LocalCoop)
        {
            WindigosManager.Instance.Remove(windigo);
            Destroy(windigo.gameObject);
        }
        else
        {
            FixedString64Bytes path = Utilities.GetPath(windigo.transform);
            
            if (ignoreSee) 
            {
                RequestDestroyServerRpc(path);
            }
            else
            {
                if (IsHost) hostDontSee.Value = true;
                else RequestUpdateClientSeeServerRpc(true);
            
                if (hostDontSee.Value && clientDontSee.Value)
                {
                    print(path);
                    RequestDestroyServerRpc(path);
                }
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RequestUpdateClientSeeServerRpc(bool client)
    {
        clientDontSee.Value = client;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RequestDestroyServerRpc(FixedString64Bytes path)
    {
        RequestRemoveClientRpc(path);
        
        GameObject windigo = GameObject.Find(path.ToString());
        windigo.GetComponent<NetworkObject>().Despawn();
        Destroy(windigo);
    }
    
    [ClientRpc]
    private void RequestRemoveClientRpc(FixedString64Bytes path)
    {
        Windigo windigo = GameObject.Find(path.ToString()).GetComponent<Windigo>();
        WindigosManager.Instance.Remove(windigo);
    }
}
