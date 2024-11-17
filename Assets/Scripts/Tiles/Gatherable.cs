using System;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Gatherable : MonoBehaviour
{
    [SerializeField] GatherableTileState state;
    Vector3Int tilePosition;
    float resetTimePerState;
    float currentStateTimer;
    BinaryDataHandler dataHandler;

    public Vector3Int TilePosition => tilePosition;
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
        dataHandler = new(GameManager.DataDirPath, filePath);

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
        var tile = GetComponentInParent<Tilemap>().GetTile<GatherableTile>(tilePosition);
        var sprite = Array.Find(tile.Sprites, s => s.state == state).sprite;

        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    void SaveState()
    {
        dataHandler.SaveData(writer =>
        {
            writer.Write((int)state);
            writer.Write(currentStateTimer);
        });
    }

    void LoadState()
    {
        if (!dataHandler.FileExists()) return;

        dataHandler.LoadData(reader =>
        {
            state = (GatherableTileState)reader.ReadInt32();
            currentStateTimer = reader.ReadSingle();
        });

        UpdateState();
    }

    void OnDestroy()
    {
        if(state != GatherableTileState.Replenished) SaveState();
    }
}