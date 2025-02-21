using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.U2D;
using UnityEngine;
using UnityEngine.UI;

public class AutoBubble : MonoBehaviour
{
    public GameObject vertexPrefab;
    public SpriteShapeController skin;
    public GameObject highlight;
    public float radius = 0.4f;
    public int segmentCount = 12;
    public float vertexRadius = 0.05f;
    public float vertexMass = 0.1f;
    public float centerSpringFrequency = 4f;
    public float neighborSpringFrequency = 3f;
    public float centerSpringDamping = 0f;
    public float neighborSpringDamping = 1f;
    public float unscrambleDelay = 0.3f;
    public float scrambleDotThreshold = 0.5f;

    public bool isWobble = false;
    public float wobbleAmplitude = 0.02f;
    public float wobbleFrequency = 2f;
    public float wobbleSpeed = 5f;
    // public float wobbleStrength = 0.1f;
    // public bool isWobbleImpulse = false;

    public bool isDeflating = false;
    public float deflateSpeed = 0.1f;
    public float minRadius = 0.25f;
    public int minVertexCount = 10;
    public int maxVertexCount = 20;
    public float pruneMinDelay = 1f;
    public GameObject center;

    List<Transform> vertices;
    float vertexScale = 0.1f;
    GameObject lastMergedBubble;
    
    float initialRadius;
    Vector3 initialHighlightScale;
    bool isVertexRescanNeeded = true;
    bool isUnscrambling = false;
    float pruneTimer = 0f;
    const float s_DistanceTolerance = 0.01f;//0.001f;
    public float wobbleTimer = 0f;

    void Awake()
    {
        vertices = new List<Transform>();
        initialRadius = radius;
        GenerateBubble();
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
        highlight.transform.parent = center.transform; // so that highlight moves with bubble
        initialHighlightScale = highlight.transform.localScale;
        Adjusthighlight();
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
        joint1.dampingRatio = centerSpringDamping;
        from.GetComponent<BubbleVertex>().jointToCenter = joint1;
    }

    void AddNeighborSrping(GameObject from, GameObject to) {
        SpringJoint2D joint2 = from.AddComponent<SpringJoint2D>();
        // joint2.enableCollision = true;
        joint2.connectedBody = to.GetComponent<Rigidbody2D>();
        joint2.autoConfigureDistance = true;
        joint2.frequency = neighborSpringFrequency;
        joint2.dampingRatio = neighborSpringDamping;
        from.GetComponent<BubbleVertex>().jointToNeighbor = joint2;
    }

    public void Merge(BubbleVertex a, BubbleVertex b) {
        GameObject otherBubble = b.gameObject.transform.parent.gameObject;
        if (lastMergedBubble == otherBubble) {
            return;
        }
        // Debug.Log("Merge bubbles! Collided vertices: " + a.gameObject.name + ", " + b.gameObject.name);
        lastMergedBubble = otherBubble;

        // Glue bubbles together
        ReJoint(a.nextVertex, b.prevVertex);
        ReJoint(b.nextVertex, a.prevVertex);
        Destroy(a.gameObject);
        Destroy(b.gameObject);
        isVertexRescanNeeded = true;

        Vector3 mergePoint = a.gameObject.transform.position;
        GetComponent<AirController>().AddAir();

        center.transform.position = mergePoint;
        ReparentToNewCenter(otherBubble);
        ReconnectToCenterAndIncreaseRadius(otherBubble.GetComponent<AutoBubble>().radius);
        Destroy(otherBubble);
        isVertexRescanNeeded = true;
        Adjusthighlight();

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
            isVertexRescanNeeded = true;
            // Debug.Log("Unscrambled " + vertex.name);
            return true;
        }
        return false;
    }

    void DeleteVertex(GameObject vertex) {
        var pos = vertex.transform.position;
        var prev = vertex.GetComponent<BubbleVertex>().prevVertex;
        var next = vertex.GetComponent<BubbleVertex>().nextVertex;
        ReJoint(next, prev);
        Destroy(vertex);
        isVertexRescanNeeded = true;
        // Debug.Log("Pruned " + vertex.name);
    }

    void ReparentToNewCenter(GameObject parent) {
        Destroy(parent.GetComponent<AutoBubble>().center);
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
        float newRadius = Mathf.Sqrt(Mathf.Pow(radius, 2) + Mathf.Pow(otherRadius, 2)); // 2D
        // float newRadius = Mathf.Pow(Mathf.Pow(radius, 3) + Mathf.Pow(otherRadius, 3), 1/3); // 3D
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
        wobbleTimer += Time.deltaTime;
        for (int i = 0; i < vertices.Count; i++) 
        {
            // Vector2 randomVec = UnityEngine.Random.insideUnitCircle;
            // ForceMode2D fmode = isWobbleImpulse ? ForceMode2D.Impulse : ForceMode2D.Force;
            // vertices[i].gameObject.GetComponent<Rigidbody2D>().AddForce(
            //     randomVec * wobbleStrength, fmode);
            
            var vertex = vertices[i].gameObject.GetComponent<BubbleVertex>();
            var x = wobbleTimer * wobbleSpeed;
            var shift = 2*Mathf.PI*i/(vertices.Count-1);
            vertex.jointToCenter.distance = radius +
                Mathf.Cos((x + shift) * wobbleFrequency) * wobbleAmplitude;
           
        }
    }

    void UpdateSkin() {

        if (isVertexRescanNeeded) {
            UpdateVertices();
            isVertexRescanNeeded = false;
        }
        
        skin.spline.Clear();
        int lastAddedIdx = -1;
        for (int i = 0, j = 0; i < vertices.Count; i++) 
        {
            if (lastAddedIdx >= 0) {
               var seg = vertices[lastAddedIdx].localPosition - vertices[i].localPosition;
               if (seg.sqrMagnitude < s_DistanceTolerance) {
                    // Debug.Log("Spline. Skip vertex");
                    continue;
               }
            }
            try {
                skin.spline.InsertPointAt(j, vertices[i].localPosition);
                skin.spline.SetTangentMode(j, ShapeTangentMode.Continuous);

                Vector2 radius = vertices[i].localPosition - center.transform.localPosition;
                Vector2 tangent = Vector2.Perpendicular(radius);

                skin.spline.SetLeftTangent(j, tangent.normalized * skin.spline.GetLeftTangent(j).magnitude);
                skin.spline.SetRightTangent(j, -tangent.normalized * skin.spline.GetRightTangent(j).magnitude);
                lastAddedIdx = i;
                j++;
            } catch (Exception ex) {
                // Debug.Log("Spline exception: " + ex);
            }
            
        }
    }

    void UpdateVertices() {
        vertices.Clear();
        GameObject firstVertex = null;
        // find first vertex child
        foreach (Transform t in transform) {
            if(t.gameObject.TryGetComponent<BubbleVertex>(out var vertex)) {
                if(vertex.prevVertex != null) {
                    firstVertex = vertex.gameObject;
                    break;
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
        if (radius <= minRadius) {
            // Debug.Log("Deflated to minimum radius");
            return;
        }

        radius -= Time.deltaTime * deflateSpeed;
        Adjusthighlight();
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
        
        // occasionaly remove vertices to keep them in limits
        PruneVertices();
    }

    void PruneVertices() {
        pruneTimer += Time.deltaTime;
        if (vertices.Count < maxVertexCount) {
            return;
        }
        if (pruneTimer > pruneMinDelay) {
            pruneTimer = 0;

            int j = UnityEngine.Random.Range(0, vertices.Count);
            DeleteVertex(vertices[j].gameObject);
        } 
        
    }

    void Adjusthighlight() {
        var vec = highlight.transform.localPosition.normalized * radius;
        highlight.transform.position = vec + center.transform.position;
        highlight.transform.localScale = initialHighlightScale * radius / initialRadius;
    }

    public void PushBubble(Vector2 dir) {
        foreach (var ver in vertices) {
            if(ver == null) {
                continue;
            }
            ver.GetComponent<Rigidbody2D>().AddForce(dir);
        }
    }

}