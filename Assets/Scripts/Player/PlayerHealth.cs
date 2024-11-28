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
    [SerializeField] float animationSpeed;
    Color normalColor;

    override protected void Start()
    {
        base.Start();

        normalColor = barImage.color;

        healthBar.maxValue = maxHealth;
        healthBar.value = health;
    }

    IEnumerator DamageAnimation()
    {
        float time = 0;
        while(barImage.color != damageColor)
        {
            time += Time.deltaTime * animationSpeed;
            barImage.color = Color.Lerp(normalColor, damageColor, time);
            yield return null;

        };

        while (barImage.color != normalColor)
        {
            time -= Time.deltaTime * animationSpeed;
            barImage.color = Color.Lerp(normalColor, damageColor, time);
            yield return null;

        };
    }

    IEnumerator HealAnimation()
    {
        float time = 0;
        while (barImage.color != healColor)
        {
            time += Time.deltaTime * animationSpeed;
            barImage.color = Color.Lerp(normalColor, healColor, time);
            yield return null;

        };

        while (barImage.color != normalColor)
        {
            time -= Time.deltaTime * animationSpeed;
            barImage.color = Color.Lerp(normalColor, healColor, time);
            yield return null;

        };
    }

    void UpdateDisplay()
    {
        healthBar.value = health;
        healthText.text = health.ToString();
    }

    public override void Damage(int damageAmount)
    {
        base.Damage(damageAmount); 
        UpdateDisplay();
        StartCoroutine(DamageAnimation());
    }

    public override void Heal(int damageAmount)
    {
        base.Heal(damageAmount); 
        UpdateDisplay();
        StartCoroutine(HealAnimation());
    }
}
