using CustomExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveTerrain : MonoBehaviour
{
    TerrainManager main;

    void Awake()
    {
        main = GetComponent<TerrainManager>();
    }

    /*
    Chunk Save Format:
        vector2int chunkIndex;
        foreach tilemap
        {
            string tilemapName;
            int nodeCount;
            List<TilemapSaveNode> nodes; 
        }
    */
    public void SaveTiles(Vector2Int region, Vector2Int chunkIndex, params Tilemap[] tilemaps)
    {
        var dirPath = Path.Combine(GameManager.DataDirPath, TerrainManager.DataDirName);
        BinaryDataHandler dataHandler = new(dirPath, region.ToString());
        
        dataHandler.SaveData(writer =>
        {
            writer.WriteVector2Int(chunkIndex);

            int startX = chunkIndex.x * TerrainManager.ChunkSize.x;
            int startY = chunkIndex.y * TerrainManager.ChunkSize.y;

            foreach (var tilemap in tilemaps)
            {
                writer.Write(tilemap.name);

                TilemapSaveNode currentNode = new();
                var nodes = new LinkedList<TilemapSaveNode>();

                for (int x = startX; x < startX + TerrainManager.ChunkSize.x; x++)
                {
                    for (int y = startY; y < startY + TerrainManager.ChunkSize.y; y++)
                    {
                        //get tile and its respective id
                        var tile = tilemap.GetTile<TileBase>(new(x, y));
                        int id = Array.FindIndex(main.MasterTiles, t => t == tile);

                        //write current node and make new node if different tile encountered
                        if(currentNode.tileID != id)
                        {
                            if (currentNode.length > 0) nodes.AddLast(currentNode);

                            currentNode = new() { tileID = id };
                        }

                        currentNode.length++;
                    }
                }

                //write last node if not empty
                if (currentNode.length > 0) nodes.AddLast(currentNode);

                //write node count for data alignment
                writer.Write(nodes.Count);

                //write the nodes
                foreach (var node in nodes)
                {
                    node.Write(writer);
                }
            }

        }, FileMode.Append);
    }

    public static async void RemoveTileSaveData(Vector3Int tilePosition, string tilemapName)
    {
        JsonFileDataHandler dataHandler = new(Path.Combine(GameManager.DataDirPath, "Terrain"), 
            TerrainManager.GetChunkIndexFromPosition(tilePosition + new Vector3(0.5f,0.5f,0)).ToString());

        var save = await dataHandler.LoadDataAsync<TerrainSaveData>();

        var tilemapIndex = save.tilemapNames.IndexOf(tilemapName);
        var tilemapSaveData = save.data[tilemapIndex];
        var index = tilemapSaveData.positions.FindIndex(position => position == (Vector2Int)tilePosition);

        tilemapSaveData.indexes.RemoveAt(index);
        tilemapSaveData.positions.Remove((Vector2Int)tilePosition);

        await dataHandler.SaveDataAsync(save);
    }
}

[Serializable]
public struct TilemapSaveNode
{
    public int tileID;
    public int length;

    public readonly void Write(BinaryWriter writer)
    {
        writer.Write(tileID);
        writer.Write(length);
    }
    public TilemapSaveNode(BinaryReader reader)
    {
        tileID = reader.ReadInt32();
        length = reader.ReadInt32();
    }
}

public struct TilemapChunkNodesData
{
    public Dictionary<Tilemap, LinkedList<TilemapSaveNode>> data;
}

[Serializable]
public class TerrainSaveData
{
    public List<string> tilemapNames = new();
    public List<TilemapSaveData> data = new();
}

[Serializable]
public struct TilemapSaveData
{
    public List<int> indexes;
    public List<Vector2Int> positions;
}
