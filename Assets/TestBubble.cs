using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.U2D;
using UnityEngine;
using UnityEngine.UI;

public class TestBubble : MonoBehaviour
{
    public GameObject vertexPrefab;
    public float radius = 0.4f;
    public int segmentCount = 12;
    public float vertexRadius = 0.05f;
    public float vertexMass = 0.1f;
    public float centerSpringFrequency = 4f;
    public float neighborSpringFrequency = 3f;
    public float mergeDelay = 0.3f;
    public float unscrambleDelay = 0.3f;
    public float scrambleDotThreshold = 0.5f;

    public bool isWobble = false;
    public float wobbleDelta = 0.2f; // e.g. 0.1 means +/-0.1 * radius
    public float wobbleStrength = 0.1f;
    public bool isWobbleImpulse = false;

    public bool isDeflating = false;
    public float deflateSpeed = 0.1f;
    public float minRadius = 0.25f;
    public int minVertexCount = 10;
    public int maxVertexCount = 20;

    List<Transform> vertices;
    float vertexScale = 0.1f;
    GameObject lastMergedBubble;
    GameObject center;
    bool needSkinReset = false;
    bool isUnscrambling = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vertices = new List<Transform>();
        GenerateBubble();
        // ResetSkin();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSkin();
        if (isWobble) {
            Wobble();
        }
        if (isUnscrambling) {
            UnscrambleVertices();
        }
        if (isDeflating) {
            Deflate();
        }
    }


    void GenerateBubble() {
        vertexScale = vertexRadius / 0.5f; // unity's 2d circle sprite with scale 1 is of radius 0.5
        center = Spawnvertex(transform.position, null, null, name+"Center");

        float radians = 0f;
        GameObject firstVertex = null;
        GameObject prevVertex = null;
        float radianIncr = (float)(2 * Math.PI / segmentCount);
        for (int i = 0; i < segmentCount; i++, radians += radianIncr) {
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
        // joint1.enableCollision = true;
        joint1.connectedBody = to.GetComponent<Rigidbody2D>();
        joint1.autoConfigureDistance = false;
        joint1.distance = radius;
        joint1.frequency = centerSpringFrequency;
        from.GetComponent<BubbleVertex>().jointToCenter = joint1;
    }

    void AddNeighborSrping(GameObject from, GameObject to) {
        SpringJoint2D joint2 = from.AddComponent<SpringJoint2D>();
        // joint2.enableCollision = true;
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

    IEnumerator CompleteMergeAfterDelay(GameObject otherBubble, Vector3 mergePoint, float delay) {
        yield return new WaitForSeconds(delay);
        center.transform.position = mergePoint;
        ReparentToNewCenter(otherBubble);
        ReconnectToCenterAndIncreaseRadius(otherBubble.GetComponent<TestBubble>().radius);
        Destroy(otherBubble);
        needSkinReset = true;
        StartCoroutine(StartUnscrambleVertices());
    }

    IEnumerator StartUnscrambleVertices() { // TODO: unify iteration with UpdateVertices
        yield return new WaitForSeconds(unscrambleDelay);
        isUnscrambling = true;
    }

    void UnscrambleVertices() {
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
                if (DeleteVertexIfScrambled(curVertex)) {
                    return;
                }
            }
            curVertex = curVertex.GetComponent<BubbleVertex>().prevVertex;
            if (curVertex == firstVertex) {
                break;
            }
        }
        isUnscrambling = false; // stop unscrambling if no vertices are scrambled
    }

    bool DeleteVertexIfScrambled (GameObject vertex) {
        var pos = vertex.transform.position;
        var prev = vertex.GetComponent<BubbleVertex>().prevVertex;
        var next = vertex.GetComponent<BubbleVertex>().nextVertex;
        var vecToPrev = prev.transform.position - pos;
        var vecToNext = next.transform.position - pos;
        if (Vector3.Dot(vecToNext.normalized, vecToPrev.normalized) > scrambleDotThreshold) {
            ReJoint(next, prev);
            Destroy(vertex);
            needSkinReset = true;
            Debug.Log("Unscrambled " + vertex.name);
            return true;
        }
        return false;
    }

    void ReparentToNewCenter(GameObject parent) {
        Destroy(parent.GetComponent<TestBubble>().center);
        while (parent.transform.childCount > 0) {
            Transform child = parent.transform.GetChild(parent.transform.childCount - 1);
            if(!child.gameObject.TryGetComponent<BubbleVertex>(out var otherVertex)) {
                Destroy(child.gameObject);
            } else {
                segmentCount++;
            }
            child.SetParent(transform); // unwanted children will be destoyed next frame
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
        int j = UnityEngine.Random.Range(0, vertices.Count);
        for (int i = 0; i < vertices.Count; i++) 
        {
            // Vector2 randomVec = UnityEngine.Random.insideUnitCircle;
            // ForceMode2D fmode = isWobbleImpulse ? ForceMode2D.Impulse : ForceMode2D.Force;
            // vertices[i].gameObject.GetComponent<Rigidbody2D>().AddForce(
            //     randomVec * wobbleStrength, fmode);
            
            var vertex = vertices[i].gameObject.GetComponent<BubbleVertex>();
            if (i == j) {
                var rnd = UnityEngine.Random.value * 2 - 1;
                vertex.jointToCenter.distance += wobbleDelta * vertex.jointToCenter.distance * rnd;
            } else {
                vertex.jointToCenter.distance = radius;
            }
           
        }
    }

    void ResetSkin() {
        UpdateVertices();
        // skin.spline.Clear();
        // for (int i = 0; i < vertices.Count; i++) 
        // {
        //     skin.spline.InsertPointAt(i, vertices[i].localPosition);
        //     skin.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        // }
    }

    void UpdateSkin() {
        // if (needSkinReset) {
        //     ResetSkin();
        //     needSkinReset = false;
        // }
        ResetSkin();
        // for (int i = 0; i < vertices.Count; i++) 
        // {
        //     try 
        //     {
        //         skin.spline.SetPosition(i, vertices[i].localPosition);
        //     } catch{}
            

        //     Vector2 radius = vertices[i].localPosition - center.transform.localPosition;
        //     Vector2 tangent = Vector2.Perpendicular(radius);

        //     skin.spline.SetLeftTangent(i, tangent.normalized * skin.spline.GetLeftTangent(i).magnitude);
        //     skin.spline.SetRightTangent(i, -tangent.normalized * skin.spline.GetRightTangent(i).magnitude);
            
        // }
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

    void Deflate() {
        // decrease radius and circle segment length
        // occasionaly remove vertices -> another procedure
        if (radius <= minRadius) {
            // Debug.Log("Deflated to minimum radius");
            return;
        }

        radius -= Time.deltaTime * deflateSpeed;
        float seg_len = 2 * Mathf.PI * radius / segmentCount;

        // for every vertex 
        // center joint dist = radius
        // neighbor joint dist = seg_len
        foreach (Transform child in transform) {
            if (child.gameObject.TryGetComponent<BubbleVertex>(out var vertex)) {
                var centerJoint = vertex.jointToCenter;
                if (centerJoint != null) {
                    centerJoint.distance -= Time.deltaTime * deflateSpeed;
                }
                var neighborJoint = vertex.jointToNeighbor;
                if (neighborJoint != null) {
                    neighborJoint.distance = seg_len;
                }
            }
        }
    }
}