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
    [SerializeField] Tile[] masterTiles;
    [SerializeField] Tilemap ground, top, solid;
    [SerializeField] WeightedStructure[] structures;
    [SerializeField] WeightedTileById[] grTiles;
    [SerializeField] TerrainGenTiles[] topTiles;
    [SerializeField] Transform player;
    [SerializeField] Vector2Int worldSize;
    [SerializeField] int structureRollsPerChunk;
    [SerializeField] Vector2Int structureMargin;
    [SerializeField][Tooltip("In Chunks")] int renderDist;

    WorldBorderManager borderManager;
    LoadTerrain loadTerrain;
    SaveTerrain saveTerrain;
    HashSet<Vector2Int> loadedChunks = new();
    static Vector2Int chunkSize = new(32, 32);
    public const string chunkSaveName = "chunk";

    public Tile[] MasterTiles { get { return masterTiles; } }
    public static Vector2Int ChunkSize { get { return chunkSize; } }
    public Vector2Int WorldSize { get { return worldSize; } }

    void Awake()
    {
        loadTerrain = GetComponent<LoadTerrain>();
        saveTerrain = GetComponent<SaveTerrain>();
        borderManager = GetComponent<WorldBorderManager>(); 
    }

    void Start()
    {
        CompressTilemaps(GetChunkIndexFromPosition(player.position), ground, top, solid);
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
        //TODO: Find out why origin doesn't automatically update
        foreach (var tilemap in tilemaps)
        {
            tilemap.origin = (Vector3Int)(currentChunkIndex * chunkSize - chunkSize);
            tilemap.size = new(chunkSize.x * (renderDist * 2 + 1), chunkSize.y * (renderDist * 2 + 1), 1);
            tilemap.ResizeBounds();
        }
    }

    void Update()
    {
        //TODO: only save user change data to tiles and generate chunks using the seed each time 
        bool shrunkTilemap = false;
        var currentChunk = GetChunkIndexFromPosition(player.position);
        var renderedChunks = GetChunksInsideRenderDistance(currentChunk);

        foreach (var chunk in loadedChunks.ToArray())
        {
            if (renderedChunks.Contains(chunk)) continue;

            //unload chunk if not in render distance but is loaded
            borderManager.UnloadBorder(chunk);

            loadedChunks.Remove(chunk);

            shrunkTilemap = true;
        }

        foreach (var chunk in renderedChunks)
        {
            if (!loadedChunks.Add(chunk) || !ChunkInsideWorldBorder(chunk)) continue;

            //load chunk if in render distance and not loaded already
            string fullPath = Path.Combine(GameManager.Instance.DataDirPath, "Terrain", chunk.ToString());
            if (File.Exists(fullPath))
            {
                loadTerrain.LoadTiles(chunk, ground, top, solid);
            }
            else
            {
                GenChunk(chunk * chunkSize, chunk);
            }

            LoadBorderIfEndOfWorld(chunk);
        }

        if (shrunkTilemap) CompressTilemaps(currentChunk, ground, top, solid);
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

    void GenChunk(Vector2Int tilePos, Vector2Int chunkIndex)
    {
        GenBoxTiles(ground, grTiles, tilePos);
        foreach (var genTiles in topTiles)
        {
            GenScatteredTiles(top, genTiles.Tiles, genTiles.GenGap, tilePos);
        }

        GenStructures(tilePos, solid);

        saveTerrain.SaveTilesAsync(chunkIndex, ground, top, solid);
    }

    void GenStructures(Vector2Int startPos, Tilemap tilemap)
    {
        Vector2Int endPos = startPos + chunkSize;

        for(int i = 0; i < structureRollsPerChunk; i++)
        {
            var structure = WeightedUtils.RollStructure(structures);

            if (structure == null) continue;

            Vector3Int spawnPosition = new(Random.Range(startPos.x, endPos.x), Random.Range(startPos.y, endPos.y));

            var bounds = structure.cellBounds;
            bounds.position = spawnPosition;

            if (!HasRoomToPlaceStructure(tilemap, bounds)) continue;

            tilemap.SetTilesBlock(bounds, structure.GetAllTiles());
        }
    }

    void GenBoxTiles(Tilemap tilemap, WeightedTileById[] tiles, Vector2Int startPos)
    {
        Vector2Int endPos = startPos + chunkSize;
        Vector3Int[] genCoords = new Vector3Int[chunkSize.x * chunkSize.y];
        Tile[] genTiles = new Tile[genCoords.Length];

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
        LinkedList<Tile> allTiles = new();

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

    public WeightedTileById[] Tiles { get { return tiles; } }
    public int GenGap { get { return genGap; } }
}