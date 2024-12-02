using EditorAttributes;
using System.Collections;
using UnityEngine;

public class LivingBase : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;

    protected virtual void Start()
    {
        //TODO: implement saving and loading health
        health = maxHealth;
    }

    [Button("Damage",30)]
    public virtual void Damage(float damageAmount)
    {
        health -= damageAmount;

        if(health <= 0) OnDeath();
    }

    [Button("Heal", 30)]
    public virtual bool Heal(float healAmount)
    {
        if (health == maxHealth) return false;

        health = Mathf.Clamp(health + healAmount, 0, maxHealth);
        return true;
    }

    protected virtual void OnDeath() { }
}
