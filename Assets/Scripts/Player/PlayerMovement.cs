using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float vertInput;
    float horInput;
    [SerializeField] float speed = 0.015f;
    [SerializeField] Texture2D cursor;
    [SerializeField] Transform cam;

    void Start()
    {
        //Cursor.SetCursor(cursor,Vector2.zero,CursorMode.Auto);         
        Cursor.lockState = CursorLockMode.Confined;

        string[] pos = PlayerPrefs.GetString("pos","0,0,0").Split(",");
        transform.position = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
    }

    void FixedUpdate()
    {
        vertInput = Input.GetAxis("Vertical");
        horInput = Input.GetAxis("Horizontal");

        transform.Translate(new Vector2(horInput, vertInput).normalized * speed);
    }

    void OnDestroy()
    {
        PlayerPrefs.SetString("pos", transform.position.x + "," + transform.position.y + "," + transform.position.z);
    }
}
