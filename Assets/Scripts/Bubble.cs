using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float valueToAdd = 10f;
    public float floatSpeed = 1f;
    public float floatAmplitude = 0.5f;
    public float scaleSpeed = 1.5f;
    public float scaleAmplitude = 0.5f;

    private Vector3 startPosition; 
    private Vector3 initialScale;

    private int baseScore = 200;

    private void Start()
    {
        startPosition = transform.position;
        initialScale = transform.localScale;
    }

    private void Update()
    {
        transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localScale = initialScale + Vector3.one * Mathf.Sin(Time.time * scaleSpeed) * scaleAmplitude;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ComboManager comboManager = FindObjectOfType<ComboManager>();
            BubbleBar bubbleBar = FindObjectOfType<BubbleBar>();

            if (comboManager != null)
            {
                comboManager.AddScore(baseScore);
                comboManager.IncrementMultiplier();
                bubbleBar.AddValue(valueToAdd);
            }

            Destroy(gameObject);
        }
    }
}
