using UnityEngine;
using System.Collections;

public class Shark : MonoBehaviour
{
    public float bubbleBarReduction = 20f;
    public float attackCooldown = 1f;
    public float attackRange = 3f; 

    private Animator animator;
    private bool canAttack = true;
    private GameObject player;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (animator == null)
        {
            Debug.LogError("Animator component not found on this object.");
        }
        if (player == null)
        {
            Debug.LogError("Player not found.");
        }
    }

    private void Update()
    {
        if (player == null || !canAttack)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            TriggerAttack();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            BubbleBar bubbleBar = FindFirstObjectByType<BubbleBar>();
            if (bubbleBar != null)
            {
                bubbleBar.currentValue -= bubbleBarReduction;
                bubbleBar.currentValue = Mathf.Max(bubbleBar.currentValue, 0);
                bubbleBar.FlashBubbleBarColor();
            }

            AnimationController animationController = collision.GetComponent<AnimationController>();
            if (animationController != null)
            {
                animationController.SetDeathTrigger();
            }
            TriggerAttack();
        }
    }

    private void TriggerAttack()
    {
        if (animator != null && canAttack)
        {
            StartCoroutine(AttackCooldown());
            animator.SetTrigger("Attack");
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}