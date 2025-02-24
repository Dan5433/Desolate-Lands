using CustomClasses;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu(menuName = "2D/Tiles/Random Weighted Tile")]
public class RandomWeightedTile : Tile
{
    [SerializeField] WeightedTile[] tiles;
    [SerializeField] float noiseScale = 0.1f;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (!Application.isPlaying)
            return false;

        GameManager.Instance.StartCoroutine(
            SetTileNextFrame(tilemap.GetComponent<Tilemap>(), position));

        return true;
    }

    IEnumerator SetTileNextFrame(Tilemap instancedTilemap, Vector3Int position)
    {
        yield return null;
        instancedTilemap.SetTile(position, RollTileByPosition(position));
    }

    TileBase RollTileByPosition(Vector3Int position)
    {
        float noiseValue = Mathf.PerlinNoise(position.x * noiseScale, position.y * noiseScale);

        int totalWeight = 0;
        foreach (var tile in tiles) 
            totalWeight += tile.weight;

        int scaledWeight = (int)(totalWeight * noiseValue);

        foreach (var tile in tiles)
        {
            scaledWeight -= tile.weight;
            if (scaledWeight < 0)
                return tile.tile;
        }

        return null;
    }
}
