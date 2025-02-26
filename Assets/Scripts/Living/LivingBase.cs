using EditorAttributes;
using System.Collections;
using System.IO;
using UnityEngine;

public class LivingBase : MonoBehaviour, IDamageable
{
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float spawnProtectionTime = 1f;
    bool invulnerable = false;
    static string dataDirPath;

    private void Awake()
    {
        dataDirPath = Path.Combine(GameManager.DataDirPath, "entities");
    }

    protected virtual void Start()
    {
        LoadHealth();
        StartCoroutine(InvokeSpawnProtection());
    }
    void OnDestroy()
    {
        SaveHealth();
    }

    protected virtual string GetSaveKey()
    {
        return gameObject.name;
    }

    [Button("Damage", 30)]
    public virtual void Damage(float damageAmount)
    {
        if (invulnerable)
            return;

        health -= damageAmount;

        if(health <= 0) 
            OnDeath();
    }

    [Button("Heal", 30)]
    public virtual bool Heal(float healAmount)
    {
        if (health == maxHealth) 
            return false;

        health = Mathf.Clamp(health + healAmount, 0, maxHealth);
        return true;
    }

    protected virtual void OnDeath() { }

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

    IEnumerator InvokeSpawnProtection()
    {
        invulnerable = true;
        yield return new WaitForSeconds(spawnProtectionTime);
        invulnerable = false;
    }
}
