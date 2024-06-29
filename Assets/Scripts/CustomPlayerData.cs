using UnityEngine;

public class CustomPlayerData : MonoBehaviour
{
    [SerializeField] float mineRevealDistMultiplier = 1f;
    public float MineRevealDistMultiplier { 
        get { return mineRevealDistMultiplier; } 
        set { mineRevealDistMultiplier = value; }
    }
}
