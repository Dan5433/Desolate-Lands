using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] string worldName;
    [SerializeField] GameObject worldCanvas;
    [SerializeField] GameObject player;
    [SerializeField] CursorTexture[] cursors;
    [SerializeField] Settings settings;
    int deaths;

    static CursorState cursorState;
    static string worldDirPath;
    static string playerDataDirPath;
    static string pendingWorldName = null;
    static bool isGamePaused = false;

    public static bool IsGamePaused => isGamePaused;
    public string WorldName => worldName;
    public GameObject WorldCanvas => worldCanvas;
    public GameObject Player => player;
    public static string DataDirPath => worldDirPath;
    public static string PlayerDataDirPath => playerDataDirPath;
    public static string PendingWorldName { get { return pendingWorldName; } set { pendingWorldName = value; } }
    
    public static CursorState CursorState {  
        get { return cursorState; } 
        set 
        {
            cursorState = value;
            Cursor.SetCursor(Instance.cursors.First(t => t.state == cursorState).texture,
                new(16f,16f), CursorMode.ForceSoftware);
        } 
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            if(pendingWorldName != null)
                worldName = pendingWorldName;
            Debug.Log("Loaded world: " + worldName);

            worldDirPath = Path.Combine(Application.persistentDataPath, "saves", worldName);
            playerDataDirPath = Path.Combine(worldDirPath, "player");

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

    public static void TogglePauseState()
    {
        isGamePaused = !isGamePaused;

        if(isGamePaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    private void Start()
    {
        CursorState = CursorState.Default;
    }

    private void OnDestroy()
    {
        UpdatePlaytime();
    }

    void UpdatePlaytime()
    {
        BinaryDataHandler handler = new(worldDirPath,MainMenuManager.StatsFileName);

        WorldStats stats = new();
        handler.LoadData(reader => stats = new(reader));

        Playtime playtime = stats.playtime;
        /*
        1h,10m,24s playtime
        1h,50m,50s time in scene (6650s)
         
        add seconds together => 6674s
        divide by 60 to get minutes => 111m
        mod by 60 to get remaining secs => 14s

        add minutes together => 121m
        divide by 60 to get hours => 2h
        mod by 60 to get remaining minutes => 1m

        add hours together => 3h

        join together => 3h, 1m, 14s
         */

        int totalSeconds = playtime.seconds + (int)Time.timeSinceLevelLoad;
        int minutesInScene = totalSeconds / 60;
        stats.playtime.seconds = totalSeconds % 60;

        int totalMinutes = playtime.minutes + minutesInScene;
        int hoursInScene = totalMinutes / 60;
        stats.playtime.minutes = totalMinutes % 60;

        stats.playtime.hours = playtime.hours + hoursInScene;

        stats.deaths += deaths;

        handler.SaveData(writer => stats.Write(writer));
    }

    public static void IncrementDeaths()
    {
        Instance.deaths++;
    }
    public void ExitToMain()
    {
        SceneManager.LoadScene(0);
    }
}

[Serializable]
struct CursorTexture
{
    public CursorState state;
    public Texture2D texture;
}

public enum CursorState
{
    Default = 0,
    Use = 1,
    Drop = 2
}

[Serializable]
public struct Settings
{
    public float holdTimeToMainMenu;
}