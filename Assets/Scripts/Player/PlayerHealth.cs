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
    Color normalColor;

    override protected void Start()
    {
        base.Start();

        normalColor = barImage.color;

        healthBar.maxValue = maxHealth;
        healthBar.value = health;

        UpdateDisplay();
    }

    public override void Damage(float damageAmount)
    {
        base.Damage(damageAmount);

        if (health <= 0) return;

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
        StartCoroutine(LerpBarValue(health));
        healthText.text = Mathf.RoundToInt(health).ToString();
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

    protected override void OnDeath()
    {
        //TODO: add game over functionality
    }
}
