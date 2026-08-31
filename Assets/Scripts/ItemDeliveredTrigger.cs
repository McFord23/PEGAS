using UnityEngine;
using UnityEngine.Events;

public class ItemDeliveredTrigger : MonoBehaviour
{
    public UnityEvent itemDeliveredEvent;
    
    private void OnTriggerEnter2D(Collider2D item)
    {
        if (item.gameObject.CompareTag("Item"))
        {
            itemDeliveredEvent?.Invoke();
        }
    }
}
