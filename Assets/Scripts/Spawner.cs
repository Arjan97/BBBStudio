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
    public float yPadding = 0.3f;

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
        GameObject rndObj = objectPool[Random.Range(0, objectPool.Length - 1)];
        float spriteShift = rndObj.GetComponent<SpriteRenderer>().bounds.size.y / 2 / rndObj.transform.localScale.x + yPadding;
        float randomY = isBuilding ?  -spriteShift : Random.Range(minY, maxY);

        Vector3 spawnPosition = new Vector3(Camera.main.transform.position.x + spawnXOffset, randomY, 0);

        GameObject spawnedObject = Instantiate(rndObj, spawnPosition, Quaternion.identity);
    }
}
