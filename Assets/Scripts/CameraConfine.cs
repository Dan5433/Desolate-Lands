using Cinemachine;
using UnityEngine;

public class CameraConfine : MonoBehaviour
{
    [SerializeField] TerrainManager genTerrain;
    [SerializeField] CinemachineConfiner confiner;

    void Awake()
    {
        var collider = GetComponent<PolygonCollider2D>();
        var worldSize = genTerrain.WorldSize;
        var chunkSize = genTerrain.ChunkSize;

        Vector2[] points = {
            new Vector2(worldSize.x * chunkSize.x, worldSize.y * chunkSize.y),
            new Vector2(-worldSize.x * chunkSize.x, worldSize.y * chunkSize.y),
            new Vector2(-worldSize.x * chunkSize.x, -worldSize.y * chunkSize.y),
            new Vector2(worldSize.x * chunkSize.x, -worldSize.y * chunkSize.y)};
        
        collider.SetPath(0,points);

        //confiner.InvalidatePathCache();
    }
}
