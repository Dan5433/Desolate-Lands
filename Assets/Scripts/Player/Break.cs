using CustomClasses;
using CustomExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Break : MonoBehaviour
{
    float breakingTime;
    float curBreakTime;
    bool breaking;
    Vector3Int breakCell;
    BreakableTile tile;

    [SerializeField] Tile[] breakEffects;
    [SerializeField] ParticleSystem breakParticles;
    [SerializeField] Tilemap effects;
    [SerializeField] PlayAudio audioSource;
    [SerializeField] EfficientTool tool;
    [SerializeField] RequiredMaterial material;
    [SerializeField][Tooltip("In Seconds")] float breakTimeReduction;
    [SerializeField][Tooltip("Mutiplier per Material")] float wrongToolPenalty;
    public void ResetBreaking()
    {
        breaking = false;
        if (breakCell.z != -1)
        {
            effects.SetTile(breakCell, null);
            breakCell.z = -1;
        }
    }

    public void Breaking(BreakableTile breakTile, Vector3Int cell, Tilemap tilemap)
    {
        tile = breakTile;
        breakCell = cell;

        //if (tile.MinMaterial > material)
        //{
        //    Debug.LogWarning("Need stronger material!");
        //    return;
        //}

        if (!breaking)
        {
            ApplyPenalties();

            curBreakTime = breakingTime;
            breaking = true;
        }

        if (curBreakTime > 0 && breaking)
        {
            curBreakTime -= Time.deltaTime;

            audioSource.PlayRandomSound(material.ToString());
            UpdateEffect();
        }

        else if (curBreakTime <= 0 && breaking)
        {
            OnBreak(tilemap);
            breakCell.z = -1;
        }
    }

    void UpdateEffect()
    {
        float percentBroken = (breakingTime - curBreakTime) / breakingTime;

        for (int i = 0; i < breakEffects.Length; i++)
        {
            float highRange = (float)(i + 1) / breakEffects.Length;
            float lowRange = (float)i / breakEffects.Length;

            if (percentBroken >= lowRange && percentBroken <= highRange)
            {
                effects.SetTile(breakCell, breakEffects[i]);
                break;
            }
        }
    }

    async void RemoveTileData()
    {

    }

    void OnBreak(Tilemap tilemap)
    {
        PlayerPrefs.DeleteKey(tilemap.name + breakCell);
        tilemap.SetTile(breakCell, null);
        effects.SetTile(breakCell, null);

        foreach (var drop in tile.Drops)
        {
            int count = Random.Range(drop.MinDropCount, drop.MaxDropCount);
            if (count == 0) { continue; }

            ItemManager.SpawnGroundItem(new InvItem(drop.Item, drop.Item.Name, count), breakCell, true);
        }

        breakParticles.ChangeColors(tile.Colors[0], tile.Colors[1]);
        breakParticles.transform.position = breakCell;
        breakParticles.Play();
    }

    void ApplyPenalties()
    {
        breakingTime = tile.BreakingTime - breakTimeReduction * (material - tile.MinMaterial);
        if (tile.Tool != tool && tile.Tool != EfficientTool.None)
        {
            breakingTime *= wrongToolPenalty;
        }
    }
}
