using CustomClasses;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu(menuName = "2D/Tiles/Breakable Tile")]
public class BreakableTile : RuleTile
{
    [SerializeField][Min(0.0001f)] protected float hardness = 1f;
    [SerializeField] protected DropItem[] drops;
    [SerializeField][Tooltip("Used for Break Effect")] protected Color[] colors = { Color.white, Color.white };
    [SerializeField] protected AudioClip[] breakingAudio;
    [SerializeField] protected ToolType tool;
    [SerializeField] protected ItemMaterial minMaterial;
    public float Hardness => hardness;
    public DropItem[] Drops => drops;
    public Color[] Colors => colors;
    public AudioClip[] BreakingAudio => breakingAudio;
    public ToolType Tool => tool;
    public ItemMaterial MinMaterial => minMaterial;
}
