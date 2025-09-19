using CustomClasses;
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "2D/Tiles/Container Tile")]
public class ContainerTile : BreakableTile
{
    [SerializeField] int guaranteedEmptySlots;
    [SerializeField] GuaranteedLootPool[] guaranteedLootTable;
    [SerializeField] WeightedLootPool[] lootTable;

    public int GuaranteedEmptySlots => guaranteedEmptySlots;
    public GuaranteedLootPool[] GuaranteedLootTable => guaranteedLootTable;
    public WeightedLootPool[] LootTable => lootTable;
}
