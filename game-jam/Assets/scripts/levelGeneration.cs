using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class levelGeneration : MonoBehaviour
{
    public Transform[] chunkPrefabs;
    public Transform initialChunk;
    private Transform lastChunk;
    public GameObject obstaclePrefab;
    private GameObject player;
    public float spawnDistance = 10f;
    public float removeDistance = 20f;
    private List<Transform> activeChunks = new List<Transform>();

    void Start()
    {
        if (initialChunk == null)
        {
            return;
        }

        lastChunk = initialChunk;
        activeChunks.Add(lastChunk);

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }
    }

    void Update()
    {
        if (lastChunk != null && player != null)
        {
            float playerX = player.transform.position.x;
            float endOfCurrentChunkX = GetLowestPointX(lastChunk) - spawnDistance;

            if (playerX >= endOfCurrentChunkX)
            {
                SpawnNextChunk();
                RemoveOldChunks();
            }
        }
    }

    public void SpawnObstacle(Transform newChunk)
    {
        int spawnChance = Random.Range(0, 4);
        if (spawnChance == 3 || spawnChance == 2) 
        {
            StartCoroutine(DelayedSpawnObstacle(newChunk));
        }
        
    }

    private IEnumerator DelayedSpawnObstacle(Transform newChunk)
    {
        yield return new WaitForSeconds(0.5f); // Wait for half a second

        Vector2 firstChunkPos = newChunk.Find("HighestPoint").position;
        Vector2 lastChunkPos = newChunk.Find("LowestPoint").position;

        float obsX = Random.Range(firstChunkPos.x + 2, lastChunkPos.x - 2);

        Vector2 raycastOrigin = new Vector2(obsX, firstChunkPos.y + 10);

        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down * 50);

        if (hit.collider != null)
        {
            GameObject newObstacle = Instantiate(obstaclePrefab);
            newObstacle.transform.position = hit.point;
            newObstacle.transform.up = hit.normal;
            newObstacle.transform.SetParent(newChunk);
        }
    }

    public void SpawnNextChunk()
    {
        if (chunkPrefabs.Length == 0)
        {
            return;
        }

        Transform newChunk = Instantiate(chunkPrefabs[Random.Range(0, chunkPrefabs.Length)]);

        ConnectChunks(lastChunk, newChunk);
        SpawnObstacle(newChunk);

        lastChunk = newChunk;
        activeChunks.Add(newChunk);
    }

    private void ConnectChunks(Transform previousChunk, Transform newChunk)
    {
        Vector2 previousLowestPoint = GetLowestPoint(previousChunk);
        Vector2 newHighestPoint = GetHighestPoint(newChunk);

        Vector3 offset = (Vector3)previousLowestPoint - (Vector3)newHighestPoint;
        newChunk.position += offset;
    }

    private void RemoveOldChunks()
    {
        for (int i = activeChunks.Count - 1; i >= 0; i--)
        {
            Transform chunk = activeChunks[i];
            if (chunk != null)
            {
                float distance = player.transform.position.x - GetLowestPointX(chunk);

                if (distance > removeDistance)
                {
                    Destroy(chunk.gameObject);
                    activeChunks.RemoveAt(i);
                }
            }
            else
            {
                activeChunks.RemoveAt(i);
            }
        }
    }

    private Vector2 GetHighestPoint(Transform chunk)
    {
        PolygonCollider2D collider = chunk.GetComponent<PolygonCollider2D>();
        if (collider == null)
        {
            return Vector2.zero;
        }

        Vector2 highestPoint = chunk.TransformPoint(collider.points[0]);
        foreach (Vector2 point in collider.points)
        {
            Vector2 worldPoint = chunk.TransformPoint(point);
            if (worldPoint.x < highestPoint.x || (worldPoint.x == highestPoint.x && worldPoint.y > highestPoint.y))
            {
                highestPoint = worldPoint;
            }
        }
        return highestPoint;
    }

    private Vector2 GetLowestPoint(Transform chunk)
    {
        PolygonCollider2D collider = chunk.GetComponent<PolygonCollider2D>();
        if (collider == null)
        {
            return Vector2.zero;
        }

        Vector2 lowestPoint = chunk.TransformPoint(collider.points[0]);
        foreach (Vector2 point in collider.points)
        {
            Vector2 worldPoint = chunk.TransformPoint(point);
            if (worldPoint.x > lowestPoint.x || (worldPoint.x == lowestPoint.x && worldPoint.y > lowestPoint.y))
            {
                lowestPoint = worldPoint;
            }
        }
        return lowestPoint;
    }

    private float GetLowestPointX(Transform chunk)
    {
        return GetLowestPoint(chunk).x;
    }
}
