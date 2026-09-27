using UnityEngine;
using UnityEngine.Events;

public class EnterTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent enterEvent;
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        enterEvent?.Invoke();
    }

    private void OnTriggerEnter(Collider collider)
    {
        enterEvent?.Invoke();
    }
    
    private void OnTriggerExit(Collider collider)
    {
        enterEvent?.Invoke();
    }
}