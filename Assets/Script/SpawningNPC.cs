using UnityEngine;

public class SpawningNPC : MonoBehaviour
{
    [Header("NPC Spawning Settings")]
    public GameObject npcPrefab;
    public Transform[] pathPoints;

    public float minSpawnInterval = 15f;
    public float maxSpawnInterval = 25f;

    public float spawnRadius = 1.5f;

    [Header("NPC Limit")]
    public int maxNPCs = 5;       // Maximum NPCs allowed
    private int currentNPCCount = 0;

    private float spawnTimer = 0f;
    private float currentSpawnInterval;

    void Start()
    {
        currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void Update()
    {
        // Stop updating if we reached the limit
        if (currentNPCCount >= maxNPCs)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= currentSpawnInterval)
        {
            SpawnNPC();
            spawnTimer = 0f;
            currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    void SpawnNPC()
    {
        // Double-check limit here too
        if (currentNPCCount >= maxNPCs)
            return;

        if (npcPrefab != null && pathPoints.Length > 0)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0,
                Random.Range(-spawnRadius, spawnRadius)
            );

            Vector3 spawnPos = pathPoints[0].position + randomOffset;

            GameObject newNPC = Instantiate(npcPrefab, spawnPos, Quaternion.identity);

            currentNPCCount++;  // Increase NPC count

            NPCPathfinding npcPathfinding = newNPC.GetComponent<NPCPathfinding>();
            if (npcPathfinding != null)
                npcPathfinding.pathPoints = pathPoints;

            var agent = newNPC.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.avoidancePriority = Random.Range(10, 99);
                agent.stoppingDistance = 0.5f;
            }
        }
    }
}
