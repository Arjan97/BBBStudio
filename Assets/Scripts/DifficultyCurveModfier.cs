using System.Collections.Generic;
using UnityEngine;

public class DifficultyCurveModfier : MonoBehaviour
{
    public float internalClock;
    public List<float> curvePoints;
    private int totalCurvePoints, currentCurvePoint;
    public AnimationCurve difficultyCurve;
    public AnimationCurve bubbleCurve;

    public Spawner bubbleSpawner;
    public List<Spawner> enemySpawners;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalCurvePoints = curvePoints.Count;
        currentCurvePoint = 0;
    }

    // Update is called once per frame
    void Update()
    {
        internalClock += Time.deltaTime;

        if (currentCurvePoint < totalCurvePoints)
        {
            if (internalClock >= curvePoints[currentCurvePoint])
            {
                bubbleSpawner.maxSpawnInterval /= bubbleCurve.Evaluate(currentCurvePoint);
                foreach(Spawner enemySpawner in enemySpawners)
                    enemySpawner.maxSpawnInterval /= difficultyCurve.Evaluate(currentCurvePoint);
                currentCurvePoint++;
            }
        }
    }
}
