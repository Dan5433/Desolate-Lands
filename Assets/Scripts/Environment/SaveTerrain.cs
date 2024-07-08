using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveTerrain : MonoBehaviour
{
    GenerateTerrain main;

    void Awake()
    {
        main = GetComponent<GenerateTerrain>();
    }

    public async void SaveTilesAsync(Tilemap tilemap, Vector2Int indexPos, string saveName)
    {
        TerrainSaveData data = new();

        for (int x = indexPos.x * main.ChunkSize.x; x < indexPos.x * main.ChunkSize.x + main.ChunkSize.x; x++)
        {
            for (int y = indexPos.y * main.ChunkSize.y; y < indexPos.y * main.ChunkSize.y + main.ChunkSize.y; y++)
            {
                Tile tile = tilemap.GetTile<Tile>(new(x,y));
                if (tile == null) continue;

                int id = Array.FindIndex(main.MasterTiles, t => t == tile);

                data.indexes.Add(id);
                data.positions.Add(new(x, y));
            }
        }

        JsonFileDataHandler handler = new(Path.Combine(GameManager.Instance.DataDirPath, "Terrain"), saveName + indexPos);
        await handler.SaveDataAsync(data);
    }
}

[Serializable]
public class TerrainSaveData
{
    public List<int> indexes = new();
    public List<Vector3Int> positions = new();
}
