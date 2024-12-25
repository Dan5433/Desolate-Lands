using CustomClasses;
using CustomExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
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
    static Vector2Int chunkSize = new(32, 32);
    static Vector2Int regionSize = new(32, 32);
    static readonly string dataDirName = "regions";

    public TileBase[] MasterTiles => masterTiles;
    public static Vector2Int ChunkSize => chunkSize;
    public static Vector2Int RegionSize => regionSize;
    public static string DataDirName => dataDirName;
    public Vector2Int WorldSize => worldSize;

    void Awake()
    {
        loadTerrain = GetComponent<LoadTerrain>();
        saveTerrain = GetComponent<SaveTerrain>();
        borderManager = GetComponent<WorldBorderManager>(); 
    }

    private void Start()
    {
        //0 second delay causes the compress to happen after player is loaded (idk why)
        Invoke(nameof(CompressOnStart),0);
    }

    void CompressOnStart()
    {
        CompressTilemaps(GetChunkIndexFromPosition(player.position), ground, top, solid, BreakingManager.Instance.Tilemap);
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
        var currentChunk = GetChunkIndexFromPosition(player.position);
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

        HashSet<Vector2Int> parsedRegions = new();
        Dictionary<Vector2Int, TilemapChunkNodesData> parsedChunks = null;

        foreach (var chunk in chunksInRender)
        {
            if (!renderedChunks.Add(chunk) || !ChunkInsideWorldBorder(chunk)) 
                continue;

            //load chunk if in render distance and not loaded already
            Vector2Int region = new(chunk.x / RegionSize.x,
                chunk.y / RegionSize.y);

            string fullPath = Path.Combine(GameManager.DataDirPath, DataDirName, region.ToString());
            if (File.Exists(fullPath) && !parsedRegions.Contains(region))
            {
                parsedChunks = loadTerrain.ParseRegionFile(region, chunksInRender, ground, top, solid);
                parsedRegions.Add(region);
            }

            if (!parsedChunks.ContainsKey(chunk))
            {
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

    bool HasRoomToPlaceStructure(Tilemap tilemap, BoundsInt structureBounds)
    {
        structureBounds.position -= (Vector3Int)structureMargin;
        structureBounds.size += (Vector3Int)structureMargin*2;
        foreach (var tile in tilemap.GetTilesBlock(structureBounds))
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
        GenBoxTiles(ground, grTiles, tilePos);
        foreach (var genTiles in topTiles)
        {
            GenScatteredTiles(top, genTiles.Tiles, genTiles.GenGap, tilePos);
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

                if (structure == null) continue;

                Vector3Int spawnPosition = new(Random.Range(startPos.x, endPos.x), Random.Range(startPos.y, endPos.y));

                var bounds = structure.cellBounds;
                bounds.position = spawnPosition;

                if (!HasRoomToPlaceStructure(tilemap, bounds)) continue;

                tilemap.SetTilesBlock(bounds, structure.GetAllTiles());
            }
        }
    }

    void GenBoxTiles(Tilemap tilemap, WeightedTileById[] tiles, Vector2Int startPos)
    {
        Vector2Int endPos = startPos + chunkSize;
        Vector3Int[] genCoords = new Vector3Int[chunkSize.x * chunkSize.y];
        TileBase[] genTiles = new TileBase[genCoords.Length];

        int index = 0;

        for (int x = startPos.x; x < endPos.x; x++)
        {
            for (int y = startPos.y; y < endPos.y; y++)
            {
                genCoords[index] = new Vector3Int(x, y);

                genTiles[index] = WeightedUtils.RollTile(tiles, masterTiles);

                index++;
            }
        }

        tilemap.SetTiles(genCoords, genTiles);
    }

    void GenScatteredTiles(Tilemap tilemap, WeightedTileById[] tiles, int distance, Vector2Int startPos)
    {
        Vector2Int endPos = startPos + chunkSize;
        HashSet<Vector2Int> spawns = new();

        for (int x = startPos.x; x < endPos.x; x++)
        {
            for (int y = startPos.y; y < endPos.y; y++)
            {
                if (!tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    spawns.Add(new Vector2Int(x, y));
                }
            }
        }

        LinkedList<Vector3Int> allCoords = new();
        LinkedList<TileBase> allTiles = new();

        while (spawns.Count > 0)
        {
            Vector2Int coords = spawns.ToArray()[Random.Range(0, spawns.Count)];

            allCoords.AddLast((Vector3Int)coords);
            allTiles.AddLast(WeightedUtils.RollTile(tiles, masterTiles));

            for (int x = coords.x - distance; x < coords.x + distance; x++)
            {
                for (int y = coords.y - distance; y < coords.y + distance; y++)
                {
                    spawns.Remove(new Vector2Int(x, y));
                }
            }
        }

        tilemap.SetTiles(allCoords.ToArray(), allTiles.ToArray());
    }

    public static Vector2Int GetChunkIndexFromPosition(Vector3 position)
    {
        Vector2Int chunkPos = new();

        if (position.x < 0) chunkPos.x = Mathf.FloorToInt(position.x / chunkSize.x);
        else chunkPos.x = Mathf.CeilToInt(position.x / chunkSize.x) - 1;

        if (position.y < 0) chunkPos.y = Mathf.FloorToInt(position.y / chunkSize.y);
        else chunkPos.y = Mathf.CeilToInt(position.y / chunkSize.y) - 1;

        return chunkPos;
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