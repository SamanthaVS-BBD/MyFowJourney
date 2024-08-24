using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Transform[] chunkPrefabs;
    public Transform initialChunk;
    private Transform lastChunk;
    public GameObject[] obstaclePrefab;
    private GameObject player;
    public float spawnDistance = 10f;
    public float removeDistance = 20f;
    private List<Transform> activeChunks = new List<Transform>();

    void Start()
    {
        if (initialChunk == null) {
            Debug.LogError("InitialChunk is not set!");
            return;
        }
    
        lastChunk = initialChunk;
        activeChunks.Add(lastChunk);
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null) {
            Debug.LogError("Player not found!");
        }
    }

    void Update()
    {
        if (lastChunk != null) {
            float playerX = player.transform.position.x;
            float endOfCurrentChunkX = lastChunk.Find("LowestPoint").position.x - spawnDistance;

            if (playerX >= endOfCurrentChunkX)
            {
                SpawnNextChunk();
                RemoveOldChunks();
            }
        } else {
            Debug.LogWarning("LastChunk is null.");
        }
    }

    public void SpawnObstacle(Transform newChunk)
    {
        int spawnChance = Random.Range(0, 7);
        if (spawnChance == 0 || spawnChance == 1) 
        {
            Debug.Log("No obstacle spawned, chance: " + spawnChance);
            return;
        }
        StartCoroutine(DelayedSpawnObstacle(newChunk));
    }

    private IEnumerator DelayedSpawnObstacle(Transform newChunk)
    {
        yield return new WaitForSeconds(0.5f); // Wait for half a second

        Vector2 firstChunkPos = newChunk.Find("HighestPoint").position;
        Vector2 lastChunkPos = newChunk.Find("LowestPoint").position;

        float obsX = Random.Range(firstChunkPos.x + 2, lastChunkPos.x - 2);

        Vector2 raycastOrigin = new Vector2(obsX, firstChunkPos.y + 10);

        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down * 50);
        Debug.DrawRay(raycastOrigin, Vector2.down * 50, Color.red, 2f);

        if (hit.collider != null)
        {
            int randomNum = Random.Range(0, obstaclePrefab.Length + 1 );
            GameObject newObstacle = Instantiate(obstaclePrefab[randomNum]);
            newObstacle.transform.position = hit.point;
            newObstacle.transform.up = hit.normal;
            newObstacle.transform.SetParent(newChunk);
            Debug.Log("Obstacle spawned at: " + hit.point);
        }
        else
        {
            Debug.LogWarning("Failed to find ground for obstacle placement. Raycast Origin: " + raycastOrigin);
        }
    }

    public void SpawnNextChunk()
    {
        Transform newChunk = Instantiate(chunkPrefabs[Random.Range(0, chunkPrefabs.Length)]);
        ConnectChunks(lastChunk, newChunk);
        SpawnObstacle(newChunk);
        lastChunk = newChunk;
        activeChunks.Add(newChunk);
    }

    private void ConnectChunks(Transform previousChunk, Transform newChunk)
    {
        Transform previousLowest = previousChunk.Find("LowestPoint");
        Transform newHighest = newChunk.Find("HighestPoint");

        if (previousLowest != null && newHighest != null)
        {
            Vector3 offset = previousLowest.position - newHighest.position;
            //newChunk.position += offset;
            newChunk.position = previousLowest.position;
            Debug.Log($"Connected chunks. Previous Lowest: {previousLowest.position}, New Highest: {newHighest.position}, Offset: {offset}");
        }
        else
        {
            Debug.LogWarning("Missing HighestPoint or LowestPoint in ConnectChunks.");
        }
    }

    private void RemoveOldChunks()
    {
        for (int i = activeChunks.Count - 1; i >= 0; i--)
        {
            Transform chunk = activeChunks[i];
            if (chunk != null)
            {
                Transform lowestPoint = chunk.Find("LowestPoint");
                if (lowestPoint != null)
                {
                    float distance = player.transform.position.x - lowestPoint.position.x;

                    if (distance > removeDistance)
                    {
                        Destroy(chunk.gameObject);
                        activeChunks.RemoveAt(i);
                        Debug.Log("Removed old chunk: " + chunk.name);
                    }
                }
                else
                {
                    Debug.LogWarning("LowestPoint not found in chunk: " + chunk.name);
                }
            }
            else
            {
                Debug.LogWarning("Active chunk reference is null.");
                activeChunks.RemoveAt(i);
            }
        }
    }
}