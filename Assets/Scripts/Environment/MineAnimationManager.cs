using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MineAnimationManager : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] Transform player;
    [SerializeField] float revealDist = 3f;
    LinkedList<Transform> mineList = new();

    void Update()
    {
        float revealMultiplier = player.GetComponent<CustomPlayerData>().MineRevealDistMultiplier;

        foreach (Transform landmine in mineList)
        {
            float distanceToPlayer = Vector2.Distance(landmine.position, player.position);

            Color color = new(1, 1, 1, 1 - distanceToPlayer * 1 / (revealDist * revealMultiplier));

            tilemap.SetColor(tilemap.WorldToCell(landmine.position), color);
        }
    }

    public void AddMine(Transform landmine)
    {
        mineList.AddLast(landmine);
    }

    public void DeleteMine(Transform landmine)
    {
        mineList.Remove(landmine);
    }

}
