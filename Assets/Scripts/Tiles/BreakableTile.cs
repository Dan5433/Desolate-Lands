using CustomClasses;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu(menuName = "2D/Tiles/Breakable Tile")]
public class BreakableTile : RuleTile
{
    [SerializeField][Tooltip("In Seconds")] protected float breakingTime;
    [SerializeField] protected DropItem[] drops;
    [SerializeField][Tooltip("Used for Break Effect")] protected Color[] colors = { Color.white, Color.white };
    [SerializeField] protected AudioClip[] breakingAudio;
    [SerializeField] protected ToolType tool;
    [SerializeField] protected ItemMaterial minMaterial;
    public float BreakingTime => breakingTime;
    public DropItem[] Drops => drops;
    public Color[] Colors => colors;
    public AudioClip[] BreakingAudio => breakingAudio;
    public ToolType Tool => tool;
    public ItemMaterial MinMaterial => minMaterial;
}
