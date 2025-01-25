using UnityEngine;
using System.Collections;

public class SeaDevil : MonoBehaviour
{
    public float bubbleBarReduction = 30f;
    public float attackCooldown = 1f;

    private bool canAttack = true;
    public float detectionRange = 5f;
    public float attackRange = 1f;
    private GameObject player;
    private Animator animator;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();

        if (player == null)
        {
            Debug.LogError("Player not found.");
        }

        if (animator == null)
        {
            Debug.LogError("Animator not found on SeaDevil.");
        }
    }

    private void Update()
    {
        if (player == null || !canAttack)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange && canAttack)
        {
            TriggerAttack();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canAttack)
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
        }
    }

    private void TriggerAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}