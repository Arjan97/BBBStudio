using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public GameObject Highlight;
    public GameObject Skin;
    public GameObject PopAnimation;

    private List<Rigidbody2D> vertices;

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

    public void popBubble() {
        
        Highlight.SetActive(false);
        Skin.SetActive(false);
        PopAnimation.SetActive(true);
    }
}
