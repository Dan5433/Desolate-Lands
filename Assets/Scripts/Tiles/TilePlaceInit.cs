using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePlaceInit : MonoBehaviour
{
    Tile assignedTile;
    Vector3Int position;

    public void Init(Tile tileToPlace, Vector3Int position)
    {
        assignedTile = tileToPlace;
        this.position = position;
    }

    void Start()
    {
        GetComponentInParent<Tilemap>().SetTile(position, assignedTile);
    }
}
