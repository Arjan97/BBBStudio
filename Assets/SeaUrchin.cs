using UnityEngine;
using System.Collections;

public class SeaUrchin : MonoBehaviour
{
    public float bubbleBarReduction = 30f;
    public float attackCooldown = 2f;

    private Animator animator;
    private bool canAttack = true;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on this object.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canAttack && collision.CompareTag("Player"))
        {
            TriggerAttack(collision.gameObject);
        }
    }

    private void TriggerAttack(GameObject player)
    {
        if (animator != null)
        {
            animator.SetTrigger("AttackTrigger");
        }

        BubbleBar bubbleBar = FindFirstObjectByType<BubbleBar>();
        if (bubbleBar != null)
        {
            bubbleBar.currentValue -= bubbleBarReduction;
            bubbleBar.currentValue = Mathf.Max(bubbleBar.currentValue, 0);
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
