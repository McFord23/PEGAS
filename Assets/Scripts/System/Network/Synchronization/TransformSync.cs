using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class TransformSync : NetworkBehaviour
{
    private IEnumerator sync;

    private void Start()
    {
        if (!IsOwner) return;

        sync = Sync();

        if (IsClient) StartCoroutine(sync);
        else
        {
            HostMonitoring.Instance.OnClientConnectedEvent += OnNetworkCoopStart;
            ClientMonitoring.Instance.OnConnectedEvent += OnNetworkCoopStart;
        }
    }

    [ServerRpc]
    private void RequestUpdateTransformServerRpc(ulong clientID, float posX, float posY, float rotZ, float scaleY)
    {
        RequestUpdateTransformClientRpc(clientID, posX, posY, rotZ, scaleY);
    }

    [ClientRpc]
    private void RequestUpdateTransformClientRpc(ulong clientID, float posX, float posY, float rotZ, float scaleY)
    {
        if (NetworkManager.LocalClientId == clientID) return;

        transform.position = Vector2.Lerp(transform.position, new Vector2(posX, posY), Time.deltaTime); 
        transform.localScale = new Vector3(1, scaleY, 1);
        
        var transformRotation = transform.rotation;
        transformRotation.z = rotZ;
        transform.rotation = transformRotation;
    }

    private void OnNetworkCoopStart()
    {
        StopCoroutine(sync);
        StartCoroutine(sync);
    }

    private IEnumerator Sync()
    {
        while (IsClient && IsOwner)
        {
            var selfTransform = transform;
            var selfPosition = selfTransform.position;
            RequestUpdateTransformServerRpc(
                NetworkManager.LocalClientId,
                selfPosition.x,
                selfPosition.y,
                selfTransform.rotation.z,
                selfTransform.localScale.y
            );

            yield return null;
        }
    }
}
