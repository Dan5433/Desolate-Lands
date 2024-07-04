using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Gatherable : MonoBehaviour
{
    [SerializeField] GatherableTileState state;
    Vector3Int tilePosition;
    float resetTimePerState;
    float currentStateTimer;

    public GatherableTileState State { get { return state; } }

    public void Init(float resetTime, Vector3Int position)
    {
        tilePosition = position;

        int numberOfStates = Enum.GetNames(typeof(GatherableTileState)).Length - 1;
        resetTimePerState = resetTime / numberOfStates;

        state = GatherableTileState.Replenished;
        UpdateState();
    }

    void Update()
    {
        if (state == GatherableTileState.Replenished) return;

        if (currentStateTimer < resetTimePerState)
        {
            currentStateTimer += Time.deltaTime;
        }
        else
        {
            state += 1;
            currentStateTimer = 0;
            UpdateState();
        }
    }

    void UpdateState()
    {
        GetComponentInParent<Tilemap>().RefreshTile(tilePosition);
    }
}
