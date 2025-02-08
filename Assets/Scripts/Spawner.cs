using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] objectPool;
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 3f;
    public float minY = -5f;
    public float maxY = 5f;
    public float spawnXOffset = 10f;
    public bool isBuilding = false;
    public float buildingY = -5.2f;

    [Header("Score Requirement")]
    public int spawnFromScore = 0;

    private float nextSpawnTime;
    private int lastRnd;
    private bool isActive = false;

    private void Update()
    {
        if (!isActive && ScoreManager.Instance != null && ScoreManager.Instance.GetScore() >= spawnFromScore)
        {
            isActive = true;
        }

        if (isActive && Time.time >= nextSpawnTime)
        {
            SpawnObject();
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    private void SpawnObject()
    {
        if (objectPool.Length == 0) return;

        int newRnd = Random.Range(0, objectPool.Length);
        while (objectPool.Length > 1 && newRnd == lastRnd)
        {
            newRnd = Random.Range(0, objectPool.Length);
        }
        GameObject rndObj = objectPool[newRnd];
        lastRnd = newRnd;
        float y = isBuilding ? buildingY : Random.Range(minY, maxY);

        Vector3 spawnPosition = new Vector3(Camera.main.transform.position.x + spawnXOffset, y, 0);

        if (rndObj.CompareTag("Shark")) 
        {
            WarningManager warningManager = FindFirstObjectByType<WarningManager>();
            if (warningManager != null)
            {
                warningManager.ShowWarning(spawnPosition);
            }
        }
        Instantiate(rndObj, spawnPosition, Quaternion.identity);
    }
}
