using CustomExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

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
        int tilemapCount;
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
            writer.Write(chunkIndex);

            int startX = chunkIndex.x * TerrainManager.ChunkSize.x;
            int startY = chunkIndex.y * TerrainManager.ChunkSize.y;

            writer.Write(tilemaps.Length);
            foreach (var tilemap in tilemaps)
            {
                writer.Write(tilemap.name);

                TilemapSaveNode currentNode = new();
                List<TilemapSaveNode> nodes = new();

                BoundsInt bounds = new(startX, startY, 0, 
                    TerrainManager.ChunkSize.x, TerrainManager.ChunkSize.y, 1);

                var tiles = tilemap.GetTilesBlock(bounds);

                int index = 0;
                for (int x = startX; x < startX + TerrainManager.ChunkSize.x; x++)
                {
                    for (int y = startY; y < startY + TerrainManager.ChunkSize.y; y++)
                    {
                        int id = tiles[index] != null ? main.TileLookup[tiles[index]] : -1;

                        //write current node and make new node if different tile encountered
                        if(currentNode.tileID != id)
                        {
                            if (currentNode.length > 0) nodes.Add(currentNode);

                            currentNode = new() { tileID = id };
                        }

                        currentNode.length++;
                        index++;
                    }
                }

                //write last node if not empty
                if (currentNode.length > 0) 
                    nodes.Add(currentNode);

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

    public static void RemoveTileData(Vector3Int tilePosition, string tilemapName)
    {
        Vector2Int chunkIndex = TerrainManager.GetChunkIndex(tilePosition);

        var dirPath = Path.Combine(GameManager.DataDirPath, TerrainManager.DataDirName);
        BinaryDataHandler dataHandler = new(dirPath, TerrainManager.GetRegionIndex(chunkIndex).ToString());

        dataHandler.ModifyData((reader, writer) =>
        {
            while (reader.BaseStream.Position + sizeof(int) * 2 < reader.BaseStream.Length)
            {
                Vector2Int currentChunk = reader.ReadVector2Int();
                writer.Write(currentChunk);

                //process tilemap if chunk matches
                if (currentChunk == chunkIndex)
                {
                    int tilemapCount = reader.ReadInt32();
                    writer.Write(tilemapCount);

                    for (int i = 0; i < tilemapCount; i++)
                    {
                        string currentTilemap = reader.ReadString();
                        writer.Write(currentTilemap);

                        int nodeCount = reader.ReadInt32();

                        //write nodes as-is if tilemap doesn't match
                        if (currentTilemap != tilemapName)
                        {
                            writer.Write(nodeCount);

                            byte[] nodes = reader.ReadBytes(sizeof(int) * 2 * nodeCount);
                            writer.Write(nodes);
                            continue;
                        }

                        TilemapSaveNode[] tilemapNodes = new TilemapSaveNode[nodeCount];
                        for (int j = 0; j < nodeCount; j++)
                        {
                            tilemapNodes[j] = new(reader);
                        }

                        Vector2Int localOffset = new(
                            tilePosition.x - TerrainManager.ChunkSize.x * chunkIndex.x, 
                            tilePosition.y - TerrainManager.ChunkSize.y * chunkIndex.y);

                        int tileIndex = localOffset.y + TerrainManager.ChunkSize.y * localOffset.x;

                        var modifiedNodes = TilemapSaveNode.DeleteTile(tileIndex, tilemapNodes);
                        
                        writer.Write(modifiedNodes.Count);
                        foreach(var node in modifiedNodes)
                        {
                            node.Write(writer);
                        }
                    }
                }
                else
                {
                    //copy unrelated chunks as-is
                    int tilemapCount = reader.ReadInt32();
                    writer.Write(tilemapCount);

                    for(int i = 0;i < tilemapCount; i++)
                    {
                        string tilemapName = reader.ReadString();
                        writer.Write(tilemapName);

                        int nodeCount = reader.ReadInt32();
                        writer.Write(nodeCount);

                        //write all nodes
                        byte[] nodes = reader.ReadBytes(sizeof(int) * 2 * nodeCount);
                        writer.Write(nodes);
                    }
                }
            }
        });
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

    public static LinkedList<TilemapSaveNode> DeleteTile(int tileIndex, TilemapSaveNode[] nodes)
    {
        LinkedList<TilemapSaveNode> results = new();

        //length: 10, id: -1
        //length: 3, id: 1
        //length: 10, id: -1
        //tile index: 12 (the middle one in id 1)
        //index in node: 2

        int currentIndex = 0;
        for(int i = 0; i < nodes.Length; i++)
        {
            var node = nodes[i];

            if (tileIndex < currentIndex || tileIndex >= currentIndex + node.length)
            {
                results.AddLast(node);
                currentIndex += node.length;
                continue;
            }

            int indexInNode = tileIndex - currentIndex;
            bool mergedWithOtherEmpty = false;

            //add tiles up to target if tile is not first in node
            if(indexInNode > 0)
            {
                results.AddLast(new TilemapSaveNode()
                {
                    length = indexInNode,
                    tileID = node.tileID
                });
            }
            else
            {
                if (i > 0 && nodes[i - 1].tileID == -1 && !mergedWithOtherEmpty)
                {
                    //merge with the previous empty node if possible
                    results.Last.Value = new()
                    {
                        length = results.Last.Value.length+1,
                        tileID = -1
                    };
                    mergedWithOtherEmpty = true;
                }
                else
                {
                    //add a new empty node otherwise
                    results.AddLast(new TilemapSaveNode
                    {
                        length = 1,
                        tileID = -1
                    });
                }
            }

            //add the part of the node after the tile being removed (if it exists)
            int remainingLength = node.length - indexInNode - 1;
            if (remainingLength > 0)
            {
                results.AddLast(new TilemapSaveNode
                {
                    length = remainingLength,
                    tileID = node.tileID
                });
            }
            else if (i < nodes.Length - 1 && nodes[i + 1].tileID == -1 && !mergedWithOtherEmpty)
            {
                //merge with the next empty node if possible
                nodes[i + 1].length++;
            }

            currentIndex += node.length; //update the current index
        }

        return results;
    }
}

public struct TilemapChunkNodesData
{
    public Dictionary<Tilemap, TilemapSaveNode[]> data;
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
