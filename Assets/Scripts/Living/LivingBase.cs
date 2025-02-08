using EditorAttributes;
using System.IO;
using UnityEngine;

public class LivingBase : MonoBehaviour, IDamageable
{
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth = 100f;
    static string dataDirPath;

    private void Awake()
    {
        dataDirPath = Path.Combine(GameManager.DataDirPath, "entities");
    }

    protected virtual void Start()
    {
        LoadHealth();
    }

    protected virtual string GetSaveKey()
    {
        return gameObject.name;
    }

    [Button("Damage", 30)]
    public virtual void Damage(float damageAmount)
    {
        health -= damageAmount;

        if(health <= 0) OnDeath();
    }

    [Button("Heal", 30)]
    public virtual bool Heal(float healAmount)
    {
        if (health == maxHealth) 
            return false;

        health = Mathf.Clamp(health + healAmount, 0, maxHealth);
        return true;
    }

    protected virtual void OnDeath() {}

    void LoadHealth()
    {
        var handler = new BinaryDataHandler(dataDirPath, GetSaveKey());

        if (!handler.FileExists())
        {
            health = maxHealth;
            return;
        }

        handler.LoadData(reader => health = reader.ReadSingle());
    }

    void SaveHealth()
    {
        var handler = new BinaryDataHandler(dataDirPath, GetSaveKey());

        handler.SaveData(writer => writer.Write(health));
    }

    void OnDestroy()
    {
        SaveHealth();
    }
}
