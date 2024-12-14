using EditorAttributes;
using UnityEngine;

public class LivingBase : MonoBehaviour, IDamageable
{
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;

    protected virtual void Start()
    {
        //TODO: implement saving and loading health
        health = maxHealth;
    }

    public virtual void Damage(float damageAmount)
    {
        health -= damageAmount;

        if(health <= 0) OnDeath();
    }

    public virtual bool Heal(float healAmount)
    {
        if (health == maxHealth) return false;

        health = Mathf.Clamp(health + healAmount, 0, maxHealth);
        return true;
    }

    protected virtual void OnDeath() { }
}
