using UnityEngine;
using System.Collections;
public class SeaUrchin : MonoBehaviour
{
    public float bubbleBarReduction = 30f;
    public float attackCooldown = 1f;

    private Animator animator;
    private bool canAttack = true;
    public float detectionRange = 5f; 
    public float attackRange = 2f;
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
            Debug.LogError("Player  not found.");
        }
    }

    private void Update()
    {
        if (player == null || !canAttack)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
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
        }
    }

    private void TriggerAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("AttackTrigger");
            StartCoroutine(AttackCooldown());

        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
