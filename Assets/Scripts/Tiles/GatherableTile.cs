using CustomClasses;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu]
public class GatherableTile : Tile
{
    [SerializeField] GatherableTileSprite[] sprites;
    [SerializeField] float regenerateTime;
    [SerializeField] DropItem[] drops;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if(go == null) return false;

        if (go.TryGetComponent<Gatherable>(out var script))
        {
            script.Init(regenerateTime, position);
        }

        return true;
    }


    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);

        var attachedTilemap = tilemap.GetComponent<Tilemap>();
        var instancedGameObject = attachedTilemap.GetInstantiatedObject(position);

        if (instancedGameObject == null) 
        {
            tileData.sprite = Array.Find(sprites, spr => spr.state == GatherableTileState.Replenished).sprite;
            return;
        }

        if (instancedGameObject.TryGetComponent<Gatherable>(out var script))
        {
            tileData.sprite = Array.Find(sprites, spr => spr.state == script.State).sprite;
        }
    }
}

[Serializable]
public enum GatherableTileState
{
    Gathered,
    HalfReplenished,
    Replenished,
}

[Serializable]
public struct GatherableTileSprite
{
    public GatherableTileState state;
    public Sprite sprite;
}
