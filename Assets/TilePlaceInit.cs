using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePlaceInit : MonoBehaviour
{
    public Tile tileToPlace;
    public Vector3Int position;

    private void Start()
    {
        transform.parent.GetComponent<Tilemap>().SetTile(position, tileToPlace);
    }
}
