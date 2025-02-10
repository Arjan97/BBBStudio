using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SoftBody : MonoBehaviour
{
    public SpriteShapeController skin;

    public float wobbleStrength = 0.1f;


    List<Transform> points;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        points = new List<Transform>();
        foreach (Transform obj in transform) {
            if (obj.tag == "BubblePoint") {
                points.Add(obj);
            }
        }
        UpdatePoints();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePoints();
        Wobble();
    }

    void UpdatePoints()
    {
        for (int i = 0; i < points.Count; i++) 
        {
            try 
            {
                skin.spline.SetPosition(i, points[i].localPosition);
            } catch{}
            

            Vector2 radius = points[i].localPosition;
            Vector2 tangent = Vector2.Perpendicular(radius);

            skin.spline.SetLeftTangent(i, tangent.normalized * skin.spline.GetLeftTangent(i).magnitude);
            skin.spline.SetRightTangent(i, -tangent.normalized * skin.spline.GetRightTangent(i).magnitude);
        }
    }

    void Wobble() 
    {
        for (int i = 0; i < points.Count; i++) 
        {
            Vector2 randomVec = Random.insideUnitCircle;
            points[i].gameObject.GetComponent<Rigidbody2D>().AddForce(randomVec * wobbleStrength);
        }
    }
}
