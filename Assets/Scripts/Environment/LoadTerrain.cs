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

    public Dictionary<Vector2Int, TilemapChunkNodesData> ParseRegionFile(Vector2Int region, LinkedList<Vector2Int> chunksToLoad, params Tilemap[] tilemaps)
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

    LinkedList<TilemapSaveNode> ParseTilemapNodes(BinaryReader reader, int nodeCount)
    {
        LinkedList<TilemapSaveNode> nodes = new();

        for (int i = 0; i < nodeCount; i++)
            nodes.AddLast(new TilemapSaveNode(reader));

        return nodes;
    }

    public void SetTilemapChunk(Tilemap tilemap, LinkedList<TilemapSaveNode> nodes, int startX, int startY)
    {
        //TODO: fix excessive calls when slightly moving (without loading new chunks)
        var tileData = new Dictionary<Vector3Int, TileBase>();
        int x = startX;
        int y = startY;

        foreach(var node in nodes)
        {
            for (int i = 0; i < node.length; i++)
            {
                var tile = node.tileID == -1 ? null : main.MasterTiles[node.tileID];
                tileData.Add(new(x, y), tile);

                x++;

                if (x >= startX + TerrainManager.ChunkSize.x)
                {
                    x = startX;
                    y++;
                }
            }
        }

        tilemap.SetTiles(tileData.Keys.ToArray(), tileData.Values.ToArray());
    }
}
