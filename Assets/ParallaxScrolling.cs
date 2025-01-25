using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private Vector3 startPosition;
    private float resetPositionX;

    private void Start()
    {
        startPosition = transform.position;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            resetPositionX = startPosition.x - spriteRenderer.bounds.size.x;
        }
        else
        {
            Debug.LogError("ParallaxScrolling requires a SpriteRenderer on the same GameObject.");
        }
    }

    private void Update()
    {
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        if (transform.position.x <= resetPositionX)
        {
            transform.position = startPosition;
        }
    }
}
