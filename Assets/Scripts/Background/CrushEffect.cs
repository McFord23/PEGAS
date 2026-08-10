using UnityEngine;

public class CrushEffect : MonoBehaviour
{
    private Transform parent;
    private Animator animator;

    private void Start()
    {
        parent = transform.parent;
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    public void StartExplode()
    {
        transform.parent = null;
        animator.enabled = true;
        animator.Play("Explode");
    }

    //присоединен к анимации через AnimationEvent
    public void OnExplodeFinish()
    {
        transform.parent = parent;
        animator.enabled = false;
    }
}
