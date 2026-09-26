using UnityEngine;

public class Door : MonoBehaviour
{
    public bool IsOpen { get; private set; }
    
    [SerializeField] private Animator animator;

    public void Open()
    {
        if (IsOpen) return;
        animator.Play("Open");
        IsOpen = true;
    }

    public void Close()
    {
        if (!IsOpen) return;
        animator.Play("Close");
        IsOpen = false;
    }
}