using EditorAttributes;
using System.IO;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float vertInput;
    float horInput;
    [SerializeField] float speed = 0.015f;
    [SerializeField] PlayerSprites sprites;
    [SerializeField] SpriteRenderer gfx;
    const string saveString = "Position";

    void Start()
    {      
        Cursor.lockState = CursorLockMode.Confined;

        LoadPosition();
    }

    void FixedUpdate()
    {
        vertInput = Input.GetAxis("Vertical");
        horInput = Input.GetAxis("Horizontal");

        transform.Translate(new Vector2(horInput, vertInput).normalized * speed);
        UpdateSprite(vertInput, horInput);
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

    void UpdateSprite(float vertInput, float horInput)
    {
        if (vertInput > 0)
        {
            gfx.sprite = sprites.north;
        }
        if (vertInput < 0)
        {
            gfx.sprite = sprites.south;
        }

        if (horInput > 0)
        {
            gfx.sprite = sprites.east;
        }
        if (horInput < 0)
        {
            gfx.sprite = sprites.west;
        }
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

    [Serializable]
    struct PlayerSprites
    {
        public Sprite north;
        public Sprite east;
        public Sprite south;
        public Sprite west;
        public Sprite ne;
        public Sprite se;
        public Sprite sw;
        public Sprite nw;
    }
}
