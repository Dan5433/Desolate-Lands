using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Items/Usable Item Effects/Heal")]
public class HealEffect : UsableItemEffect
{
    [SerializeField] float healAmount;

    public void Heal(GameObject user)
    {
        user.GetComponentInParent<LivingBase>().Heal(healAmount);
    }
}
