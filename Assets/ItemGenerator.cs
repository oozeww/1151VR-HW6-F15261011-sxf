using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject applePrefab;
    public GameObject bombPrefab;

    [Header("Grid Position Settings")]
    public float[] gridCoords = { -0.63f, 0f, 0.63f };
    public float spawnY = 5.0f;

    [Header("Spawn Settings")]
    public float spawnInterval = 1.0f;
    [Range(0f, 1f)] public float bombRatio = 0.3f;
    public float autoDestroyTime = 5.0f;

    private float timer;
    private bool isGenerating = true;

    void Update()
    {
        if (!isGenerating) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnItem();
        }
    }

    public void StopGenerating()
    {
        isGenerating = false;
    }

    void SpawnItem()
    {
        if (gridCoords == null || gridCoords.Length == 0) return;

        float spawnX = gridCoords[Random.Range(0, gridCoords.Length)];
        float spawnZ = gridCoords[Random.Range(0, gridCoords.Length)];
        Vector3 spawnPos = new Vector3(spawnX, spawnY, spawnZ);

        GameObject prefabToSpawn = (Random.value < bombRatio) ? bombPrefab : applePrefab;

        if (prefabToSpawn != null)
        {
            GameObject item = Instantiate(prefabToSpawn, spawnPos, prefabToSpawn.transform.rotation);
            if (autoDestroyTime > 0f)
                Destroy(item, autoDestroyTime);
        }
    }
}