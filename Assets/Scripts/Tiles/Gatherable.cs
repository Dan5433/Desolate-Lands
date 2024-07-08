using System;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Gatherable : MonoBehaviour
{
    [SerializeField] GatherableTileState state;
    Vector3Int tilePosition;
    float resetTimePerState;
    float currentStateTimer;
    JsonFileDataHandler dataHandler;

    public Vector3Int TilePosition { get { return tilePosition; } }
    public GatherableTileState State
    {
        set
        {
            state = value;
            UpdateState();
        }

        get { return state; }
    }


    public void Init(float resetTime, Vector3Int position)
    {
        string filePath = Path.Combine("Terrain", "Gatherable" + transform.position);
        dataHandler = new(GameManager.Instance.DataDirPath, filePath);

        tilePosition = position;

        int numberOfStates = (int)GatherableTileState.Replenished;
        resetTimePerState = resetTime / numberOfStates;

        LoadState();
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

    async void SaveState()
    {

        GatherableStateData data = new()
        {
            state = state,
            timer = currentStateTimer
        };

        await dataHandler.SaveDataAsync(data);
    }

    async void LoadState()
    {

        var data = await dataHandler.LoadDataAsync<GatherableStateData>();
        state = data != null ? data.state : GatherableTileState.Replenished;
        currentStateTimer = data != null ? data.timer : 0;

        UpdateState();
    }

    void OnDestroy()
    {
        SaveState();
    }
}

[Serializable]
public class GatherableStateData
{
    public GatherableTileState state;
    public float timer;
}