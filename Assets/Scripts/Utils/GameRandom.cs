using UnityEngine;

//Class for deterministic random game generation 
public static class GameRandom
{
    static Random.State state;
    public static Random.State State => state;

    static void ValidateState()
    {
        if (Random.state.Equals(state))
            return;

        Random.state = state;
    }

    static void UpdateState()
    {
        state = Random.state;
    }

    public static void Init(Random.State state)
    {
        GameRandom.state = state;
    }

    public static float Value
    {
        get
        {
            ValidateState();
            float val = Random.value;
            UpdateState();
            return val;
        }
    }

    public static int Range(int minInclusive, int maxExclusive)
    {
        ValidateState();
        int val = Random.Range(minInclusive, maxExclusive);
        UpdateState();
        return val;
    }

    public static float Range(float min, float max)
    {
        ValidateState();
        float val = Random.Range(min, max);
        UpdateState();
        return val;
    }
}
