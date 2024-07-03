using CustomClasses;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu]
public class RandomTile : Tile
{
    [SerializeField] WeightedTile[] tiles;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (go == null) return false;

        if(go.TryGetComponent<TilePlaceInit>(out var script))
        {
            script.tileToPlace = GetTile();
            script.position = position;
        }

        return true;
    }

    Tile GetTile()
    {
        int totalWeight = 0;
        foreach (var tile in tiles) totalWeight += tile.weight;

        int randomWeight = Random.Range(0, totalWeight);
        foreach (var tile in tiles)
        {
            randomWeight -= tile.weight;
            if (randomWeight < 0)
            {
                return tile.tile;
            }
        }
        return null;
    }
}
