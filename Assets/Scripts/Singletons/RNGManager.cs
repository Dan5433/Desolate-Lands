using System;
using UnityEngine;

public class RNGManager : MonoBehaviour
{
    public static RNGManager Instance { get; private set; }

    [SerializeField] RandomGenerators generators;

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

            string json = PlayerPrefs.GetString(GameManager.Instance.WorldName);
            generators = new(json);
        }
    }

    private void OnDestroy()
    {
        generators.SaveState(GameManager.Instance.WorldName);
    }
}

[Serializable]
public struct RandomGenerators
{
    public SeededRandom worldgen;
    public SeededRandom loot;
    public SeededRandom damage;

    public readonly void SaveState(string key)
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(key, json);
    }

    public RandomGenerators(string json)
    {
        var globalState = JsonUtility.FromJson<RandomStateWrapper>(json);
        if (globalState.s0 != 0 && globalState.s1 != 0 && globalState.s2 != 0 && globalState.s3 != 0)
        {
            Debug.Log("Global seed save format found. Setting all generators to same saved state...");

            this = new(globalState);
            return;
        }

        this = JsonUtility.FromJson<RandomGenerators>(json);
        Debug.Log("Loaded RNG saved state");
    }

    RandomGenerators(RandomStateWrapper globalState)
    {
        worldgen = new(globalState);
        loot = new(globalState);
        damage = new(globalState);
    }
}
