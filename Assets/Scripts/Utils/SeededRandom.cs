using UnityEngine;

//Class for deterministic random game generation 
public class SeededRandom
{
    Random.State state;
    bool isInitialized = false;
    public Random.State State => state;
    public float Value
    {
        get
        {
            ValidateState();
            float val = Random.value;
            UpdateState();
            return val;
        }
    }

    public SeededRandom(Random.State initState)
    {
        Init(initState);
    }

    void Init(Random.State initState)
    {
        state = initState;
        isInitialized = true;
    }

    void ValidateState()
    {
        if (!isInitialized)
        {
            Debug.LogWarning("RNG not initialized with seed!");
            Init(Random.state);
        }

        if (Random.state.Equals(state))
            return;

        Random.state = state;
    }

    void UpdateState()
    {
        state = Random.state;
    }

    public int Range(int minInclusive, int maxExclusive)
    {
        ValidateState();
        int val = Random.Range(minInclusive, maxExclusive);
        UpdateState();
        return val;
    }

    public float Range(float min, float max)
    {
        ValidateState();
        float val = Random.Range(min, max);
        UpdateState();
        return val;
    }
}
