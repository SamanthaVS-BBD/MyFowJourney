using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SmoothColliderAdder2D : MonoBehaviour
{
    public float minimumVertexDistance = 0.1f;

    void Start()
    {
        Debug.Log("SmoothColliderAdder2D script started.");
        UpdateColliders();
    }

    void Awake()
    {
        UpdateColliders();
    }

    void UpdateColliders()
    {
        GameObject[] groundObjects = GameObject.FindGameObjectsWithTag("ground");
        GameObject[] gapGroundObjects = GameObject.FindGameObjectsWithTag("gapGround");

        GameObject[] allGroundObjects = groundObjects.Concat(gapGroundObjects).ToArray();

        Debug.Log($"Found {allGroundObjects.Length} objects tagged as 'Ground'");

        if (allGroundObjects.Length == 0)
        {
            Debug.LogWarning("No objects found with 'Ground' tag.");
        }

        foreach (GameObject groundObject in allGroundObjects)
        {
            if (groundObject == null) continue;

            PolygonCollider2D existingCollider = groundObject.GetComponent<PolygonCollider2D>();
            if (existingCollider != null)
            {
                Debug.Log($"Removing existing PolygonCollider2D from {groundObject.name}");
                Destroy(existingCollider);
            }

            PolygonCollider2D polygonCollider = groundObject.AddComponent<PolygonCollider2D>();

            SpriteRenderer spriteRenderer = groundObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Vector2[] colliderPath = GenerateSmoothColliderPath(spriteRenderer.sprite);
                if (colliderPath.Length > 0)
                {
                    polygonCollider.SetPath(0, colliderPath);
                    Debug.Log($"Collider set for {groundObject.name} with {colliderPath.Length} points");
                }
                else
                {
                    Debug.LogWarning($"No valid collider path generated for {groundObject.name}");
                }
            }
            else
            {
                Debug.LogWarning($"No SpriteRenderer found on {groundObject.name}");
            }
        }

    }

    Vector2[] GenerateSmoothColliderPath(Sprite sprite)
    {
        Texture2D texture = sprite.texture;

        if (texture == null)
        {
            Debug.LogError($"Texture is null for sprite {sprite.name}");
            return new Vector2[0];
        }

        if (!texture.isReadable)
        {
            return new Vector2[0];
        }

        Rect rect = sprite.rect;

        List<Vector2> path = new List<Vector2>();
        Color[] pixels = texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);

        if (pixels.Length == 0)
        {
            Debug.LogError($"No pixels retrieved from texture '{texture.name}'.");
            return new Vector2[0];
        }

        for (int y = 0; y < rect.height; y++)
        {
            for (int x = 0; x < rect.width; x++)
            {
                if (IsEdgePixel(pixels, x, y, (int)rect.width, (int)rect.height))
                {
                    Vector2 newPoint = new Vector2(
                        (x + rect.x) / texture.width,
                        (y + rect.y) / texture.height
                    );

                    if (path.Count == 0 || Vector2.Distance(path[path.Count - 1], newPoint) > minimumVertexDistance)
                    {
                        path.Add(newPoint);
                    }
                }
            }
        }

        if (path.Count > 2 && path[0] != path[path.Count - 1])
        {
            path.Add(path[0]);
        }

        return path.ToArray();
    }

    bool IsEdgePixel(Color[] pixels, int x, int y, int width, int height)
    {
        Color pixelColor = pixels[y * width + x];
        if (pixelColor.a == 0) return false;

        bool isEdge = false;

        if (x > 0 && pixels[y * width + (x - 1)].a == 0) isEdge = true;
        if (x < width - 1 && pixels[y * width + (x + 1)].a == 0) isEdge = true;
        if (y > 0 && pixels[(y - 1) * width + x].a == 0) isEdge = true;
        if (y < height - 1 && pixels[(y + 1) * width + x].a == 0) isEdge = true;

        return isEdge;
    }
}