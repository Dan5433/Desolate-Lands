using EditorAttributes;

public interface IDamageable
{
    public void Damage(float damageAmount);

    //return boolean to check whether the heal actually healed
    public bool Heal(float healAmount);
}