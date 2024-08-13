using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Items/Mine Reaveal Effect")]
public class MineRevealEffect : ItemEffect
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
