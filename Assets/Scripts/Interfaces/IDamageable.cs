using EditorAttributes;

public interface IDamageable
{
    public bool Damage(float damageAmount);

    //return boolean to check whether the heal actually healed
    public bool Heal(float healAmount);
}