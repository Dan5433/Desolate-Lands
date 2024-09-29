using CustomClasses;
using System;
using EditorAttributes;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "2D/Tiles/Breakable Rule Tile")]
public class BreakableRuleTile : RuleTile
{
    [SerializeField][Tooltip("In Seconds")] protected float breakingTime;
    [SerializeField] protected DropItem[] drops;
    [SerializeField][Tooltip("Used for Break Effect")] protected Color[] colors = { Color.white, Color.white };
    [SerializeField] protected ToolType tool;
    [SerializeField] protected ItemMaterial minMaterial;
    public float BreakingTime { get { return breakingTime; } }
    public DropItem[] Drops { get { return drops; } }
    public Color[] Colors { get { return colors; } }
    public ToolType Tool { get { return tool; } }
    public ItemMaterial MinMaterial { get { return minMaterial; } }
}
