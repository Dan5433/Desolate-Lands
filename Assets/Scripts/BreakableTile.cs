using CustomClasses;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu]
public class BreakableTile : Tile
{
    [SerializeField][Tooltip("In Seconds")] float breakingTime;
    [SerializeField] CustomClasses.DropItem[] drops;
    [SerializeField][Tooltip("Used for Break Effect")] Color[] colors = { Color.white, Color.white };
    [SerializeField] EfficientTool tool;
    [SerializeField] RequiredMaterial minMaterial;
    public float BreakingTime { get { return breakingTime; } }
    public CustomClasses.DropItem[] Drops { get { return drops; } }
    public Color[] Colors { get { return colors; } }
    public EfficientTool Tool { get { return tool; } }
    public RequiredMaterial MinMaterial { get { return minMaterial; } }
}
