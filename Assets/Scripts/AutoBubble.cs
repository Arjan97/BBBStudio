using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.U2D;
using UnityEngine;

public class AutoBubble : MonoBehaviour
{
    public GameObject vertexPrefab;
    public SpriteShapeController skin;
    public GameObject highLight;
    public float radius = 0.4f;
    public int segments = 12;
    public float vertexRadius = 0.05f;
    public float vertexMass = 0.1f;
    public float centerSpringFrequency = 4f;
    public float neighborSpringFrequency = 3f;
    public float mergeDelay = 0.3f;
    public bool isWobble = false;
    public float wobbleStrength = 0.1f;
    public bool isWobbleImpulse = false;

    
    List<Transform> vertices;
    float vertexScale = 0.1f;
    GameObject lastMergedBubble;
    GameObject center;
    bool needSkinReset = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vertices = new List<Transform>();
        GenerateBubble();
        ResetSkin();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSkin();
        if (isWobble) {
            Wobble();
        }
    }

    void GenerateBubble() {
        vertexScale = vertexRadius / 0.5f; // unity's 2d circle sprite with scale 1 is of radius 0.5
        center = Spawnvertex(transform.position, null, null, name+"Center");
        highLight.transform.parent = center.transform; // so that highlight moves with bubble
        float radians = 0f;
        GameObject firstVertex = null;
        GameObject prevVertex = null;
        float radianIncr = (float)(2 * Math.PI / segments);
        for (int i = 0; i < segments; i++, radians += radianIncr) {
            float x = Mathf.Cos(radians) * radius;
            float y = Mathf.Sin(radians) * radius;
            var pos = transform.position + new Vector3(x, y);
            prevVertex = Spawnvertex(pos, center, prevVertex, name+(i+1));
            if (i == 0) {
                firstVertex = prevVertex.gameObject;
            }
        }
        AddNeighborSrping(firstVertex, prevVertex);
        firstVertex.GetComponent<BubbleVertex>().prevVertex = prevVertex;
        prevVertex.GetComponent<BubbleVertex>().nextVertex = firstVertex;
    }

    GameObject Spawnvertex(Vector3 pos, GameObject center, GameObject neighbor, string name) {
        var vertex = Instantiate(vertexPrefab, pos, Quaternion.identity, transform);
        vertex.name = name;
        vertex.transform.localScale = Vector3.one * vertexScale;
        vertex.GetComponent<Rigidbody2D>().mass = vertexMass;

        if (center != null) // Add first SpringJoint2D connecting to center
        {
            AddCenterSpring(vertex, center);
        }
        if (neighbor != null) // Add second SpringJoint2D connecting to neighbor
        {
            AddNeighborSrping(vertex, neighbor);
            neighbor.GetComponent<BubbleVertex>().nextVertex = vertex;
            vertex.GetComponent<BubbleVertex>().prevVertex = neighbor;
        }

        return vertex;
    }

    void AddCenterSpring(GameObject from, GameObject to) {
        SpringJoint2D joint1 = from.AddComponent<SpringJoint2D>();
        joint1.enableCollision = true;
        joint1.connectedBody = to.GetComponent<Rigidbody2D>();
        joint1.autoConfigureDistance = false;
        joint1.distance = radius;
        joint1.frequency = centerSpringFrequency;
        from.GetComponent<BubbleVertex>().jointToCenter = joint1;
    }

    void AddNeighborSrping(GameObject from, GameObject to) {
        SpringJoint2D joint2 = from.AddComponent<SpringJoint2D>();
        joint2.enableCollision = true;
        joint2.connectedBody = to.GetComponent<Rigidbody2D>();
        joint2.autoConfigureDistance = true;
        joint2.frequency = neighborSpringFrequency;
        from.GetComponent<BubbleVertex>().jointToNeighbor = joint2;
    }

    public void Merge(BubbleVertex a, BubbleVertex b) {
        GameObject otherBubble = b.gameObject.transform.parent.gameObject;
        if (lastMergedBubble == otherBubble) {
            return;
        }
        Debug.Log("Merge bubbles! Collided vertices: " + a.gameObject.name + ", " + b.gameObject.name);
        lastMergedBubble = otherBubble;

        // Glue bubbles together
        ReJoint(a.nextVertex, b.prevVertex);
        ReJoint(b.nextVertex, a.prevVertex);
        Destroy(a.gameObject);
        Destroy(b.gameObject);
        needSkinReset = true;

        Vector3 mergePoint = a.gameObject.transform.position;
        StartCoroutine(CompleteMergeAfterDelay(otherBubble, mergePoint, mergeDelay));
    }

    private IEnumerator CompleteMergeAfterDelay(GameObject otherBubble, Vector3 mergePoint, float delay) {
        yield return new WaitForSeconds(delay);
        center.transform.position = mergePoint;
        ReparentToNewCenter(otherBubble);
        ReconnectToCenterAndIncreaseRadius(otherBubble.GetComponent<AutoBubble>().radius);
        Destroy(otherBubble);
        needSkinReset = true;
    }

    void ReparentToNewCenter(GameObject parent) {
        Destroy(parent.GetComponent<AutoBubble>().center);
        while (parent.transform.childCount > 0) {
            Transform child = parent.transform.GetChild(parent.transform.childCount - 1);
            if(!child.gameObject.TryGetComponent<BubbleVertex>(out var otherVertex)) {
                Destroy(child.gameObject);
            }
            child.SetParent(transform);
        }
    }

    void ReconnectToCenterAndIncreaseRadius(float otherRadius) {
        float newRadius = Mathf.Sqrt(Mathf.Pow(radius, 2) + Mathf.Pow(otherRadius, 2));
        radius = newRadius;
        foreach (Transform child in transform) {
            if (child.gameObject.TryGetComponent<BubbleVertex>(out var vertex)) {
                var centerJoint = vertex.jointToCenter;
                if (centerJoint != null) {
                    centerJoint.connectedBody = center.GetComponent<Rigidbody2D>();
                    centerJoint.distance = newRadius;
                }
            }
        }
    }

    void ReJoint(GameObject vertex1, GameObject vertex2) {
        vertex1.GetComponent<BubbleVertex>().prevVertex = vertex2;
        vertex2.GetComponent<BubbleVertex>().nextVertex = vertex1;
        vertex1.GetComponent<BubbleVertex>().jointToNeighbor.connectedBody 
            = vertex2.GetComponent<Rigidbody2D>();
    }

    void Wobble() 
    {
        for (int i = 0; i < vertices.Count; i++) 
        {
            Vector2 randomVec = UnityEngine.Random.insideUnitCircle;
            ForceMode2D fmode = isWobbleImpulse ? ForceMode2D.Impulse : ForceMode2D.Force;
            vertices[i].gameObject.GetComponent<Rigidbody2D>().AddForce(
                randomVec * wobbleStrength, fmode);
        }
    }

    void ResetSkin() {
        UpdateVertices();
        skin.spline.Clear();
        for (int i = 0; i < vertices.Count; i++) 
        {
            skin.spline.InsertPointAt(i, vertices[i].localPosition);
            skin.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        }
    }

    void UpdateSkin() {
        // if (needSkinReset) {
        //     ResetSkin();
        //     needSkinReset = false;
        // }
        ResetSkin();
        for (int i = 0; i < vertices.Count; i++) 
        {
            try 
            {
                skin.spline.SetPosition(i, vertices[i].localPosition);
            } catch{}
            

            Vector2 radius = vertices[i].localPosition - center.transform.localPosition;
            Vector2 tangent = Vector2.Perpendicular(radius);

            skin.spline.SetLeftTangent(i, tangent.normalized * skin.spline.GetLeftTangent(i).magnitude);
            skin.spline.SetRightTangent(i, -tangent.normalized * skin.spline.GetRightTangent(i).magnitude);
            
        }
    }

    void UpdateVertices() {
        vertices.Clear();
        GameObject firstVertex = null;
        foreach (Transform t in transform) {
            if(t.gameObject.TryGetComponent<BubbleVertex>(out var vertex)) {
                if(vertex.prevVertex != null) {
                    firstVertex = vertex.gameObject;
                }
            }
        }
        var curVertex = firstVertex;
        while (true) {
            // so we don't use not yet reparented vertices
            if (curVertex.transform.parent == transform) { 
                vertices.Add(curVertex.transform);
            }
            curVertex = curVertex.GetComponent<BubbleVertex>().prevVertex;
            if (curVertex == firstVertex) {
                break;
            }
        }
    }
}
