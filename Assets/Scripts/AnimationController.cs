using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Animator Settings")]
    public bool isStatic = false;
    public bool isDead = false;

    [Header("Hit Color Settings")]
    public Color hitColor = Color.red;
    public float colorTime = 0.5f;

    private Color originalColor;
    private AudioSource audioSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

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
        if (animator != null && spriteRenderer != null)
        {
            animator.SetTrigger("Death");
            spriteRenderer.color = hitColor;
            if (audioSource != null)
            {
                audioSource = GetComponent<AudioSource>();
                audioSource.Play();
            }
            StartCoroutine(ResetColor(colorTime));
        }
    }

    IEnumerator ResetColor(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
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
