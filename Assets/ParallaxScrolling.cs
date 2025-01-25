using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    public Transform image1;
    public Transform image2;
    public float resetDistance = 10f;

    private void Update()
    {
        image1.position += Vector3.left * scrollSpeed * Time.deltaTime;
        image2.position += Vector3.left * scrollSpeed * Time.deltaTime;

        if (image1.position.x <= -resetDistance)
        {
            image1.position = new Vector3(image2.position.x + resetDistance, image1.position.y, image1.position.z);
        }

        if (image2.position.x <= -resetDistance)
        {
            image2.position = new Vector3(image1.position.x + resetDistance, image2.position.y, image2.position.z);
        }
    }
}
