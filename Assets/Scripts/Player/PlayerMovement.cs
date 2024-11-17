using EditorAttributes;
using System.IO;
using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public class PlayerMovement : MonoBehaviour
{
    float vertInput;
    float horInput;
    [SerializeField] float speed = 0.015f;
    [SerializeField] PlayerSprites sprites;
    [SerializeField] SpriteRenderer gfx;
    const string saveString = "Position.bin";

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

    [Button("Get Chunk Index", 36)]
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
    void SavePosition()
    {
        string dirPath = Path.Combine(GameManager.DataDirPath, "Player");
        var handler = new BinaryDataHandler(dirPath, saveString);

        handler.SaveData(writer =>
        {
            writer.Write(transform.position.x);
            writer.Write(transform.position.y);
        });
    }

    void LoadPosition()
    {
        string dirPath = Path.Combine(GameManager.DataDirPath, "Player");
        BinaryDataHandler dataHandler = new(dirPath, saveString);

        dataHandler.LoadData(reader =>
        {
            transform.position = new(reader.ReadSingle(), reader.ReadSingle());
        });
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
