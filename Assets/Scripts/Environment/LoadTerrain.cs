using CustomExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.WSA;

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

    public Dictionary<Vector2Int, TilemapChunkNodesData> LoadRegionFile(Dictionary<Vector2Int, TilemapChunkNodesData> loadedChunks, Vector2Int region, params Tilemap[] tilemaps)
    {
        BinaryDataHandler dataHandler = new(
            Path.Combine(GameManager.DataDirPath, TerrainManager.DataDirName), region.ToString());

        dataHandler.LoadData(reader =>
        {
            while (reader.BaseStream.Position + sizeof(int) * 2 < reader.BaseStream.Length)
            {
                var currentChunk = reader.ReadVector2Int();

                for (int i = 0; i < tilemaps.Length; i++)
                {
                    string tilemapName = reader.ReadString();
                    var tilemap = tilemaps.FirstOrDefault(t => t.name == tilemapName);

                    if (tilemap == null)
                    {
                        Debug.LogError($"Tilemap {tilemapName} not found!");
                        return;
                    }

                    int nodeCount = reader.ReadInt32();

                    //skip chunk if already loaded
                    if (loadedChunks.TryGetValue(currentChunk, out var chunkData) 
                        && chunkData.data.ContainsKey(tilemap))
                    {
                        reader.BaseStream.Seek(sizeof(int) * 2 * nodeCount, SeekOrigin.Current);
                        continue;
                    }

                    loadedChunks.TryAdd(currentChunk, new() { data = new()});

                    loadedChunks[currentChunk].data[tilemap] = ParseTilemapNodes(reader, nodeCount);
                }
            }
        });

        return loadedChunks;
    }

    LinkedList<TilemapSaveNode> ParseTilemapNodes(BinaryReader reader, int nodeCount)
    {
        var nodes = new LinkedList<TilemapSaveNode>();

        for (int i = 0; i < nodeCount; i++)
            nodes.AddLast(new TilemapSaveNode(reader));

        return nodes;
    }

    public void SetTilemapChunk(Tilemap tilemap, LinkedList<TilemapSaveNode> nodes, int startX, int startY)
    {
        var tileData = new Dictionary<Vector3Int, TileBase>();
        int x = startX;
        int y = startY;

        foreach(var node in nodes)
        {
            for (int i = 0; i < node.length; i++)
            {
                var tile = node.tileID == -1 ? null : main.MasterTiles[node.tileID];
                tileData.Add(new(x, y), tile);

                y++;

                if (y >= startY + TerrainManager.ChunkSize.y)
                {
                    y = startY;
                    x++;
                }
            }
        }

        tilemap.SetTiles(tileData.Keys.ToArray(), tileData.Values.ToArray());
    }
}
