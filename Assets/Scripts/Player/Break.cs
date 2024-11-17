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
    [SerializeField] InventoryBase toolInventory;
    [SerializeField][Tooltip("In Seconds")] float breakTimeReduction;
    [SerializeField][Tooltip("Mutiplier")] float wrongToolPenalty;
    public void ResetBreaking()
    {
        breaking = false;
        if (breakCell.z != -1)
        {
            effects.SetTile(breakCell, null);
            breakCell.z = -1;
        }
    }

    public void Breaking(BreakableTile breakTile, Vector3Int cell, Tilemap tilemap, AudioSource interactAudio)
    {
        tile = breakTile;
        breakCell = cell;

        var equippedTool = toolInventory.Inventory[0].ItemObj as Tool;
        if (equippedTool == null /*|| tile.MinMaterial > equippedTool.Material*/) return;

        if (!breaking)
        {
            ApplyPenalties(equippedTool);

            curBreakTime = breakingTime;
            breaking = true;
        }

        if (curBreakTime > 0 && breaking)
        {
            curBreakTime -= Time.deltaTime;

            interactAudio.PlayRandomClip(tile.BreakingAudio);
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

    void UpdateDurability()
    {
        var equippedTool = toolInventory.Inventory[0] as InvTool;
        equippedTool.Durability -= 1;

        if (equippedTool.Durability <= 0) toolInventory.Inventory[0] = ItemManager.Instance.InvItemAir;
        toolInventory.UpdateUI();
    }

    void OnBreak(Tilemap tilemap)
    {
        UpdateDurability();

        SaveTerrain.RemoveTileSaveData(breakCell, tilemap.name);

        var go = tilemap.GetInstantiatedObject(breakCell);
        if (go.TryGetComponent<IBreakable>(out var breakable)) breakable.OnBreak();

        tilemap.SetTile(breakCell, null);
        effects.SetTile(breakCell, null);

        foreach (var drop in tile.Drops)
        {
            int count = drop.RandomCount();
            if (count == 0) { continue; }

            ItemManager.SpawnGroundItem(new InvItem(drop.item, drop.item.Name, count), breakCell, true);
        }

        breakParticles.ChangeColors(tile.Colors[0], tile.Colors[1]);
        breakParticles.transform.position = breakCell;
        breakParticles.Play();
    }

    void ApplyPenalties(Tool tool)
    {
        if (!tile) return;

        breakingTime = tile.BreakingTime - breakTimeReduction * (tool.Material - tile.MinMaterial);

        if (tile.Tool != tool.Type && tile.Tool != ToolType.None) breakingTime *= wrongToolPenalty;
    }
}
