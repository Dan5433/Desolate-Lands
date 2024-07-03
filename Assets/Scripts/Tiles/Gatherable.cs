using UnityEngine;

public class Gatherable : MonoBehaviour
{
    [SerializeField] float regenerateTime;
    float currentStateTimer;

    public float RegenerateTime { set { regenerateTime = value; } }

    void Update()
    {
        if(currentStateTimer < regenerateTime) 
        {
            currentStateTimer += Time.deltaTime;
        }
    }
}
