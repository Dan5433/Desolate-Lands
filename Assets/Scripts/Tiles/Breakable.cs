using EditorAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Breakable : MonoBehaviour, IDamageable
{
    [SerializeField] float health = 100f;

    private void Awake()
    {
        //TODO: save and load health
        UpdateSprite();
    }

    public void Damage(float damageAmount)
    {
        var tilemap = GetComponentInParent<Tilemap>();
        float hardness = tilemap.GetTile<BreakableTile>(
            tilemap.WorldToCell(transform.position)).Hardness;

        health -= damageAmount / hardness;
        UpdateSprite();

        if (health <= 0) OnBreak();
    }

    void UpdateSprite()
    {
        float percentBroken = (100f - health) / 100f;

        if (percentBroken == 0) return;

        for (int i = 0; i < BreakingManager.Instance.BreakSprites.Length; i++)
        {
            float highRange = (float)(i + 1) / BreakingManager.Instance.BreakSprites.Length;
            float lowRange = (float)i / BreakingManager.Instance.BreakSprites.Length;

            if (percentBroken >= lowRange && percentBroken <= highRange)
            {
                GetComponent<SpriteRenderer>().sprite = BreakingManager.Instance.BreakSprites[i];
                break;
            }
        }
    }

    public bool Heal(float healAmount)
    {
        health += healAmount;
        return true;
    }

    public void OnBreak()
    {
        Debug.Log(GetComponents<IBreakable>().Length);
    }
}
