using CustomExtensions;
using EditorAttributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class WorldBorderManager : MonoBehaviour
{
    [SerializeField] ParticleSystem gasBorder;
    [SerializeField] DefaultPositions[] startingPositions;
    Dictionary<Vector2Int,LinkedList<GameObject>> loadedBorders = new();

    [Button("Unload Border", 25, true)]
    public void UnloadBorder(Vector2Int chunk)
    {
        if (!loadedBorders.ContainsKey(chunk)) return;

        foreach(var obj in loadedBorders[chunk]) 
        {
            Destroy(obj);
        }

        loadedBorders.Remove(chunk);
    }

    [Button("Load Border", 25, true)]
    public void LoadBorder(Vector2Int chunk, Direction direction)
    {
        Vector3 position = Array.Find(startingPositions, pos => pos.direction == direction).position;

        var border = Instantiate(gasBorder, 
            (Vector3Int)(chunk * TerrainManager.ChunkSize) + position, 
            new Vector3(0, 0, (int)direction * 90).AnglesToQuaternion());

        var shape = border.shape;
        var main = border.main;
        var emission = border.emission;

        int size = direction == Direction.Up || direction == Direction.Down ? 
            TerrainManager.ChunkSize.x / 2 : TerrainManager.ChunkSize.y / 2;

        shape.radius *= size;
        main.maxParticles *= size;
        emission.rateOverTimeMultiplier *= size;

        border.Restart();

        if (!loadedBorders.TryGetValue(chunk, out var list)){
            list = new();
        }

        list.AddLast(border.gameObject);
        loadedBorders[chunk] = list;
    }

    [Serializable]
    struct DefaultPositions
    {
        public Direction direction;
        public Vector3 position;
    }
}
