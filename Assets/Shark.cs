using UnityEngine;
using System.Collections;

public class Shark : MonoBehaviour
{
    public float bubbleBarReduction = 20f;
    public float attackCooldown = 1.5f;
    public float attackRange = 5f; // Customizable attack range
    public bool smallShark = false; // Determines custom movement behavior
    public float moveSpeed = 2f; // Horizontal movement speed
    public float verticalAmplitude = 1f; // Vertical movement range
    public float verticalFrequency = 1f; // Vertical movement speed
    public float deleteThresholdX = -10f; // Distance after which shark is destroyed

    private Animator animator;
    private bool canAttack = true;
    private GameObject player;
    private float initialY;

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

        if (distanceToPlayer <= attackRange)
        {
            TriggerAttack(false);
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
            TriggerAttack(true);
        }
    }

    private void TriggerAttack(bool overrideCooldown)
    {
        if (animator != null && (canAttack || overrideCooldown))
        {
            if (!overrideCooldown)
            {
                StartCoroutine(AttackCooldown());
            }
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