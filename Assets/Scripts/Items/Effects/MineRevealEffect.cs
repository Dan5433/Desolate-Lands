using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Items/Equippable Item Effects/Landmine Reveal")]
public class MineRevealEffect : EquippableItemEffect
{
    [SerializeField] float distance;

    public void BuffDistance(GameObject player)
    {
        player.GetComponent<CustomPlayerData>().MineRevealDistMultiplier = distance;
    }
    public void DefaultDistance(GameObject player)
    {
        player.GetComponent<CustomPlayerData>().MineRevealDistMultiplier = 1f;
    }
}
