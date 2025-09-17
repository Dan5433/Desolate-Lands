using CustomClasses;
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "2D/Tiles/Container Tile")]
public class ContainerTile : BreakableTile
{
    [SerializeField] int guranteedEmptySlots;
    [SerializeField] GuranteedLootPool[] guranteedLootTable;
    [SerializeField] WeightedLootPool[] lootTable;

    public int GuranteedEmptySlots => guranteedEmptySlots;
    public GuranteedLootPool[] GuranteedLootTable => guranteedLootTable;
    public WeightedLootPool[] LootTable => lootTable;
}
