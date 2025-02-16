using CustomExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MineAnimationManager : MonoBehaviour
{
    public static MineAnimationManager Instance { get; private set; }

    [SerializeField] Tilemap tilemap;
    [SerializeField] Transform player;
    [SerializeField] float revealDist = 3f;
    [SerializeField] ParticleSystem[] explosionParticles;
    private void Awake()
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

    void Update()
    {
        if (GameManager.IsGamePaused)
            return;

        float revealMultiplier = player.GetComponent<CustomPlayerData>().MineRevealDistMultiplier;

        foreach (Transform child in tilemap.gameObject.transform)
        {
            if (!child.GetComponent<Landmine>())
                continue;

            float distanceToPlayer = Vector3.Distance(child.position, player.position);

            Color color = new(1, 1, 1, 1 - distanceToPlayer * 1 / (revealDist * revealMultiplier));

            tilemap.SetColor(child.position.ToVector3Int(), color);
        }
    }

    public void ExplodeMine(Vector3 position)
    {
        foreach(var particle in explosionParticles)
            particle.PlayOneShot(position, Quaternion.Euler(-90f, 0, 0));
    }

}
