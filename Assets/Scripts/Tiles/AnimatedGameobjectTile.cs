using CustomClasses;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu(menuName = "2D/Tiles/Animated Gameobject Tile")]
public class AnimatedGameobjectTile : AnimatedTile
{
    [SerializeField] GameObject instancedGameobject;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        tileData.gameObject = instancedGameobject;
    }
}
