using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; 
    public float deleteThresholdX = -10f; 
    public bool isBubble = false; 
    public float tapForce = 10f;

    private List<Rigidbody2D> vertices; 

    private void Start()
    {
        if (isBubble)
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
    }

    private void Update()
    {
        if (!isBubble)
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

          
        }

        if (transform.position.x <= deleteThresholdX)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (isBubble && vertices != null)
        {
            foreach (var vertex in vertices)
            {
                vertex.AddForce(Vector2.left * tapForce * Time.fixedDeltaTime);
            }
        }
    }
}