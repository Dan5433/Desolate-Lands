using System.Collections.Generic;
using UnityEngine;

public class MineAnimationManager : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float revealDist = 3f;
    LinkedList<Transform> mineList = new();

    void Update()
    {
        var colorLerpValue = Mathf.PingPong(Time.time, 1);
        float revealMultiplier = player.GetComponent<CustomPlayerData>().MineRevealDistMultiplier;

        foreach (Transform mine in mineList)
        {
            var skull = mine.Find("skull").GetComponent<SpriteRenderer>();
            skull.color = Color.Lerp(Color.white, Color.red, colorLerpValue);

            float distanceToPlayer = Vector2.Distance(mine.position, player.position);

            foreach (var renderer in mine.GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b,
                    1 - distanceToPlayer * 1 / (revealDist * revealMultiplier));
            }
        }
    }

    public void AddMine(Transform mine)
    {
        mineList.AddLast(mine);
    }

    public void DeleteMine(Transform mine)
    {
        mineList.Remove(mine);
    }

}
