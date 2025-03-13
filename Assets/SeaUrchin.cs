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
    private GameObject bubble;

    private AutoBubble autoBubble;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        bubble = GameObject.FindGameObjectWithTag("PlayerBubble");

        if (animator == null)
        {
            Debug.LogError("Animator component not found on this object.");
        }
        if (player == null)
        {
            Debug.LogError("Player  not found.");
        }
        if (autoBubble == null)
        {
            autoBubble = bubble.GetComponent<AutoBubble>();
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
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null && playerController.CanTakeHit())
            {
                if (autoBubble != null)
                {
                    autoBubble.DecreaseRadiusByAmount(bubbleBarReduction);
                    Debug.Log("decreasing" + bubbleBarReduction);
                } else 
                {
                    Debug.Log("AutoBubble not found on player.");
                }
                playerController.RegisterHit();
            }
        }
    }

    private void TriggerAttack()
    {
        if (animator != null && canAttack)
        {
            StartCoroutine(AttackCooldown());

            animator.SetTrigger("AttackTrigger");

        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
