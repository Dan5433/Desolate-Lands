using UnityEngine;
using UnityEngine.Tilemaps;

public class BreakingManager : MonoBehaviour
{
    public static BreakingManager Instance { get; private set; }

    [SerializeField] Tilemap effectTilemap;
    [SerializeField] Tile[] breakStages;
    [SerializeField] ParticleSystem[] breakParticles;

    public Tilemap Tilemap => effectTilemap;
    public ParticleSystem[] BreakParticles => breakParticles;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public static void UpdateBreakStage(Vector3Int position, float percentBroken)
    {
        if (percentBroken == 0)
        {
            Instance.effectTilemap.SetTile(position, null);
            return;
        }

        for (int i = 0; i < Instance.breakStages.Length; i++)
        {
            float highRange = (float)(i + 1) / Instance.breakStages.Length;
            float lowRange = (float)i / Instance.breakStages.Length;

            if (percentBroken >= lowRange && percentBroken <= highRange)
            {
                Instance.effectTilemap.SetTile(position, Instance.breakStages[i]);
                break;
            }
        }
    }
}
