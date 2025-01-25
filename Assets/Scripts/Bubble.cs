using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float valueToAdd = 10f;
    public float destroyDelay = 1f;

    private List<Rigidbody2D> vertices;
    private int baseScore = 200;

    private void Start()
    {
        vertices = new List<Rigidbody2D>();

        Rigidbody2D mainRb = GetComponent<Rigidbody2D>();
        if (mainRb != null)
        {
            vertices.Add(mainRb);
        }

        foreach (Transform child in transform)
        {
            if (child.CompareTag("BubblePoint"))
            {
                Rigidbody2D childRb = child.GetComponent<Rigidbody2D>();
                if (childRb != null)
                {
                    vertices.Add(childRb);
                }
            }
        }

        if (vertices.Count == 0)
        {
            Debug.LogError("No Rigidbody2D components found for bubble movement.");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") )
        {
            HandlePlayerCollisionWithDelay(0.05f);
            Debug.Log("Bubble collided with player");
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
