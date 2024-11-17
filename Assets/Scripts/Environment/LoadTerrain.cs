using System.IO;
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

    public async void LoadTiles(Vector2Int chunkIndex, params Tilemap[] tilemaps)
    {
        JsonFileDataHandler handler = new(Path.Combine(GameManager.DataDirPath, TerrainManager.DirName), chunkIndex.ToString());

        var save = await handler.LoadDataAsync<TerrainSaveData>();

        foreach(var tilemap in tilemaps) 
        {
            var tilemapIndex = save.tilemapNames.IndexOf(tilemap.name);
            var tilemapSaveData = save.data[tilemapIndex];

            TileBase[] tiles = new TileBase[tilemapSaveData.indexes.Count];
            Vector3Int[] positions = new Vector3Int[tilemapSaveData.positions.Count];

            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = main.MasterTiles[tilemapSaveData.indexes[i]];
                positions[i] = (Vector3Int)tilemapSaveData.positions[i];
            }

            tilemap.SetTiles(positions, tiles);
        }
    }
}
