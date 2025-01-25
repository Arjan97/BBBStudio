using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject objectToSpawn; 
    public float minSpawnInterval = 1f; 
    public float maxSpawnInterval = 3f; 
    public float minY = -5f; 
    public float maxY = 5f; 
    public float spawnXOffset = 10f;
    public bool isBuilding = false;
    public float fixedY = 0f;

    private float nextSpawnTime;

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
        float randomY = isBuilding ? fixedY : Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(Camera.main.transform.position.x + spawnXOffset, randomY, 0);

        GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
    }
}
