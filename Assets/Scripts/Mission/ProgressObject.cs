using UnityEngine;

public class ProgressObject : MonoBehaviour
{
    public delegate void ChangeProgressEvent(bool isIncrease);
    public event ChangeProgressEvent OnChangeProgressEvent;
    
    public virtual void ChangeProgress(bool isIncrease)
    {
        OnChangeProgressEvent?.Invoke(isIncrease);
    }
    
    public virtual void OnReset() { }
}