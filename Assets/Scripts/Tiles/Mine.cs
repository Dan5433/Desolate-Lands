using UnityEngine;
using UnityEngine.Tilemaps;

public class Mine : MonoBehaviour
{
    Transform player;
    Tilemap parent;
    float pulseStage = 0f;
    bool pulseUp = true;
    [SerializeField] float pulseSpeed = 0.03f;
    [SerializeField] float revealDist = 3f;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        parent = GameObject.Find("Top").GetComponent<Tilemap>();
    }

    void FixedUpdate()
    {

        foreach (Transform child in transform)
        {
            SpriteRenderer rend = child.GetComponent<SpriteRenderer>();
            float dist = Vector2.Distance(transform.position, player.position);

            if (child.name == "skull")
            {
                rend.color = Color.Lerp(Color.white, Color.red, pulseStage);
                pulseStage += pulseUp ? pulseSpeed : -pulseSpeed;

                if (pulseStage >= 1f)
                {
                    pulseUp = false;
                    pulseStage = 1f;
                }
                if (pulseStage <= 0f)
                {
                    pulseUp = true;
                    pulseStage = 0f;
                }
            }

            float revealMultiplier = player.GetComponent<CustomPlayerData>().MineRevealDistMultiplier;
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 
                                   1 - dist * 1 / (revealDist*revealMultiplier));
        }
    }
                    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            PlayerPrefs.DeleteKey(parent.name + parent.WorldToCell(transform.position));
            Destroy(gameObject);
        }

        if (collision.transform.CompareTag("Item"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
