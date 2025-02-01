using System;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static GameSettings settings;
    //TODO: add settings
}

[Serializable]
public struct GameSettings
{
    public float returnTimeToMenu;
}