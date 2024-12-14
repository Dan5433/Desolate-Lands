using EditorAttributes;

public interface IDamageable
{
    [Button("Damage", 30)]
    public void Damage(float damageAmount);

    [Button("Heal", 30)]
    public bool Heal(float healAmount);
}