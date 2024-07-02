using CustomClasses;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu]
public class RandomBreakableTile : BreakableTile
{
    [SerializeField] WeightedBreakableTile[] tiles;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        var tile = GetTile();
        tileData.sprite = tile.sprite;

        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var fields = GetType().GetFields(flags);

        foreach (var field in fields)
        {
            var tileField = tile.GetType().GetField(field.Name,flags);
            if(tileField == null) continue;

            field.SetValue(this, tileField.GetValue(tile));
        }
    }

    BreakableTile GetTile()
    {
        int totalWeight = 0;
        foreach (var tile in tiles) totalWeight += tile.Weight;

        int randomWeight = Random.Range(0, totalWeight);
        foreach (var tile in tiles)
        {
            randomWeight -= tile.Weight;
            if (randomWeight < 0)
            {
                return tile.Tile;
            }
        }
        return null;
    }
}
