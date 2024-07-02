using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] bool useJsonEncryption;
    [SerializeField] string worldName;
    [SerializeField] GameObject worldCanvas;
    [SerializeField] GameObject player;

    public bool UseJsonEncryption { get { return useJsonEncryption; } }
    public string WorldName { get { return  worldName; } }
    public GameObject WorldCanvas { get { return worldCanvas; } }
    public GameObject Player { get { return player; } }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            if (!PlayerPrefs.HasKey(worldName))
            {
                Debug.Log("Initializing new seed for world: "+worldName);
                PlayerPrefs.SetInt(worldName, Random.Range(int.MinValue,int.MaxValue));
            }

            int seed = PlayerPrefs.GetInt(worldName);
            Random.InitState(seed);

            Debug.Log("Set seed to " + seed);

            Instance = this;
        }
    }
}
