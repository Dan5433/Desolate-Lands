using EditorAttributes;

public interface IDamageable
{
    [Button("Damage", 30)]
    public void Damage(float damageAmount);

    [Button("Heal", 30)]
    //return boolean to check whether the heal actually healed
    public bool Heal(float healAmount);
}