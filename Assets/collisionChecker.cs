using UnityEngine;

public class collisionChecker : MonoBehaviour
{
    public int baseScore = 200;
    public float valueToAdd = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bubble"))
        {
            Destroy(collision.gameObject, 0.2f);
        }
        else 
        {
            HandlePlayerCollisionWithDelay(0.05f);
        }
    }

    private void HandlePlayerCollisionWithDelay(float delay)
    {
        ComboManager comboManager = FindFirstObjectByType<ComboManager>();
        BubbleBar bubbleBar = FindFirstObjectByType<BubbleBar>();

        if (comboManager != null)
        {
            comboManager.AddScore(baseScore);
            comboManager.IncrementMultiplier();
        }

        if (bubbleBar != null)
        {
            bubbleBar.AddValue(valueToAdd);
        }

        StartCoroutine(DestroyAfterDelay(delay));
    }

    private System.Collections.IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
