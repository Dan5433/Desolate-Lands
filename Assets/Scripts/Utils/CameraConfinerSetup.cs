using Cinemachine;
using UnityEngine;

public class CameraConfinerSetup : MonoBehaviour
{
    [SerializeField] TerrainManager terrainManager;
    [SerializeField] CinemachineConfiner confiner;

    void Awake()
    {
        var collider = GetComponent<PolygonCollider2D>();
        var worldSize = terrainManager.WorldSize;
        var chunkSize = TerrainManager.ChunkSize;

        Vector2[] points = {
            new(worldSize.x * chunkSize.x, worldSize.y * chunkSize.y),
            new(-worldSize.x * chunkSize.x, worldSize.y * chunkSize.y),
            new(-worldSize.x * chunkSize.x, -worldSize.y * chunkSize.y),
            new(worldSize.x * chunkSize.x, -worldSize.y * chunkSize.y)};

        collider.SetPath(0, points);

        confiner.InvalidatePathCache();
    }
}
