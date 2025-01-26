using UnityEngine;
using System.Collections;

public class bubbleDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
      if (collision.CompareTag("Bubble"))
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
        bubble.GetComponent<Bubble>().popBubble();
        yield return new WaitForSeconds(0.5f);
        Destroy(bubble);
        Debug.Log("Destroyed " + bubble.name);

    }
}
