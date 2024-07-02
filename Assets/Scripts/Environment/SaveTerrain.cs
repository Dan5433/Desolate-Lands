using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveTerrain : MonoBehaviour
{
    GenerateTerrain main;

    void Awake()
    {
        main = GetComponent<GenerateTerrain>();
    }

    public async void SaveTilesAsync(Tilemap tilemap, Vector2Int startPos, string saveName)
    {
        Vector2Int endPos = startPos + main.ChunkSize;
        TerrainSaveData data = new();

        for (int x = startPos.x; x < endPos.x; x++)
        {
            for (int y = startPos.y; y < endPos.y; y++)
            {
                Tile tile = tilemap.GetTile<Tile>(new Vector3Int(x, y));
                if (tile == null) continue;

                int id = Array.FindIndex(main.MasterTiles, t => t == tile);

                data.indexes.Add(id);
                data.positions.Add(new Vector3Int(x, y));
            }
        }

        JsonFileDataHandler handler = new(main.DataDirPath, saveName + startPos);
        await handler.SaveDataAsync(data);
    }
}

[Serializable]
public class TerrainSaveData
{
    public List<int> indexes = new();
    public List<Vector3Int> positions = new();
}
