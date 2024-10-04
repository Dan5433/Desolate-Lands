using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePlaceInit : MonoBehaviour
{
    TileBase assignedTile;
    Vector3Int position;

    public void Init(TileBase tileToPlace, Vector3Int position)
    {
        assignedTile = tileToPlace;
        this.position = position;
    }

    void Start()
    {
        GetComponentInParent<Tilemap>().SetTile(position, assignedTile);
    }
}
