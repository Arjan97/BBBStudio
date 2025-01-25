using UnityEngine;

public class collisionChecker : MonoBehaviour
{
    public int baseScore = 200;
    public float valueToAdd = 10f;
    public bool isCeiling = false;
    public bool isWall = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isCeiling && !isWall)
        {
            HandlePlayerCollisionWithDelay(0.05f);
        } else
        {
            if (isCeiling && !collision.CompareTag("Bubble"))
            {
                KillPlayer();
            }

            if (collision.CompareTag("Bubble"))
            {
                Destroy(collision.gameObject, 0.2f);
            }
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

    private void KillPlayer()
    {
        BubbleBar bubbleBar = FindFirstObjectByType<BubbleBar>();
        if (bubbleBar != null)
        {
            bubbleBar.TriggerGameOver();
        }
    }
    private System.Collections.IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
