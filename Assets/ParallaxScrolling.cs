using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public float scrollSpeed = 0.5f;
    public float resetDistance = 10f;

    private GameObject currentBackground;
    private GameObject nextBackground;
    private float backgroundWidth;

    private void Start()
    {
        currentBackground = gameObject;
        backgroundWidth = resetDistance;

        nextBackground = Instantiate(backgroundPrefab, GetNextPosition(), Quaternion.identity);
        nextBackground.transform.SetParent(transform.parent);
    }

    private void Update()
    {
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        if (transform.position.x <= -backgroundWidth)
        {
            Destroy(currentBackground);
            currentBackground = nextBackground;
            nextBackground = Instantiate(backgroundPrefab, GetNextPosition(), Quaternion.identity);
            nextBackground.transform.SetParent(transform.parent);
        }
    }

    private Vector3 GetNextPosition()
    {
        return new Vector3(currentBackground.transform.position.x + backgroundWidth, currentBackground.transform.position.y, currentBackground.transform.position.z);
    }
}
