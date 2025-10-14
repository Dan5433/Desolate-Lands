using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] string worldName;
    [SerializeField] GameObject worldCanvas;
    [SerializeField] GameObject player;
    [SerializeField] TMP_Text seedText;
    [SerializeField] CursorTexture[] cursors;
    int deaths;

    static CursorState cursorState;
    static string worldDirPath;
    static string playerDataDirPath;
    static string pendingWorldName = null;
    static string pendingSeed;
    static bool isGamePaused = false;

    public static bool IsGamePaused => isGamePaused;
    public GameObject WorldCanvas => worldCanvas;
    public GameObject Player => player;
    public static string DataDirPath => worldDirPath;
    public static string PlayerDataDirPath => playerDataDirPath;
    public static string PendingWorldName { get { return pendingWorldName; } set { pendingWorldName = value; } }
    public static string PendingSeed { get { return pendingSeed; } set { pendingSeed = value; } }

    public static CursorState CursorState
    {
        get { return cursorState; }
        set
        {
            cursorState = value;
            Cursor.SetCursor(Instance.cursors.First(t => t.state == cursorState).texture,
                new(16f, 16f), CursorMode.ForceSoftware);
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
            if (pendingWorldName != null)
                worldName = pendingWorldName;

            seedText.text = pendingSeed;

            Debug.Log("Loaded world: " + worldName);

            worldDirPath = Path.Combine(Application.persistentDataPath, "saves", worldName);
            playerDataDirPath = Path.Combine(worldDirPath, "player");

            Instance = this;
        }
    }

    private void Start()
    {
        CursorState = CursorState.Default;
    }

    private void OnDestroy()
    {
        SaveRandomState();
        UpdatePlaytime();
    }

    void SaveRandomState()
    {
        RandomStateWrapper wrapper = new(SeededRandom.State);
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(worldName, json);
    }

    public static void TogglePauseState()
    {
        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }
        else
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }

    void UpdatePlaytime()
    {
        BinaryDataHandler handler = new(worldDirPath, MainMenuManager.StatsFileName);

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

    public static void ExitToMain()
    {
        if (isGamePaused)
            TogglePauseState();

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
public struct RandomStateWrapper
{
    public int s0, s1, s2, s3;

    public RandomStateWrapper(int s0, int s1, int s2, int s3)
    {
        this.s0 = s0;
        this.s1 = s1;
        this.s2 = s2;
        this.s3 = s3;
    }

    public RandomStateWrapper(Hash128 hash)
    {
        string hex = hash.ToString();
        s0 = (int)Convert.ToUInt32(hex.Substring(0, 8), 16);
        s1 = (int)Convert.ToUInt32(hex.Substring(8, 8), 16);
        s2 = (int)Convert.ToUInt32(hex.Substring(16, 8), 16);
        s3 = (int)Convert.ToUInt32(hex.Substring(24, 8), 16);
    }

    public RandomStateWrapper(Random.State state)
    {
        this = JsonUtility.FromJson<RandomStateWrapper>(JsonUtility.ToJson(state));
    }

    public static implicit operator Random.State(RandomStateWrapper wrapper)
    {
        return JsonUtility.FromJson<Random.State>(JsonUtility.ToJson(wrapper));
    }
}