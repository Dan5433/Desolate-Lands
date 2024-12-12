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
    LinkedList<Vector3> mineList = new();

    void Update()
    {
        float revealMultiplier = player.GetComponent<CustomPlayerData>().MineRevealDistMultiplier;

        foreach (Vector3 landmine in mineList)
        {
            float distanceToPlayer = Vector2.Distance(landmine, player.position);

            Color color = new(1, 1, 1, 1 - distanceToPlayer * 1 / (revealDist * revealMultiplier));

            tilemap.SetColor(tilemap.WorldToCell(landmine), color);
        }
    }

    public void AddMine(Vector3 landmine)
    {
        mineList.AddLast(landmine);
    }

    public void DeleteMine(Vector3 landmine)
    {
        foreach(var particle in explosionParticles)
        {
            particle.PlayOneShot(landmine, new Vector3(-90f,0,0).AnglesToQuaternion());
        }

        mineList.Remove(landmine);
    }

}
