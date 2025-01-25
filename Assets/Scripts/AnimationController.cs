using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    [Header("Animator Settings")]
    public bool isStatic = false;
    public bool isDead = false;
    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetBool("isStatic", isStatic);
        }

        if (animator != null && isStatic)
        {
            SetStatic(true);  
        }

        if (animator != null && isDead)
        {
            SetStaticDeathTrigger();
        }
    }
    public void SetDeathTrigger()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }

    public void SetStaticDeathTrigger()
    {
        if (animator != null)
        {
            animator.SetTrigger("staticDeath");
        }
    }

    public void SetIsFlying(bool value)
    {
        if (animator != null)
        {
            animator.SetBool("isFlying", value);
        }
    }

    public void SetFliesAway()
    {
        if (animator != null)
        {
            animator.SetTrigger("fliesAway");
        }
    }

    public void SetStatic(bool value)
    {
        if (animator != null)
        {
            animator.SetBool("isStatic", value);
        }
    }
}