using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveTerrain : MonoBehaviour
{
    GenerateTerrain main;

    void Awake()
    {
        main = GetComponent<GenerateTerrain>();
    }

    public void SaveTiles(Tilemap tilemap, Vector2Int startPos, string saveName)
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

                data.indexes.AddLast(id);
                data.positions.AddLast(new Vector3Int(x, y));
            }
        }

        string dirPath = Path.Combine(Application.persistentDataPath, "Saves", GameManager.Instance.WorldName, "Terrain");

        JSONFileDataHandler handler = new(dirPath, saveName + startPos);
        handler.SaveData(data);
    }
}

[Serializable]
public struct TerrainSaveData
{
    public Dictionary<int, Vector3Int> tiles;
}
