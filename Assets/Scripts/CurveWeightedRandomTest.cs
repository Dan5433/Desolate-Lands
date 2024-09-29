using EditorAttributes;
using UnityEngine;

public class CurveWeightedRandomTest : MonoBehaviour
{
    [SerializeField] AnimationCurve curve;

    [Button("Test Weighted Random",25f)]
    void WeightedRandom(Vector2Int minmax, int iterations)
    {
        for(int i = 0; i < iterations; i++)
        {
            float weightedValue = curve.Evaluate(Random.value);
            Debug.Log((int)Mathf.Lerp(minmax.x, minmax.y, weightedValue));
        }
    }
}
