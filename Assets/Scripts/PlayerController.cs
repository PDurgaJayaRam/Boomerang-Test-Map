using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    void Update()
    {
        // 1. Read WASD / Arrow keys
        float h = Input.GetAxisRaw("Horizontal");   // A/D or ←/→
        float v = Input.GetAxisRaw("Vertical");     // W/S or ↑/↓

        // 2. Convert to isometric directions
        Vector3 forward  = new Vector3( 1, 0,  1).normalized;   // ↗
        Vector3 right    = new Vector3( 1, 0, -1).normalized;   // ↘
        Vector3 moveDir  = (forward * v + right * h).normalized;

        // 3. Move the cube
        transform.position += moveDir * speed * Time.deltaTime;
    }
}
