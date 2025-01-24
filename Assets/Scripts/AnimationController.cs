using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetDeathTrigger()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }

    public void SetIsFlying(bool value)
    {
        if (animator != null)
        {
            animator.SetBool("isFlying", value);
        }
    }
}
