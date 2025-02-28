using CustomClasses;
using CustomExtensions;
using EditorAttributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SaveTerrain))]
[RequireComponent(typeof(LoadTerrain))]
[RequireComponent(typeof(WorldBorderManager))]
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
    [SerializeField][Tooltip("In Chunks")] int renderRadius;

    Vector2Int currentChunk;

    WorldBorderManager borderManager;
    LoadTerrain loadTerrain;
    SaveTerrain saveTerrain;

    HashSet<Vector2Int> renderedChunks = new();
    Dictionary<Vector2Int, List<StructurePlaceData>> deferredStructures = new();
    HashSet<Vector2Int> savedChunks = new();

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
        Invoke(nameof(CompressOnStart), 0);
        loadTerrain.LoadTerrainData(deferredStructures, savedChunks);
    }

    void CompressOnStart()
    {
        CompressTilemaps(GetChunkIndex(player.position), ground, top, solid, BreakingManager.Instance.Tilemap);
    }

    void Update()
    {
        if(GameManager.IsGamePaused) 
            return;

        //IDEA: alpha 1.0.1 disable tilemaps while generating and enable at the end;
        //might help perfomance

        //IDEA alpha 2.0: only save user change data to tiles and generate chunks using the seed each time
        //treat previously saved region data as user modified to make data persistent after seed update
        bool shrunkTilemap = false;
        currentChunk = GetChunkIndex(player.position);
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

        Dictionary<Vector2Int, 
            Dictionary<Vector2Int, TilemapChunkNodesData>> parsedRegions = new();

        foreach (var chunk in chunksInRender)
        {
            if (!renderedChunks.Add(chunk) || !ChunkInsideWorldBorder(chunk)) 
                continue;

            //load chunk if in render distance and not loaded already
            Vector2Int region = GetRegionIndex(chunk);

            string fullPath = Path.Combine(GameManager.DataDirPath, DataDirName, region.ToString());
            if (File.Exists(fullPath) && !parsedRegions.ContainsKey(region))
                parsedRegions[region] = loadTerrain.ParseRegionFile(region, chunksInRender, ground, top, solid);

            if (!parsedRegions.ContainsKey(region) || !parsedRegions[region].ContainsKey(chunk))
                GenChunk(chunk * chunkSize, chunk, region);
            else
                foreach (var tilemapNodesPair in parsedRegions[region][chunk].data)
                {
                    loadTerrain.SetTilemapChunk(
                        tilemapNodesPair.Key, tilemapNodesPair.Value,
                        chunk.x * ChunkSize.x, chunk.y * ChunkSize.y);
                }

            LoadBorderIfEndOfWorld(chunk);
        }

        savedChunks.AddRange(chunksInRender);
        
        //avoid compressing multiple times
        if (shrunkTilemap)
            CompressTilemaps(currentChunk, ground, top, solid, BreakingManager.Instance.Tilemap);
    }

    Vector2Int[] GetChunksInsideRenderDistance(Vector2Int currentChunk)
    {
        int renderDiameter = (renderRadius * 2 + 1);
        var rendered = new Vector2Int[renderDiameter * renderDiameter];

        int i = 0;
        for (int x = currentChunk.x - renderRadius; x <= currentChunk.x + renderRadius; x++)
        {
            for (int y = currentChunk.y - renderRadius; y <= currentChunk.y + renderRadius; y++)
            {
                rendered[i] = new(x, y);
                i++;
            }
        }
        return rendered;
    }

    void CompressTilemaps(Vector2Int currentChunkIndex, params Tilemap[] tilemaps)
    {
        foreach (var tilemap in tilemaps)
        {
            tilemap.origin = (Vector3Int)(currentChunkIndex * chunkSize - chunkSize);

            int xSize = chunkSize.x * (renderRadius * 2 + 1);
            int ySize = chunkSize.y * (renderRadius * 2 + 1);

            tilemap.size = new(xSize, ySize, 1);
            tilemap.ResizeBounds();

            //needed for proper rendering
            tilemap.gameObject.SetActive(false);
            tilemap.gameObject.SetActive(true);
        }
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
                borderManager.LoadBorder(chunk, Direction.Right);
            else
                borderManager.LoadBorder(chunk, Direction.Left);
        }

        if (chunk.y == worldSize.y - 1 || chunk.y == -worldSize.y)
        {
            if (Mathf.Sign(chunk.y) == -1)
                borderManager.LoadBorder(chunk, Direction.Up);
            else
                borderManager.LoadBorder(chunk, Direction.Down);
        }
    }

    bool HasRoomToPlace(Tilemap tilemap, BoundsInt bounds)
    {
        bounds.position -= (Vector3Int)structureMargin;
        bounds.size += (Vector3Int)structureMargin*2;
        foreach (var tile in tilemap.GetTilesBlock(bounds))
        {
            if(tile == null)
                continue;

            return false;
        }
        return true;
    }

    void GenChunk(Vector2Int startTile, Vector2Int chunkIndex, Vector2Int region)
    {
        GenBoxTiles(ground, grTiles, startTile);
        foreach (var genTiles in topTiles)
        {
            GenGriddedTiles(top, genTiles.Tiles, genTiles.GenGap, startTile);
        }

        GenStructures(chunkIndex, startTile, solid);

        saveTerrain.SaveTiles(region, chunkIndex, ground, top, solid);
    }

    [Button("Place Structure",36)]
    void PlaceStructure(Vector3Int spawn, int group, int index)
    {
        Vector2Int renderEnd = (currentChunk + new Vector2Int(renderRadius, renderRadius) * 2) * chunkSize;
        var structure = structures[group].structures[index].structure;

        var bounds = structure.cellBounds;
        bounds.position = spawn;

        if (!HasRoomToPlace(solid, bounds))
            return;

        Vector2Int chunk = GetChunkIndex(spawn);

        if (IsOverlappingWithSavedChunks(chunk, spawn + bounds.size))
        {
            Debug.Log("structure collision with saved chunk detected:\n" + bounds.position);
            return;
        }

        //Stop structures outside of render from being generated
        CheckForStructureLeaks(chunk, bounds, renderEnd, spawn, default, structures[group], structure);

        solid.SetTilesBlock(bounds, structure.GetAllTiles());
    }

    void GenStructures(Vector2Int chunk, Vector2Int startPos, Tilemap tilemap)
    {
        Vector2Int renderEnd = (currentChunk + new Vector2Int(renderRadius, renderRadius) * 2) * chunkSize;

        if (deferredStructures.Remove(chunk, out var dataSet))
            PlaceDeferredStructures(dataSet, tilemap, renderEnd);

        Vector2Int endPos = startPos + chunkSize;

        foreach(var structureGroup in structures)
        {
            for (int i = 0; i < structureGroup.rollsPerChunk; i++)
            {
                var structure = WeightedUtils.RollStructure(structureGroup.structures);

                if (!structure) 
                    continue;

                Vector3Int spawnPosition = new(
                    GameRandom.Range(startPos.x, endPos.x),
                    GameRandom.Range(startPos.y, endPos.y));

                var bounds = structure.cellBounds;
                bounds.position = spawnPosition;

                if (!HasRoomToPlace(tilemap, bounds)) 
                    continue;

                if (IsOverlappingWithSavedChunks(chunk, spawnPosition + bounds.size))
                {
                    Debug.Log("structure collision with saved chunk detected:\n" + bounds.position);
                    continue;
                }

                //Stop structures outside of render from being generated
                CheckForStructureLeaks(chunk, bounds, renderEnd, spawnPosition, default, structureGroup, structure);
                

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

        //split chunk into cells based on distance
        int index = 0;
        for (int x = startPos.x; x < endPos.x; x += distance)
        {
            for (int y = startPos.y; y < endPos.y; y += distance)
            {
                Vector3Int position = new(
                    Mathf.Clamp(GameRandom.Range(x, x + distance),startPos.x,endPos.x-1),
                    Mathf.Clamp(GameRandom.Range(y, y + distance), startPos.y, endPos.y-1));

                positions[index] = position;
                tiles[index] = WeightedUtils.RollTile(tilePool, masterTiles);
                index++;
            }
        }

        tilemap.SetTiles(positions, tiles);
    }

    void CheckForStructureLeaks(Vector2Int chunk, BoundsInt bounds, Vector2Int renderEnd, Vector3Int spawn, Vector3Int currentTrimOffset, StructureGroup group, Tilemap structure)
    {
        Vector3Int trimOffset = (Vector3Int)renderEnd - bounds.position;
        if (trimOffset.x >= bounds.size.x)
            trimOffset.x = 0;
        if (trimOffset.y >= bounds.size.y)
            trimOffset.y = 0;

        if (trimOffset.x == 0 && trimOffset.y == 0)
            return;

        int groupIndex = Array.FindIndex(structures, g => g.Equals(group));
        int index = Array.FindIndex(group.structures, s => s.structure == structure);

        if (trimOffset.x > 0)
        {
            Vector3Int adjustedPosition = new((chunk.x + 1) * chunkSize.x, spawn.y);
            Vector3Int adjustedOffset = new(trimOffset.x, 0);
            Vector3Int size = new(bounds.size.x - trimOffset.x, bounds.size.y, 1);

            AddDeferredStructure(chunk + new Vector2Int(1, 0), 
                new(groupIndex, index, adjustedPosition, size, adjustedOffset + currentTrimOffset));
        }

        if (trimOffset.y > 0)
        {
            Vector3Int adjustedPosition = new(spawn.x, (chunk.y + 1) * chunkSize.y);
            Vector3Int adjustedOffset = new(0, trimOffset.y);
            Vector3Int size = new(bounds.size.x, bounds.size.y - trimOffset.y, 1);

            AddDeferredStructure(chunk + new Vector2Int(0, 1),
                new(groupIndex, index, adjustedPosition, size, adjustedOffset + currentTrimOffset));
        }

        if (trimOffset.x > 0 && trimOffset.y > 0)
        {
            Vector3Int adjustedPosition = (Vector3Int)(chunk * chunkSize + chunkSize);

            AddDeferredStructure(chunk + new Vector2Int(1, 1),
                new(groupIndex, index, adjustedPosition, bounds.size - trimOffset, trimOffset + currentTrimOffset));
        }
    }

    void PlaceDeferredStructures(List<StructurePlaceData> dataSet, Tilemap tilemap, Vector2Int renderEnd)
    {
        foreach (var data in dataSet)
        {
            var group = structures[data.group];
            var structure = group.structures[data.index].structure;

            var bounds = structure.cellBounds;
            bounds.position = data.position;
            bounds.size = data.size;

            CheckForStructureLeaks(GetChunkIndex(data.position), bounds, renderEnd, data.position, data.trimOffset, group, structure);

            tilemap.SetTilesBlock(bounds, structure.GetAllTilesTrimmed(data.trimOffset, data.size));

            Debug.Log("Placed deferred structure at: " + bounds.position);
        }
    }

    bool IsOverlappingWithSavedChunks(Vector2Int chunk, Vector3Int endPos)
    {
        HashSet<Vector2Int> coveredChunks = new(){chunk};
        Vector3 positionOffset = endPos - (Vector3Int)(chunk * chunkSize);
        float xOverlap = positionOffset.x / chunkSize.x;
        float yOverlap = positionOffset.y / chunkSize.y;

        for (int x = 1; x < xOverlap; x++)
            coveredChunks.Add(chunk + new Vector2Int(x, 0));

        for (int y = 1; y < yOverlap; y++)
            coveredChunks.Add(chunk + new Vector2Int(0, y));

        for (int y = 1; y < yOverlap; y++)
            for (int x = 1; x < xOverlap; x++)
                coveredChunks.Add(chunk + new Vector2Int(x, y));

        return savedChunks.Overlaps(coveredChunks);
    }

    void AddDeferredStructure(Vector2Int chunkToWaitFor, StructurePlaceData data)
    {
        deferredStructures.TryAdd(chunkToWaitFor, new());
        deferredStructures[chunkToWaitFor].Add(data);
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

    private void OnDestroy()
    {
        saveTerrain.SaveTerrainData(deferredStructures, savedChunks);
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

public struct StructurePlaceData
{
    public int group;
    public int index;
    public Vector3Int position;
    public Vector3Int size;
    public Vector3Int trimOffset;

    public StructurePlaceData(int group, int index, Vector3Int position, Vector3Int size, Vector3Int trimOffset)
    {
        this.group = group;
        this.index = index;
        this.position = position;
        this.size = size;
        this.trimOffset = trimOffset;
    }

    public StructurePlaceData(BinaryReader reader)
    {
        group = reader.ReadInt32();
        index = reader.ReadInt32();
        position = reader.ReadVector3Int();
        size = reader.ReadVector3Int();
        trimOffset = reader.ReadVector3Int();
    }

    public readonly void Write(BinaryWriter writer)
    {
        writer.Write(group);
        writer.Write(index);
        writer.Write(position);
        writer.Write(size);
        writer.Write(trimOffset);
    }
}