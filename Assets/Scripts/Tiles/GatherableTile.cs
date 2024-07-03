using CustomClasses;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu]
public class GatherableTile : Tile
{
    [SerializeField] GatherableTileSprites sprites;
    [SerializeField] GatherableTileStates state;
    [SerializeField] float regenerateTime;
    [SerializeField] DropItem[] drops;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if(go == null) return false;

        go.GetComponent<Gatherable>().RegenerateTime = regenerateTime;
        return true;
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        Debug.Log("updated tile");
        switch (state)
        {
            case GatherableTileStates.Gathered:
                tileData.sprite = sprites.gathered;
                break;
            case GatherableTileStates.HalfReplenished:
                tileData.sprite = sprites.halfReplenished; 
                break;
            case GatherableTileStates.Replenished:
                tileData.sprite = sprites.replenished;
                break;
        }
    }
}

[Serializable]
public enum GatherableTileStates
{
    Gathered,
    HalfReplenished,
    Replenished,
}

[Serializable]
public struct GatherableTileSprites
{
    public Sprite gathered;
    public Sprite halfReplenished;
    public Sprite replenished;
}
