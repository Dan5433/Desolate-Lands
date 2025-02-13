using CustomExtensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LoadTerrain : MonoBehaviour
{
    TerrainManager main;

    void Awake()
    {
        main = GetComponent<TerrainManager>();
    }

    public void SetFlatTiles(Tilemap tilemap, Vector2Int startPos, TileBase[] tiles)
    {
        Vector2Int endPos = startPos + TerrainManager.ChunkSize;
        Vector3Int[] genCoords = new Vector3Int[TerrainManager.ChunkSize.x * TerrainManager.ChunkSize.y];

        int index = 0;

        for (int x = startPos.x; x < endPos.x; x++)
        {
            for (int y = startPos.y; y < endPos.y; y++)
            {
                genCoords[index] = new Vector3Int(x, y);
                index++;
            }
        }

        tilemap.SetTiles(genCoords, tiles);
    }

    public Dictionary<Vector2Int, TilemapChunkNodesData> ParseRegionFile(Vector2Int region, Vector2Int[] chunksToLoad, params Tilemap[] tilemaps)
    {
        Dictionary<Vector2Int, TilemapChunkNodesData> parsedChunks = new();

        BinaryDataHandler dataHandler = new(
            Path.Combine(GameManager.DataDirPath, TerrainManager.DataDirName), region.ToString());

        dataHandler.LoadData(reader =>
        {
            while (reader.BaseStream.Position + sizeof(int) * 2 < reader.BaseStream.Length)
            {
                var currentChunk = reader.ReadVector2Int();

                int tilemapCount = reader.ReadInt32();
                for (int i = 0; i < tilemapCount; i++)
                {
                    string tilemapName = reader.ReadString();
                    var tilemap = tilemaps.FirstOrDefault(t => t.name == tilemapName);

                    if (tilemap == null)
                    {
                        Debug.LogError($"Tilemap {tilemapName} not found!");
                        return;
                    }

                    int nodeCount = reader.ReadInt32();

                    //skip chunk if not in request range or already loaded
                    if (!chunksToLoad.Contains(currentChunk) ||
                    (parsedChunks.TryGetValue(currentChunk, out var chunkData)
                    && chunkData.data.ContainsKey(tilemap)))
                    {
                        reader.BaseStream.Seek(sizeof(int) * 2 * nodeCount, SeekOrigin.Current);
                        continue;
                    }

                    parsedChunks.TryAdd(currentChunk, new() { data = new()});

                    parsedChunks[currentChunk].data[tilemap] = ParseTilemapNodes(reader, nodeCount);
                }
            }
        });

        return parsedChunks;
    }

    TilemapSaveNode[] ParseTilemapNodes(BinaryReader reader, int nodeCount)
    {
        var nodes = new TilemapSaveNode[nodeCount];

        for (int i = 0; i < nodeCount; i++)
            nodes[i] = new TilemapSaveNode(reader);

        return nodes;
    }

    public void SetTilemapChunk(Tilemap tilemap, TilemapSaveNode[] nodes, int startX, int startY)
    {
        int tilesCount = TerrainManager.ChunkSize.x * TerrainManager.ChunkSize.y;
        var tiles = new TileBase[tilesCount];
        var positions = new Vector3Int[tilesCount];

        int x = startX;
        int y = startY;

        int tileIndex = 0;
        foreach(var node in nodes)
        {
            for (int i = 0; i < node.length; i++)
            {
                tiles[tileIndex] = node.tileID == -1 ? null : main.MasterTiles[node.tileID];
                positions[tileIndex] = new(x, y);

                x++;

                if (x >= startX + TerrainManager.ChunkSize.x)
                {
                    x = startX;
                    y++;
                }

                tileIndex++;
            }
        }

        tilemap.SetTiles(positions, tiles);
    }

    public void LoadDeferredStructures(Dictionary<Vector2Int, List<StructurePlaceData>> dict)
    {
        var dirPath = Path.Combine(GameManager.DataDirPath, TerrainManager.DataDirName);
        BinaryDataHandler dataHandler = new(dirPath, SaveTerrain.DeferredStructuresFileName);

        if (!dataHandler.FileExists())
            return;

        dataHandler.LoadData(reader =>
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var chunk = reader.ReadVector2Int();
                List<StructurePlaceData> structures = new(reader.ReadInt32());

                for (int s = 0; s < structures.Capacity; s++)
                    structures.Add(new(reader));

                dict.Add(chunk, structures);
            }
        });
    }
}
