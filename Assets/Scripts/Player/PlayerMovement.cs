using EditorAttributes;
using System.IO;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float vertInput;
    float horInput;
    [SerializeField] float speed = 0.015f;
    [SerializeField] Texture2D cursor;
    [SerializeField] Transform cam;
    const string saveString = "Position";

    void Start()
    {
        //Cursor.SetCursor(cursor,Vector2.zero,CursorMode.Auto);         
        Cursor.lockState = CursorLockMode.Confined;

        LoadPosition();
    }

    void FixedUpdate()
    {
        vertInput = Input.GetAxis("Vertical");
        horInput = Input.GetAxis("Horizontal");

        transform.Translate(new Vector2(horInput, vertInput).normalized * speed);
    }

    [Button("Get Chunk Index",36)]
    void PrintCurrentChunk()
    {
        Debug.Log(TerrainManager.GetChunkIndexFromPosition(transform.position));
    }

    void OnDestroy()
    {
        SavePosition();
    }

    async void LoadPosition()
    {
        string dirPath = Path.Combine(GameManager.Instance.DataDirPath, "Player");
        JsonFileDataHandler dataHandler = new(dirPath, saveString);

        var save = await dataHandler.LoadDataAsync<PlayerPosition>();

        if (save != null) transform.position = save.position;
    }

    async void SavePosition()
    {
        PlayerPosition save = new() { position = transform.position };

        string dirPath = Path.Combine(GameManager.Instance.DataDirPath, "Player");
        JsonFileDataHandler dataHandler = new(dirPath, saveString);

        await dataHandler.SaveDataAsync(save);
    }

    class PlayerPosition
    {
        public Vector3 position;
    }
}
