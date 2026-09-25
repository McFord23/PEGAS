using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProgressObjectsManager : MonoBehaviour
{
    [SerializeField] protected List<ProgressObject> progressObjects;
    [SerializeField] protected UnityEvent progressIncreaseEvent;
    [SerializeField] protected UnityEvent progressDecreaseEvent;
    
    protected virtual void Start()
    {
        foreach (var progressObject in progressObjects)
        {
            progressObject.OnChangeProgressEvent += OnChangeProgress;
        }
    }
    
    private void OnDestroy()
    {
        foreach (var progressObject in progressObjects)
        {
            progressObject.OnChangeProgressEvent -= OnChangeProgress;
        }
    }
    
    public void OnReset()
    {
        foreach (var progressObject in progressObjects)
        {
            progressObject.OnReset();
        }
    }
    
    public virtual int GetTargetCount()
    {
        return progressObjects.Count;
    }
    
    protected virtual void OnChangeProgress(bool isIncrease)
    {
        if (isIncrease) progressIncreaseEvent?.Invoke();
        else progressDecreaseEvent?.Invoke();
    }
}