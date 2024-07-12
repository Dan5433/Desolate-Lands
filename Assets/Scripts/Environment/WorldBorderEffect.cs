using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WorldBorderEffect : MonoBehaviour
{
    [SerializeField][Tooltip("How far from the border the effects start")] float effectRange;
    [SerializeField] float[] postProcessMaxIntensities;
    [SerializeField] PostProcessProfile postProcessEffect;
    [SerializeField] TerrainManager genTerrain;
    Vector2Int totalWorldSize;
    bool effectEnabled;

    void Awake()
    {
        totalWorldSize = genTerrain.WorldSize * genTerrain.ChunkSize;
    }

    void Update()
    {
        float xProximity = totalWorldSize.x - Mathf.Abs(transform.position.x);
        float yProximity = totalWorldSize.y - Mathf.Abs(transform.position.y);

        if (xProximity <= effectRange || yProximity <= effectRange)
        {
            effectEnabled = true;
            float proximity = Mathf.Min(xProximity, yProximity);
            float effectStrength = Mathf.InverseLerp(effectRange, 0, proximity);

            foreach (var effect in postProcessEffect.settings)
            {
                effect.enabled.value = true;
            }

            postProcessEffect.TryGetSettings<Vignette>(out var vignette);
            postProcessEffect.TryGetSettings<Grain>(out var grain);

            vignette.intensity.value = Mathf.Lerp(0, postProcessMaxIntensities[0], effectStrength);
            grain.intensity.value = Mathf.Lerp(0, postProcessMaxIntensities[1], effectStrength);
        }
        else
        {
            if (effectEnabled)
            {
                foreach (var effect in postProcessEffect.settings)
                {
                    effect.enabled.value = false;
                }
                effectEnabled = false;
            }
        }
    }
}
