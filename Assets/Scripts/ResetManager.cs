using UnityEngine;
using UnityEngine.Events;

/**
 * Менеджер-прокладка для группирования сброса тематически объеденённых объектов
 */
public class ResetManager : MonoBehaviour
{
    [SerializeField] private UnityEvent resetEvent;

    public void OnReset()
    {
        resetEvent?.Invoke();
    }
}