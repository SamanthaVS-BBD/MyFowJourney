using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testJoin : MonoBehaviour
{
      public GameObject[] prefabs;  // Array of prefabs to spawn
    public float spacing = 0f;    // Space between prefabs (if needed)

    private void Start()
    {
        SpawnPrefabs();
    }

    void SpawnPrefabs()
    {
        if (prefabs.Length == 0)
        {
            Debug.LogError("No prefabs assigned.");
            return;
        }

        Vector3 spawnPosition = Vector3.zero;

        for (int i = 0; i < prefabs.Length; i++)
        {
            GameObject prefab = prefabs[i];
            GameObject newPrefab = Instantiate(prefab, spawnPosition, Quaternion.identity);

            // Get the SpriteRenderer component
            SpriteRenderer spriteRenderer = newPrefab.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("Prefab does not have a SpriteRenderer.");
                continue;
            }

            // Get the sprite bounds in world units
            Bounds spriteBounds = spriteRenderer.bounds;

            Vector3 topLeftCorner = new Vector3(spriteBounds.min.x, spriteBounds.max.y, spriteBounds.center.z);

            spawnPosition = new Vector3(topLeftCorner.x + spriteBounds.size.x + spacing, topLeftCorner.y, topLeftCorner.z);

        }
    }
}
