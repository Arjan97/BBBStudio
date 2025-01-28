using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;

    [Header("Animator Settings")]
    public bool isStatic = false;
    public bool isDead = false;
    public bool isHit = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (animator != null)
        {
            animator.SetBool("isStatic", isStatic);
            animator.SetBool("isHit", isHit);
        }

        if (animator != null && isStatic)
        {
            SetStatic(true);
        }

        if (animator != null && isDead)
        {
            SetIsDead(true);
        }
    }

    public void SetIsHit(bool value)
    {
        if (animator != null)
        {
            isHit = value;
            animator.SetBool("isHit", isHit);
        }

        if (value && audioSource != null)
        {
            audioSource.Play();
        }
    }

    public void SetIsDead(bool value)
    {
        if (animator != null)
        {
            isDead = value;
            animator.SetTrigger("Death");
        }
    }

    public void SetStatic(bool value)
    {
        if (animator != null)
        {
            isStatic = value;
            animator.SetBool("isStatic", isStatic);
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
}
