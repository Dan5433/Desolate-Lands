using CustomClasses;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu(menuName = "2D/Tiles/Container Tile")]
public class ContainerTile : BreakableTile
{
    [SerializeField] GuranteedLootPool[] guranteedLootTable;
    [SerializeField] WeightedLootPool[] lootTable;
    public GuranteedLootPool[] GuranteedLootTable => guranteedLootTable;
    public WeightedLootPool[] LootTable => lootTable;
}
