using CustomClasses;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu]
public class WeightedRandomTile : Tile
{
    [SerializeField] WeightedSprite[] sprites;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        tileData.sprite = ChooseSprite();
    }

    Sprite ChooseSprite()
    {
        int totalWeight = 0;
        foreach (var sprite in sprites) totalWeight += sprite.Weight;

        int randomWeight = Random.Range(0, totalWeight);
        foreach (var sprite in sprites)
        {
            randomWeight -= sprite.Weight;
            if (randomWeight <= 0)
            {
                return sprite.Sprite;
            }
        }
        return null;
    }
}
