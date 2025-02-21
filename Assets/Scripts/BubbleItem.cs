using System.Collections.Generic;
using UnityEngine;

public class BubbleItem : MonoBehaviour
{
    public float moveLeftSpeed = 5;
    public float destroyThresholdX = -10;

    GameObject center;
    List<Rigidbody2D> physicsPoints;

    void Start()
    {
        center = GetComponent<AutoBubble>().center;
        var centerRb = center.GetComponent<Rigidbody2D>();

        // add all vertices to a list for moving them all at once later
        physicsPoints = new List<Rigidbody2D>();
        foreach (Transform obj in transform) {
            if (obj.tag == "BubblePoint") {
                physicsPoints.Add(obj.GetComponent<Rigidbody2D>());
            }
        }
        moveBubble(Vector2.left * moveLeftSpeed);
    }

    public void moveBubble(Vector2 dir) {
        foreach (var ver in physicsPoints) {
            ver.linearVelocity = dir;
        }
    }

    void FixedUpdate()
    {
        if (center.transform.position.x < destroyThresholdX) {
            Destroy(gameObject);
            Debug.Log("Despawned out of screen bubble: " + name);
        }
    }

}
