using System.IO;
using System.Threading.Tasks;
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

    public void LoadTiles(Tilemap tilemap, Vector2Int startPos, string saveName)
    {
        string dirPath = Path.Combine(Application.persistentDataPath, "Saves", GameManager.Instance.WorldName, "Terrain");
        JSONFileDataHandler handler = new(dirPath, saveName + startPos);

        var data = handler.LoadData<TerrainSaveData>();

        Tile[] tiles = new Tile[data.tiles.Count];

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = main.MasterTiles[];
        }

        tilemap.SetTiles(data.positions, tiles);
    }
}
