using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] bool useJsonEncryption;
    [SerializeField] string worldName;
    [SerializeField] GameObject worldCanvas;
    [SerializeField] GameObject player;
    static string dataDirPath;
    static string playerDataDirPath;

    public bool UseJsonEncryption => useJsonEncryption;
    public string WorldName => worldName;
    public GameObject WorldCanvas => worldCanvas;
    public GameObject Player => player;
    public static string DataDirPath => dataDirPath;
    public static string PlayerDataDirPath => playerDataDirPath;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            dataDirPath = Path.Combine(Application.persistentDataPath, "saves", worldName);
            playerDataDirPath = Path.Combine(dataDirPath, "player");

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
