using CustomClasses;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu]
public class BreakableTile : Tile
{
    [SerializeField][Tooltip("In Seconds")] protected float breakingTime;
    [SerializeField] protected DropItem[] drops;
    [SerializeField][Tooltip("Used for Break Effect")] protected Color[] colors = { Color.white, Color.white };
    [SerializeField] protected EfficientTool tool;
    [SerializeField] protected RequiredMaterial minMaterial;
    public float BreakingTime { get { return breakingTime; } }
    public DropItem[] Drops { get { return drops; } }
    public Color[] Colors { get { return colors; } }
    public EfficientTool Tool { get { return tool; } }
    public RequiredMaterial MinMaterial { get { return minMaterial; } }
}
