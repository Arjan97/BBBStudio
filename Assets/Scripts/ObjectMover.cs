using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; 
    public float deleteThresholdX = -10f; 

    private void Update()
    {
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        if (transform.position.x <= deleteThresholdX)
        {
            Destroy(gameObject);
        }
    }
}
