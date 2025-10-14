using System;
using UnityEngine;

public class RNGManager : MonoBehaviour
{
    public static RNGManager Instance { get; private set; }

    RandomGenerators generators;

    public RandomGenerators Generators => generators;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}

[Serializable]
public struct RandomGenerators
{
    public SeededRandom worldgen;
    public SeededRandom loot;
    public SeededRandom damage;
}