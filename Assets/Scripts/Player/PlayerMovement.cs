using EditorAttributes;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float vertInput;
    float horInput;
    [SerializeField] float speed = 0.015f;
    [SerializeField] PlayerSprites sprites;
    [SerializeField] SpriteRenderer gfx;
    [SerializeField] Animator playerAnimator;
    [SerializeField] CapsuleCollider2D feetCollider;
    [SerializeField] BoxCollider2D bodyCollider;
    [SerializeField] float sidewaysColliderResize;
    [SerializeField] Transform interactPointer;
    [SerializeField] float pointerOffset;
    [SerializeField] MinimapPointerSprites minimapPointer;
    Vector2 pointerOrigin;
    float feetWidth, bodyWidth;
    Direction facing;
    Rigidbody2D rb;
    const string saveString = "Position.bin";

    public Direction Facing => facing;

    private void Awake()
    {
        pointerOrigin = interactPointer.localPosition;
        feetWidth = feetCollider.size.x;
        bodyWidth = bodyCollider.size.x;

        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        LoadPosition();
    }

    void FixedUpdate()
    {
        vertInput = Input.GetAxis("Vertical");
        horInput = Input.GetAxis("Horizontal");

        playerAnimator.SetFloat("vertInput", vertInput);
        playerAnimator.SetFloat("horInput", horInput);

        rb.velocity = new Vector2(horInput, vertInput).normalized * speed;

        UpdateSprite(vertInput, horInput);
        if (UpdateFacingDirection())
        {
            UpdateHitbox();
            UpdatePointerOffset();
            minimapPointer.UpdateSprite(facing);
        }
    }

    [Button("Get Chunk Index", 36)]
    void PrintCurrentChunk()
    {
        Debug.Log(TerrainManager.GetChunkIndex(transform.position));
    }

    void OnDestroy()
    {
        SavePosition();
    }

    bool UpdateFacingDirection()
    {
        if (gfx.sprite == sprites.north)
        {
            if (facing == Direction.Up)
                return false;
            facing = Direction.Up;
        }
        if (gfx.sprite == sprites.east)
        {
            if (facing == Direction.Right)
                return false;
            facing = Direction.Right;
        }
        if (gfx.sprite == sprites.south)
        {
            if (facing == Direction.Down)
                return false;
            facing = Direction.Down;
        }
        if (gfx.sprite == sprites.west)
        {
            if (facing == Direction.Left)
                return false;
            facing = Direction.Left;
        }

        return true;
    }

    void UpdateSprite(float vertInput, float horInput)
    {
        if (horInput > 0)
            gfx.sprite = sprites.east;
        if (horInput < 0)
            gfx.sprite = sprites.west;

        //check vertical last to prioritize vertical sprites
        if (vertInput > 0)
            gfx.sprite = sprites.north;
        if (vertInput < 0)
            gfx.sprite = sprites.south;
    }

    void UpdateHitbox()
    {
        if (facing == Direction.Up || facing == Direction.Down)
        {
            feetCollider.size = new(feetWidth, feetCollider.size.y);
            bodyCollider.size = new(bodyWidth, bodyCollider.size.y);
        }
        else
        {
            feetCollider.size = new(feetWidth - sidewaysColliderResize, feetCollider.size.y);
            bodyCollider.size = new(bodyWidth - sidewaysColliderResize, bodyCollider.size.y);
        }
    }

    void UpdatePointerOffset()
    {
        interactPointer.localPosition = facing switch
        {
            Direction.Up => pointerOrigin,
            Direction.Right => new(pointerOrigin.x + pointerOffset, pointerOrigin.y),
            Direction.Down => new(pointerOrigin.x, pointerOrigin.y - pointerOffset),
            Direction.Left => new(pointerOrigin.x - pointerOffset, pointerOrigin.y),
            _ => pointerOrigin
        };
    }

    void SavePosition()
    {
        var handler = new BinaryDataHandler(GameManager.PlayerDataDirPath, saveString);

        handler.SaveData(writer =>
        {
            writer.Write(transform.position.x);
            writer.Write(transform.position.y);
        });
    }

    void LoadPosition()
    {
        BinaryDataHandler dataHandler = new(GameManager.PlayerDataDirPath, saveString);

        if (!dataHandler.FileExists())
        {
            transform.position = Vector3.zero;
            return;
        }

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
        //public Sprite ne;
        //public Sprite se;
        //public Sprite sw;
        //public Sprite nw;
        //IDEA alpha 1.2: add diagonal movement sprites
    }
}
