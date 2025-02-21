using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : LivingBase
{
    [SerializeField] Slider healthBar;
    [SerializeField] TMP_Text healthText;
    [SerializeField] Image barImage;
    [SerializeField] Color damageColor;
    [SerializeField] Color healColor;
    [SerializeField] float colorLerpSpeed;
    [SerializeField] float healthLerpSpeed;
    [SerializeField] DeathMenu deathMenu;
    Color normalColor;

    override protected void Start()
    {
        base.Start();

        normalColor = barImage.color;

        //respawn after loading if exited without respawning
        if (health <= 0f)
            Respawn();

        Reset();
    }

    public override void Damage(float damageAmount)
    {
        base.Damage(damageAmount);

        if (health <= 0) 
            return;

        StopAllCoroutines();

        UpdateDisplay();
        StartCoroutine(LerpBarColor(damageColor));
        GetComponent<PlayerAudio>().PlaySoundEffect(PlayerSounds.Hurt);
    }


    public override bool Heal(float healAmount)
    {
        if (!base.Heal(healAmount)) return false;

        StopAllCoroutines();

        UpdateDisplay();
        StartCoroutine(LerpBarColor(healColor));

        return true;
    }

    void UpdateDisplay()
    {
        healthText.text = Mathf.CeilToInt(health).ToString();
        StartCoroutine(LerpBarValue(health));
    }

    IEnumerator LerpBarValue(float targetHealth)
    {
        float time = 0;
        while (healthBar.value != targetHealth)
        {
            time += Time.deltaTime * healthLerpSpeed;
            healthBar.value = Mathf.Lerp(healthBar.value, targetHealth, time);
            yield return null;
        };
    }

    IEnumerator LerpBarColor(Color targetColor)
    {
        float time = 0;
        while (barImage.color != targetColor)
        {
            time += Time.deltaTime * colorLerpSpeed;
            barImage.color = Color.Lerp(normalColor, targetColor, time);
            yield return null;

        };

        time = 0;
        while (barImage.color != normalColor)
        {
            time += Time.deltaTime * colorLerpSpeed;
            barImage.color = Color.Lerp(barImage.color, normalColor, time);
            yield return null;

        };
    }

    private void Reset()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
        UpdateDisplay();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        healthBar.value = 0;
        healthText.text = "0";

        foreach (var item in GetComponent<PlayerInventory>().Inventory)
            ItemManager.SpawnGroundItem(item.Clone(), transform.position, new(3f, 3f));

        foreach (var item in GetComponent<SpecialPlayerInventory>().Inventory)
            ItemManager.SpawnGroundItem(item.Clone(), transform.position, new(3f, 3f));

        GetComponent<PlayerInventory>().ClearInventory();
        GetComponent<SpecialPlayerInventory>().ClearInventory();

        deathMenu.Death();
        GameManager.IncrementDeaths();
    }

    public void Respawn()
    {
        health = maxHealth;
        transform.position = Vector3.zero;
        Reset();
    }
}
