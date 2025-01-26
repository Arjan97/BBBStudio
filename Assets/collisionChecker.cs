using UnityEngine;
using System.Collections;

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
        }
        else if (isCeiling && !isWall && !collision.CompareTag("Bubble") && !collision.CompareTag("BubblePoint") && !collision.CompareTag("SeaUrchin"))
        {
            Debug.Log(collision.gameObject.name);
            KillPlayer();
        }
        else if (collision.CompareTag("Bubble"))
        {
            StartCoroutine(DestroyBubbleAfterDelay(collision.gameObject, 0.2f));
            Debug.Log("Destroying " + collision.gameObject.name);
        }
        else
        {
            Debug.Log("Collision with " + collision.gameObject.name);
        }
    }

    private IEnumerator DestroyBubbleAfterDelay(GameObject bubble, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bubble);
        Debug.Log("Destroyed " + gameObject.name);
    }

    private void HandlePlayerCollisionWithDelay(float delay)
    {
        ComboManager comboManager = FindFirstObjectByType<ComboManager>();
        BubbleBar bubbleBar = FindFirstObjectByType<BubbleBar>();

        if (comboManager != null)
        {
            comboManager.AddBubbleScore(baseScore);

            if (comboManager.CanIncrementMultiplier())
            {
                comboManager.IncrementMultiplier(); 
            }
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

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
        Debug.Log("Destroyed " + gameObject.name);
    }
}
