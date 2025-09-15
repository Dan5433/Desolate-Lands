using CustomClasses;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu(menuName = "2D/Tiles/Container Tile")]
public class ContainerTile : BreakableTile
{
    [SerializeField] LootItem[] guranteedLootPool;
    [SerializeField] WeightedLootPool[] lootTable;
    public LootItem[] GuranteedLootPool => guranteedLootPool;
    public WeightedLootPool[] LootTable => lootTable;
}
