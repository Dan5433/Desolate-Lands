using CustomClasses;
using CustomExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SaveTerrain))]
[RequireComponent(typeof(LoadTerrain))]
public class TerrainManager : MonoBehaviour
{
    [SerializeField] TileBase[] masterTiles;
    [SerializeField] Tilemap ground, top, solid;
    [SerializeField] StructureGroup[] structures;
    [SerializeField] WeightedTileById[] grTiles;
    [SerializeField] TerrainGenTiles[] topTiles;
    [SerializeField] Transform player;
    [SerializeField] Vector2Int worldSize;
    [SerializeField] Vector2Int structureMargin;
    [SerializeField][Tooltip("In Chunks")] int renderDist;

    WorldBorderManager borderManager;
    LoadTerrain loadTerrain;
    SaveTerrain saveTerrain;
    HashSet<Vector2Int> renderedChunks = new();
    Dictionary<TileBase, int> tileLookup;
    static Vector2Int chunkSize = new(32, 32);
    static Vector2Int regionSize = new(32, 32);
    static readonly string dataDirName = "regions";

    public TileBase[] MasterTiles => masterTiles;
    public Dictionary<TileBase,int> TileLookup => tileLookup;
    public static Vector2Int ChunkSize => chunkSize;
    public static Vector2Int RegionSize => regionSize;
    public static string DataDirName => dataDirName;
    public Vector2Int WorldSize => worldSize;

    void Awake()
    {
        loadTerrain = GetComponent<LoadTerrain>();
        saveTerrain = GetComponent<SaveTerrain>();
        borderManager = GetComponent<WorldBorderManager>();
        tileLookup = masterTiles.Select((tile, index) => new { tile, index }).ToDictionary(x => x.tile, x => x.index);
    }

    private void Start()
    {
        //0 second delay causes the compress to happen after player is loaded (idk why)
        Invoke(nameof(CompressOnStart),0);
    }

    void CompressOnStart()
    {
        CompressTilemaps(GetChunkIndex(player.position), ground, top, solid, BreakingManager.Instance.Tilemap);
    }

    LinkedList<Vector2Int> GetChunksInsideRenderDistance(Vector2Int currentChunk)
    {
        LinkedList<Vector2Int> rendered = new();
        for (int x = currentChunk.x - renderDist; x <= currentChunk.x + renderDist; x++)
        {
            for (int y = currentChunk.y - renderDist; y <= currentChunk.y + renderDist; y++)
            {
                rendered.AddLast(new Vector2Int(x, y));
            }
        }
        return rendered;
    }

    void CompressTilemaps(Vector2Int currentChunkIndex, params Tilemap[] tilemaps)
    {
        foreach (var tilemap in tilemaps)
        {
            tilemap.origin = (Vector3Int)(currentChunkIndex * chunkSize - chunkSize);
            tilemap.size = new(chunkSize.x * (renderDist * 2 + 1), chunkSize.y * (renderDist * 2 + 1), 1);
            tilemap.ResizeBounds();

            tilemap.gameObject.SetActive(false);
            tilemap.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        //TODO: only save user change data to tiles and generate chunks using the seed each time
        //treat previously saved region data as user modified to make data persistent after seed update
        bool shrunkTilemap = false;
        var currentChunk = GetChunkIndex(player.position);
        var chunksInRender = GetChunksInsideRenderDistance(currentChunk);

        foreach (var chunk in renderedChunks.ToArray())
        {
            if (chunksInRender.Contains(chunk)) 
                continue;

            //unload chunk if not in render distance but is loaded
            borderManager.UnloadBorder(chunk);

            renderedChunks.Remove(chunk);

            shrunkTilemap = true;
        }

        Vector2Int currRegion = default;
        Dictionary<Vector2Int, TilemapChunkNodesData> parsedChunks = null;

        foreach (var chunk in chunksInRender)
        {
            if (!renderedChunks.Add(chunk) || !ChunkInsideWorldBorder(chunk)) 
                continue;

            //load chunk if in render distance and not loaded already
            Vector2Int region = GetRegionIndex(chunk);

            string fullPath = Path.Combine(GameManager.DataDirPath, DataDirName, region.ToString());
            if (File.Exists(fullPath) && (parsedChunks == null || region != currRegion))
            {
                parsedChunks = loadTerrain.ParseRegionFile(region, chunksInRender, ground, top, solid);
                currRegion = region;
            }

            if (parsedChunks == null || !parsedChunks.ContainsKey(chunk))
            {
                Debug.Log("gen new chunk");
                GenChunk(chunk * chunkSize, chunk, region);
            }
            else
            {
                foreach (var tilemapNodesPair in parsedChunks[chunk].data)
                {
                    loadTerrain.SetTilemapChunk(
                        tilemapNodesPair.Key, tilemapNodesPair.Value,
                        chunk.x * ChunkSize.x, chunk.y * ChunkSize.y);
                }
            }

            LoadBorderIfEndOfWorld(chunk);
        }

        if (shrunkTilemap) 
            CompressTilemaps(currentChunk, ground, top, solid, BreakingManager.Instance.Tilemap);
    }

    bool ChunkInsideWorldBorder(Vector2Int chunk)
    {
        return chunk.x < worldSize.x && chunk.x >= -worldSize.x && chunk.y < worldSize.y && chunk.y >= -worldSize.y;
    }

    void LoadBorderIfEndOfWorld(Vector2Int chunk)
    {
        if (chunk.x == worldSize.x - 1 || chunk.x == -worldSize.x)
        {
            if (Mathf.Sign(chunk.x) == -1)
            {
                borderManager.LoadBorder(chunk, Direction.Right);
            }
            else
            {
                borderManager.LoadBorder(chunk, Direction.Left);
            }
        }

        if (chunk.y == worldSize.y - 1 || chunk.y == -worldSize.y)
        {
            if (Mathf.Sign(chunk.y) == -1)
            {
                borderManager.LoadBorder(chunk, Direction.Up);
            }
            else
            {
                borderManager.LoadBorder(chunk, Direction.Down);
            }
        }
    }

    bool HasRoomToPlace(Tilemap tilemap, BoundsInt bounds)
    {
        bounds.position -= (Vector3Int)structureMargin;
        bounds.size += (Vector3Int)structureMargin*2;
        foreach (var tile in tilemap.GetTilesBlock(bounds))
        {
            if(tile != null)
            {
                return false;
            }
        }
        return true;
    }

    void GenChunk(Vector2Int tilePos, Vector2Int chunkIndex, Vector2Int region)
    {
        //lags the game horribly
        //refactor settiles (which lags the most) to use tile change data instead
        GenBoxTiles(ground, grTiles, tilePos);
        foreach (var genTiles in topTiles)
        {
            GenGriddedTiles(top, genTiles.Tiles, genTiles.GenGap, tilePos);
        }

        GenStructures(tilePos, solid);

        saveTerrain.SaveTiles(region, chunkIndex, ground, top, solid);
    }

    void GenStructures(Vector2Int startPos, Tilemap tilemap)
    {
        Vector2Int endPos = startPos + chunkSize;

        foreach(var structureGroup in structures)
        {
            for (int i = 0; i < structureGroup.rollsPerChunk; i++)
            {
                var structure = WeightedUtils.RollStructure(structureGroup.structures);

                if (structure == null) 
                    continue;

                Vector3Int spawnPosition = new(Random.Range(startPos.x, endPos.x), Random.Range(startPos.y, endPos.y));

                var bounds = structure.cellBounds;
                bounds.position = spawnPosition;

                if (!HasRoomToPlace(tilemap, bounds)) 
                    continue;

                tilemap.SetTilesBlock(bounds, structure.GetAllTiles());
            }
        }
    }

    void GenBoxTiles(Tilemap tilemap, WeightedTileById[] tilePool, Vector2Int startPos)
    {
        Vector2Int endPos = startPos + chunkSize;
        Vector3Int[] positions = new Vector3Int[chunkSize.x * chunkSize.y];
        TileBase[] tiles = new TileBase[positions.Length];

        int index = 0;

        for (int x = startPos.x; x < endPos.x; x++)
        {
            for (int y = startPos.y; y < endPos.y; y++)
            {
                positions[index] = new Vector3Int(x, y);

                tiles[index] = WeightedUtils.RollTile(tilePool, masterTiles);

                index++;
            }
        }

        tilemap.SetTiles(positions, tiles);
    }

    void GenGriddedTiles(Tilemap tilemap, WeightedTileById[] tilePool, int distance, Vector2Int startPos)
    {
        //using grid partitioning algorithm
        Vector2Int endPos = startPos + chunkSize;

        Vector2Int gridSize = new(chunkSize.x / distance + 1, chunkSize.y / distance + 1);

        Vector3Int[] positions = new Vector3Int[gridSize.x * gridSize.y];
        TileBase[] tiles = new TileBase[positions.Length];

        Debug.Log("Array size: " + positions.Length);

        //split chunk into cells based on distance
        int index = 0;
        for (int x = startPos.x; x < endPos.x; x += distance)
        {
            for (int y = startPos.y; y < endPos.y; y += distance)
            {
                Vector3Int position = new(
                    Mathf.Clamp(Random.Range(x, x + distance),startPos.x,endPos.x),
                    Mathf.Clamp(Random.Range(y, y + distance), startPos.y, endPos.y));

                positions[index] = position;
                tiles[index] = WeightedUtils.RollTile(tilePool, masterTiles);
                index++;
            }
        }

        tilemap.SetTiles(positions, tiles);
    }
    public static Vector2Int GetRegionIndex(Vector2Int chunk)
    {
        Vector2Int region = new(chunk.x / RegionSize.x, chunk.y / RegionSize.y);

        if (chunk.x < 0)
            region.x -= 1;
        if(chunk.y < 0)
            region.y -= 1;

        return region;
    }

    public static Vector2Int GetChunkIndex(Vector3 position)
    {
        Vector2Int chunk = new();

        if (position.x < 0) chunk.x = Mathf.FloorToInt(position.x / chunkSize.x);
        else chunk.x = Mathf.CeilToInt(position.x / chunkSize.x) - 1;

        if (position.y < 0) chunk.y = Mathf.FloorToInt(position.y / chunkSize.y);
        else chunk.y = Mathf.CeilToInt(position.y / chunkSize.y) - 1;

        return chunk;
    }

    public static Vector2Int GetChunkIndex(Vector3Int position)
    {
        Vector2Int chunk = new(position.x / RegionSize.x, position.y / RegionSize.y);

        if (position.x < 0)
            chunk.x -= 1;
        if (position.y < 0)
            chunk.y -= 1;

        return chunk;
    }
}

[Serializable]
public struct TerrainGenTiles
{
    [SerializeField] WeightedTileById[] tiles;
    [SerializeField] int genGap;

    public WeightedTileById[] Tiles => tiles;
    public int GenGap => genGap;
}

[Serializable]
public struct StructureGroup
{
    [SerializeField] public WeightedStructure[] structures;
    [SerializeField] public int rollsPerChunk;
}