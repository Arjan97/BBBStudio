using UnityEngine;

public class BubbleVertex : MonoBehaviour
{
    public GameObject prevVertex;
    public GameObject nextVertex;
    public SpringJoint2D jointToCenter;
    public SpringJoint2D jointToNeighbor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(!transform.parent.CompareTag("PlayerBubble")) {
            return;
        }
        if(!other.collider.TryGetComponent<BubbleVertex>(out var otherVertex)) {
            return;
        }
        if(transform.parent == other.collider.gameObject.transform.parent) {
            return;
        }
        transform.parent.GetComponent<AutoBubble>().Merge(this, otherVertex);
    }
}
