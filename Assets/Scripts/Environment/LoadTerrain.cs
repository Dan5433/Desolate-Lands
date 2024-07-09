using System.IO;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LoadTerrain : MonoBehaviour
{
    GenerateTerrain main;

    void Awake()
    {
        main = GetComponent<GenerateTerrain>();
    }

    public void SetFlatTiles(Tilemap tilemap, Vector2Int startPos, Tile[] tiles)
    {
        Vector2Int endPos = startPos + main.ChunkSize;
        Vector3Int[] genCoords = new Vector3Int[main.ChunkSize.x * main.ChunkSize.y];

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

    public async void LoadTiles(Tilemap tilemap, Vector2Int indexPos, string saveName)
    {
        JsonFileDataHandler handler = new(Path.Combine(GameManager.Instance.DataDirPath, "Terrain"), tilemap.name+saveName + indexPos);

        var data = await handler.LoadDataAsync<TerrainSaveData>();

        Tile[] tiles = new Tile[data.indexes.Count];

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = main.MasterTiles[data.indexes[i]];
        }

        tilemap.SetTiles(data.positions.ToArray(), tiles);
    }

    public static async Task<bool> TileExists(Vector3Int tilePosition, string tilemapName)
    {
        JsonFileDataHandler handler = new(Path.Combine(GameManager.Instance.DataDirPath, "Terrain"),
            tilemapName + GenerateTerrain.chunkSaveName + GenerateTerrain.GetChunkIndexFromPosition(tilePosition));

        var save = await handler.LoadDataAsync<TerrainSaveData>();

        return save.positions.Contains(tilePosition);
    }
}
