using CustomExtensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MineAnimationManager : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] Transform player;
    [SerializeField] float revealDist = 3f;
    [SerializeField] ParticleSystem[] explosionParticles;
    LinkedList<Vector3Int> mineList = new();

    void Update()
    {
        float revealMultiplier = player.GetComponent<CustomPlayerData>().MineRevealDistMultiplier;

        foreach (Vector3Int landmine in mineList)
        {
            float distanceToPlayer = Vector3.Distance(landmine, player.position);

            Color color = new(1, 1, 1, 1 - distanceToPlayer * 1 / (revealDist * revealMultiplier));

            tilemap.SetColor(landmine, color);
        }
    }

    public void AddMine(Vector3Int landmine)
    {
        mineList.AddLast(landmine);
    }

    public void DeleteMine(Vector3Int landmine)
    {
        foreach(var particle in explosionParticles)
        {
            particle.PlayOneShot(landmine, new Vector3(-90f,0,0).AnglesToQuaternion());
        }

        mineList.Remove(landmine);
    }

}
