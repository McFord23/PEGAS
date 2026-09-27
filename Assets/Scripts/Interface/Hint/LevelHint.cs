using UnityEngine;

public class LevelHint : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Show()
    {
        animator.Play("Show");
    }

    public void Hide()
    {
        animator.Play("Hide");
    }

    public void OnReset()
    {
        animator.Play("Idle");
    }
}