using UnityEngine;
using System.Collections;

public class Shark : MonoBehaviour
{
    public float bubbleBarReduction = 20f;
    public float attackCooldown = 1.5f;
    public float detectionRange = 5f;
    public float attackRange = 2f;
    public bool smallShark = false;
    public float moveSpeed = 2f;
    public float verticalAmplitude = 1f;
    public float verticalFrequency = 1f;
    public float deleteThresholdX = -10f;

    private Animator animator;
    private bool canAttack = true;
    private GameObject player;
    private float initialY;
    private GameObject bubble;

    private AutoBubble autoBubble;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        initialY = transform.position.y;

        if (animator == null)
        {
            Debug.LogError("Animator component not found on this object.");
        }
        if (player == null)
        {
            Debug.LogError("Player not found.");
        }

        bubble = GameObject.FindGameObjectWithTag("PlayerBubble");
        if (autoBubble == null)
        {
            autoBubble = bubble.GetComponent<AutoBubble>();
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        if (smallShark)
        {
            CustomMovement();
        }

        if (!canAttack)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
        {
            TriggerAttack();
        }
    }

    private void CustomMovement()
    {
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        float newY = initialY + Mathf.Sin(Time.time * verticalFrequency) * verticalAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (transform.position.x <= deleteThresholdX)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TriggerAttackImmediate();
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null && playerController.CanTakeHit())
            {
                if (autoBubble != null)
                {
                    autoBubble.DecreaseRadiusByAmount(bubbleBarReduction);
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
            animator.SetTrigger("Attack");
        }
    }

    private void TriggerAttackImmediate()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");

            if (canAttack)
            {
                StartCoroutine(AttackCooldown());
            }
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
