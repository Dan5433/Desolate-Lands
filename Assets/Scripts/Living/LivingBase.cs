using EditorAttributes;
using System.Collections;
using UnityEngine;

public class LivingBase : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] protected int maxHealth;

    protected virtual void Start()
    {
        //TODO: implement saving and loading health
        health = maxHealth;
    }

    [Button("Damage",30)]
    public virtual void Damage(int damageAmount)
    {
        health -= damageAmount;

        if(health <= 0) OnDeath();
    }

    [Button("Heal", 30)]
    public virtual void Heal(int healAmount)
    {
        health = Mathf.Clamp(health + healAmount, 0, maxHealth);
    }

    protected virtual void OnDeath() { }
}
