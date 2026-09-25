using System.Collections;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private PlayersManager playersManager;
    [SerializeField] private float moveSpeed = 0.1f;
    [SerializeField] private float maxDistance = 3;
    [SerializeField] private float minDistance = 1;

    private Vector3 offset;
    private bool isInitialized;
    
    private void Start()
    {
        StartCoroutine(WaitForPlayerSpawn());
    }
    
    private void Update()
    {
        if (!isInitialized) return;

        float distance = 1;
        
        if (Settings.GameMode is GameMode.LocalCoop)
        {
            var value = Vector3.Distance(playersManager.GetPosition(0), playersManager.GetPosition(1));
            distance = Mathf.Clamp(value, minDistance, maxDistance);
        }
        
        var target = playersManager.GetPosition() + offset * distance;
        transform.position = Vector3.Lerp(transform.position, target, moveSpeed);
    }

    private IEnumerator WaitForPlayerSpawn()
    {
        yield return new WaitForFixedUpdate();
        
        // для сетевого коопа нужно будет подписываться на окончание загрузки
        offset = transform.position - playersManager.GetPosition();
        isInitialized = true;
    }
}