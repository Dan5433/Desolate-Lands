using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveTerrain : MonoBehaviour
{
    TerrainManager main;

    void Awake()
    {
        main = GetComponent<TerrainManager>();
    }

    public void SaveTiles(Vector2Int chunkIndex, params Tilemap[] tilemaps)
    {
        Vector2Int region = new(chunkIndex.x/TerrainManager.RegionSize.x, 
            chunkIndex.y / TerrainManager.RegionSize.y);

        var dirPath = Path.Combine(GameManager.DataDirPath, TerrainManager.DirName);
        BinaryDataHandler dataHandler = new(dirPath, region.ToString());
        
        dataHandler.SaveData(writer =>
        {
            TerrainSaveNode node = new();

            foreach (var tilemap in tilemaps)
            {
                writer.Write(tilemap.name);

                for (int x = chunkIndex.x * TerrainManager.ChunkSize.x; x < chunkIndex.x * TerrainManager.ChunkSize.x + TerrainManager.ChunkSize.x; x++)
                {
                    for (int y = chunkIndex.y * TerrainManager.ChunkSize.y; y < chunkIndex.y * TerrainManager.ChunkSize.y + TerrainManager.ChunkSize.y; y++)
                    {
                        TileBase tile = tilemap.GetTile<TileBase>(new(x, y));
                        int id = Array.FindIndex(main.MasterTiles, t => t == tile);

                        //write current node and make new node if different tile encountered
                        if(node.tileID != id)
                        {
                            writer.Write(node.tileID);
                            writer.Write(node.length);

                            node = new() { tileID = id };
                        }

                        node.length++;
                    }
                }
            }
        });
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
public struct TerrainSaveNode
{
    public int tileID;
    public int length;
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
