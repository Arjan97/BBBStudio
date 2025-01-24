using UnityEngine;
using UnityEngine.U2D;

public class SoftBody : MonoBehaviour
{
    public Transform[] points;
    public SpriteShapeController skin;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdatePoints();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePoints();
    }

    void UpdatePoints()
    {
        for (int i = 0; i < points.Length; i++) {
            
            skin.spline.SetPosition(i, points[i].localPosition);

            Vector2 radius = points[i].localPosition;
            Vector2 tangent = Vector2.Perpendicular(radius);

            skin.spline.SetLeftTangent(i, tangent.normalized * skin.spline.GetLeftTangent(i).magnitude);
            skin.spline.SetRightTangent(i, -tangent.normalized * skin.spline.GetRightTangent(i).magnitude);
        }
    }
}
