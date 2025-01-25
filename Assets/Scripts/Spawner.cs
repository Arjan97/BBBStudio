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

    private float nextSpawnTime;

    private int lastRnd;

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnObject();
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    private void SpawnObject()
    {
        int newRnd = Random.Range(0, objectPool.Length - 1);
        while (objectPool.Length > 1 && newRnd == lastRnd) {
            newRnd = Random.Range(0, objectPool.Length - 1);
        }
        GameObject rndObj = objectPool[newRnd];
        lastRnd = newRnd;
        float y = isBuilding ?  buildingY : Random.Range(minY, maxY);

        Vector3 spawnPosition = new Vector3(Camera.main.transform.position.x + spawnXOffset, y, 0);

        GameObject spawnedObject = Instantiate(rndObj, spawnPosition, Quaternion.identity);
    }
}
