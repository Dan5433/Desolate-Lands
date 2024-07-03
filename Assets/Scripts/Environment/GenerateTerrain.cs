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
public class GenerateTerrain : MonoBehaviour
{
    [SerializeField] Tile[] masterTiles;
    [SerializeField] Tilemap ground;
    [SerializeField] Tilemap top;
    [SerializeField] Tilemap solid;
    [SerializeField] WeightedStructure[] structures;
    [SerializeField] WeightedTileById[] grTiles;
    [SerializeField] TerrainGenTiles[] topTiles;
    [SerializeField] Transform player;
    [SerializeField] Vector2Int worldSize = new(100, 100);
    [SerializeField] Vector2Int chunkSize = new(32, 32);
    [SerializeField] float renderDist;
    [SerializeField] ParticleSystem gasBorder;
    LoadTerrain loadTerrain;
    SaveTerrain saveTerrain;
    readonly Dictionary<Vector2Int, bool> chunks = new();
    Vector2 offsetToMiddle;

    public Tile[] MasterTiles { get { return masterTiles; } }
    public Vector2Int ChunkSize { get { return chunkSize; } }
    public Vector2Int WorldSize { get { return worldSize; } }

    void Awake()
    {
        loadTerrain = GetComponent<LoadTerrain>();
        saveTerrain = GetComponent<SaveTerrain>();

        InstantiateBorders();

        offsetToMiddle = chunkSize / 2;

        for (int x = -worldSize.x; x < worldSize.x; x++)
        {
            for (int y = -worldSize.y; y < worldSize.y; y++)
            {
                chunks.Add(new Vector2Int(x * chunkSize.x, y * chunkSize.y), false);
            }
        }
    }

    void InstantiateBorders()
    {
        ParticleSystem[] borders = {
            Instantiate(gasBorder, new Vector3(0, chunkSize.y * worldSize.y),
                                   new Vector3(0,0,180).AnglesToQuaternion()),
            Instantiate(gasBorder, new Vector3(0, -(chunkSize.y * worldSize.y)),
                                   Quaternion.identity),
            Instantiate(gasBorder, new Vector3(chunkSize.y * worldSize.y, 0),
                                   new Vector3(0, 0, 90).AnglesToQuaternion()),
            Instantiate(gasBorder, new Vector3(-(chunkSize.y * worldSize.y), 0),
                                   new Vector3(0, 0, -90).AnglesToQuaternion()) };

        foreach (var b in borders)
        {
            var shape = b.shape;
            var main = b.main;
            var emission = b.emission;
            int size;

            if (b.transform.position.x == 0)
            {
                size = chunkSize.x * worldSize.x;
            }
            else
            {
                size = chunkSize.x * worldSize.x;
            }

            shape.radius *= size;
            main.maxParticles *= size;
            emission.rateOverTimeMultiplier *= size;

            b.Stop();
            b.Play();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    void FixedUpdate()
    {
        foreach (Vector2Int pos in chunks.Keys.ToList())
        {
            float distance = Vector2.Distance(pos + offsetToMiddle, player.position);

            if (distance <= renderDist && !chunks[pos])
            {
                string fullPath = Path.Combine(GameManager.Instance.DataDirPath,"Terrain", "groundchunk" + pos);
                if (File.Exists(fullPath))
                {
                    loadTerrain.LoadTiles(ground, pos, "groundchunk");
                    loadTerrain.LoadTiles(top, pos, "topchunk");
                    loadTerrain.LoadTiles(solid, pos, "solidchunk");
                }
                else
                {
                    GenChunk(pos);
                }

                chunks[pos] = true;

            }
            else if (distance > renderDist && chunks[pos])
            {
                loadTerrain.SetFlatTiles(ground, pos, new Tile[chunkSize.x * chunkSize.y]);
                loadTerrain.SetFlatTiles(top, pos, new Tile[chunkSize.x * chunkSize.y]);
                loadTerrain.SetFlatTiles(solid, pos, new Tile[chunkSize.x * chunkSize.y]);
                chunks[pos] = false;
            }
        }
    }
    bool HasRoomToPlaceStructure(Tilemap tilemap, Vector3Int startPos, Vector3Int structSize)
    {
        if (tilemap.HasTile(startPos)) return false;
        if (tilemap.HasTile(startPos + new Vector3Int(structSize.x, 0 - 1))) return false;
        if (tilemap.HasTile(startPos + new Vector3Int(0, structSize.y - 1))) return false;
        if (tilemap.HasTile(startPos + new Vector3Int(structSize.x - 1, structSize.y - 1))) return false;
        return true;
    }

    void GenChunk(Vector2Int startPos)
    {
        GenBoxTiles(ground, grTiles, startPos);
        foreach (var genTiles in topTiles)
        {
            GenScatteredTiles(top, genTiles.Tiles, genTiles.GenGap, startPos);
        }

        GenStructures(startPos, solid);

        saveTerrain.SaveTilesAsync(top, startPos, "topchunk");
        saveTerrain.SaveTilesAsync(solid, startPos, "solidchunk");
        saveTerrain.SaveTilesAsync(ground, startPos, "groundchunk");
    }

    void GenStructures(Vector2Int startPos, Tilemap tilemap)
    {
        Vector2Int endPos = startPos + chunkSize;
        Tilemap structure = WeightedUtils.RollStructure(structures);

        if (structure == null) return;

        Vector3Int randPos = new(Random.Range(startPos.x, endPos.x), Random.Range(startPos.y, endPos.y));

        if (!HasRoomToPlaceStructure(tilemap, randPos, structure.size)) return;

        TileBase[] allTiles = structure.GetAllTiles();
        BoundsInt bounds = structure.cellBounds;
        bounds.position = randPos;

        tilemap.SetTilesBlock(bounds, allTiles);
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
}

[Serializable]
public struct TerrainGenTiles
{
    [SerializeField] WeightedTileById[] tiles;
    [SerializeField] int genGap;

    public WeightedTileById[] Tiles { get { return tiles; } }
    public int GenGap { get { return genGap; } }
}