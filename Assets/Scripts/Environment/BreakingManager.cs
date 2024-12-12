using UnityEngine;

public class BreakingManager : MonoBehaviour
{
    public static BreakingManager Instance { get; private set; }

    [SerializeField] Sprite[] breakStages;
    [SerializeField] ParticleSystem[] breakParticles;

    public Sprite[] BreakSprites => breakStages;
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
}
