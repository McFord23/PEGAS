using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Open()
    {
        animator.Play("Open");
    }

    public void Close()
    {
        animator.Play("Close");
    }
}