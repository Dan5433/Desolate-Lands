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

    public async void SaveTilesAsync(Vector2Int chunkIndex, params Tilemap[] tilemaps)
    {
        TerrainSaveData tilemapData = new();
        foreach (var tilemap in tilemaps) 
        {
            TilemapSaveData data = new() { indexes = new(), positions = new() };

            for (int x = chunkIndex.x * main.ChunkSize.x; x < chunkIndex.x * main.ChunkSize.x + main.ChunkSize.x; x++)
            {
                for (int y = chunkIndex.y * main.ChunkSize.y; y < chunkIndex.y * main.ChunkSize.y + main.ChunkSize.y; y++)
                {
                    Tile tile = tilemap.GetTile<Tile>(new(x, y));
                    if (tile == null) continue;

                    int id = Array.FindIndex(main.MasterTiles, t => t == tile);

                    data.indexes.Add(id);
                    data.positions.Add(new(x, y));
                }
            }

            tilemapData.tilemapNames.Add(tilemap.name);
            tilemapData.data.Add(data);
        }
        

        JsonFileDataHandler handler = new(Path.Combine(GameManager.Instance.DataDirPath, "Terrain"), chunkIndex.ToString());
        await handler.SaveDataAsync(tilemapData);
    }

    public static async void RemoveTileSaveData(Vector3Int tilePosition, string tilemapName)
    {
        JsonFileDataHandler dataHandler = new(Path.Combine(GameManager.Instance.DataDirPath, "Terrain"), 
            TerrainManager.GetChunkIndexFromPosition(tilePosition).ToString());

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
