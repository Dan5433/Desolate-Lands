using UnityEngine;
using UnityEngine.Tilemaps;

public class Mine : MonoBehaviour
{
    Transform player;
    [SerializeField] float revealDist = 3f;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    void Update()
    {
        SpriteRenderer skull = transform.Find("skull").GetComponent<SpriteRenderer>();
        skull.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 1));

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float revealMultiplier = player.GetComponent<CustomPlayerData>().MineRevealDistMultiplier;

        foreach(var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b,
                1 - distanceToPlayer * 1 / (revealDist * revealMultiplier));
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Destroy(gameObject);
        }

        if (collision.transform.CompareTag("Item"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }

        var tilePosition = transform.parent.GetComponent<Tilemap>().WorldToCell(transform.position);
        SaveTerrain.RemoveTileSaveData(tilePosition, transform.parent.name);
    }
}
